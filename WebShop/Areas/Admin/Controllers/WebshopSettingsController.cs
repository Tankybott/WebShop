using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Services.WebshopConfigServices.Interfaces;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                    return View(_webshopSettingsServices.GetVmAsync());
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
            
        }
    }
}
