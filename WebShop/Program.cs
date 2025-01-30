using Serilog;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using WebShop.DependencyInjections;
using System.Text.Json.Serialization;
using ControllersServices.ProductManagement;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();
    builder.Services.AddDbContext<ApplicationDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddReoistoryServices();
    builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = $"/Identity/Account/Login";
        options.LogoutPath = $"/Identity/Account/Logout";
        options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

    });
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddCategoryServices();
    builder.Services.AddProductServices();
    builder.Services.AddUtilityServices();
    builder.Services.AddProductBrowserServices();
    builder.Services.AddAuthentication().AddFacebook(option => {
        option.AppId = "990980045930779";
        option.AppSecret = "7cfa3f805571384be84a996b6bb60af7";
    });

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

    builder.Services.AddAutoMapper(typeof(ProductFormToProductMapper).Assembly);

    builder.Services.AddHostedService<DiscountActivationService>();
    builder.Services.AddHostedService<DiscountDeletionService>();

    var cultureInfo = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new RequestCulture(cultureInfo);
        options.SupportedCultures = new List<CultureInfo> { cultureInfo };
        options.SupportedUICultures = new List<CultureInfo> { cultureInfo };
    });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/User/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseRequestLocalization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.Use(async (context, next) =>
    {
        var endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
        foreach (var endpoint in endpointDataSource.Endpoints)
        {
            Console.WriteLine(endpoint.DisplayName);
        }
        await next();
    });

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
