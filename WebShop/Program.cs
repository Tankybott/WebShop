using Serilog;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using WebShop.DependencyInjections;
using System.Text.Json.Serialization;
using ControllersServices.ProductManagement;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); 

try
{
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();
    builder.Services.AddDbContext<ApplicationDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddReoistoryServices();
    builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddCategoryServices();
    builder.Services.AddProductServices();
    builder.Services.AddUtilityServices();
    builder.Services.AddProductBrowserServices();


    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

    builder.Services.AddAutoMapper(typeof(ProductFormToProductMapper).Assembly);

    builder.Services.AddHostedService<DiscountActivationService>();
    builder.Services.AddHostedService<DiscountDeletionService>();


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