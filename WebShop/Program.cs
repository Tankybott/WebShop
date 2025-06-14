using Serilog;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using WebShop.DependencyInjections;
using WebShop.ProgramConfiguration;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing;
using ControllersServices.ProductManagement;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Stripe;
using BackgroundServices.BackgroundProcessors;
using Mailjet.Client;
using Utility.Common;
using Microsoft.AspNetCore.Identity.UI.Services;
using Services.EmailSender;
using DinkToPdf.Contracts;
using DinkToPdf;
using Services.UsersServices;
using Microsoft.AspNetCore.Identity;
using Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Get environment name
var environment = builder.Environment.EnvironmentName;

// Configure app settings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    // Add core services
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // Configure database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Log.Information("Using connection string: {ConnectionString}", connectionString);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        );
    });

    // Register repositories, services, and authentication
    builder.Services.AddReoistoryServices();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddCategoryServices();
    builder.Services.AddProductServices();
    builder.Services.AddUtilityServices();
    builder.Services.AddCartServices();
    builder.Services.AddProductBrowserServices();
    builder.Services.AddOrderServices();
    builder.Services.AddCarrierService();
    builder.Services.AddWebshopConfig();
    builder.Services.AddScoped<IUsersService, UsersService>();

    builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));

    builder.Services.AddSingleton<IMailjetClient>(sp =>
    {
        var config = builder.Configuration.GetSection("Mailjet");
        var publicKey = config["PublicKey"];
        var privateKey = config["PrivateKey"];
        return new MailjetClient(publicKey, privateKey);
    });

    builder.Services.AddTransient<IEmailSender, EmailSender>();


    //stripe
    StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<String>();

    // Configure authentication
    builder.Services.AddCustomAuthentication(builder.Configuration);

    // Configure external authentication providers (Facebook & Google)
    builder.Services.AddAuthentication()
        .AddFacebook(options =>
        {
            var facebookAuth = builder.Configuration.GetSection("Authentication:Facebook");
            options.AppId = facebookAuth["ClientId"];
            options.AppSecret = facebookAuth["ClientSecret"];
        })
        .AddGoogle(options =>
        {
            var googleAuth = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = googleAuth["ClientId"] ?? throw new ArgumentNullException("Google ClientId is missing!");
            options.ClientSecret = googleAuth["ClientSecret"] ?? throw new ArgumentNullException("Google ClientSecret is missing!");
        });

    // Configure JSON serialization
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

    // Configure AutoMapper
    builder.Services.AddAutoMapper(typeof(ProductFormToProductMapper).Assembly);

    // Register background services
    builder.Services.AddHostedService<DiscountActivationService>();
    builder.Services.AddHostedService<DiscountDeletionService>();
    builder.Services.AddHostedService<CartDeletionBackgroundService>();

    //services.AddHostedService<CartDeletionBackgroundService>();
    builder.Services.AddHostedService<OrderDeletionBackgroundService>();

    // Configure localization
    var cultureInfo = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new RequestCulture(cultureInfo);
        options.SupportedCultures = new List<CultureInfo> { cultureInfo };
        options.SupportedUICultures = new List<CultureInfo> { cultureInfo };
    });

    // Build the app
    var app = builder.Build();

    app.UseExceptionHandler("/User/Home/Error");
    app.UseHsts();

    // Configure middleware
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRequestLocalization();

    // Configure endpoint routing
    app.MapControllerRoute(
        name: "default",
        pattern: "{area=User}/{controller=ProductBrowser}/{action=Index}/{id?}");
    app.MapRazorPages();

    // Seed roles and users
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var config = services.GetRequiredService<IConfiguration>();

        await DbInitializer.SeedRolesAndAdminsAsync(roleManager, userManager, config);
    }

    // Run the app
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
