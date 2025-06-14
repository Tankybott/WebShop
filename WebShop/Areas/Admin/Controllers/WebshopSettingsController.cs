﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Services.WebshopConfigServices.Interfaces;
using Utility.Constants;
using Serilog;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole + "," + IdentityRoleNames.TestAdmin)]
    public class WebshopSettingsController : Controller
    {
        private readonly IWebshopSettingsServices _webshopSettingsServices;

        public WebshopSettingsController(IWebshopSettingsServices webshopSettingsServices)
        {
            _webshopSettingsServices = webshopSettingsServices;
        }

        public async Task<IActionResult> Update()
        {
            return View(await _webshopSettingsServices.GetVmAsync());
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> Update(WebshopSettingsVm vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _webshopSettingsServices.UpdateAsync(vm);
                    TempData["success"] = "Settings updated";
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
                else
                {
                    var model = await _webshopSettingsServices.GetVmAsync();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating webshop settings.");
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
        }
    }
}
