using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.UsersServices;
using Utility.Constants;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.TestAdmin)]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Api Calls
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var usersDto = await _usersService.GetUsersTableDtoAsync();
                return Json(new { success = true, data = usersDto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> ToggleUserBan([FromBody] string email)
        {
            try
            {
                var success = await _usersService.ToggleBanUserAsync(email);
                if (!success)
                {
                    return Json(new { success = false, message = "User not found or operation failed." });
                }

                return Json(new { success = true, message = "Ban status toggled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }

}
