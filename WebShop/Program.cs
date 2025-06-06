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
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    // Add core services
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // Configure database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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

    // Error handling
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/User/Home/Error");
        app.UseHsts();
    }

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
        pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    // Debugging: Print available endpoints
    app.Use(async (context, next) =>
    {
        var endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
        foreach (var endpoint in endpointDataSource.Endpoints)
        {
            Console.WriteLine(endpoint.DisplayName);
        }
        await next();
    });

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
