using Microsoft.AspNetCore.Identity;
using Models;
using Utility.Constants;

namespace WebShop.ProgramConfiguration
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminsAsync(RoleManager<IdentityRole> roleManager,
                                                         UserManager<ApplicationUser> userManager,
                                                         IConfiguration configuration)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminsAsync(userManager, configuration);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = new[]
            {
                IdentityRoleNames.HeadAdminRole,
                IdentityRoleNames.AdminRole,
                IdentityRoleNames.UserRole,
                IdentityRoleNames.TestAdmin
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminsAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            var headAdminSection = configuration.GetSection("dbUsersSeed:headAdmin");
            var testAdminSection = configuration.GetSection("dbUsersSeed:testAdmin");

            var headAdminEmail = headAdminSection["email"];
            var headAdminPass = headAdminSection["pass"];

            var testAdminEmail = testAdminSection["email"];
            var testAdminPass = testAdminSection["pass"];

            if (await userManager.FindByEmailAsync(headAdminEmail) == null)
            {
                var headAdmin = new ApplicationUser
                {
                    UserName = headAdminEmail,
                    Email = headAdminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(headAdmin, headAdminPass);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(headAdmin, IdentityRoleNames.HeadAdminRole);
                }
            }

            if (await userManager.FindByEmailAsync(testAdminEmail) == null)
            {
                var testAdmin = new ApplicationUser
                {
                    UserName = testAdminEmail,
                    Email = testAdminEmail,
                    EmailConfirmed = true,
                    Name = "Test Admin",
                    PhoneNumber = "+48123456789",
                    StreetAdress = "123 Main St",
                    City = "Testville",
                    Region = "Test Region",
                    PostalCode = "00-001",
                    Country = "Testland"
                };

                var result = await userManager.CreateAsync(testAdmin, testAdminPass);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testAdmin, IdentityRoleNames.TestAdmin);
                }
            }
        }
    }
}
