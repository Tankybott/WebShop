using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DatabaseRelatedModels;
using Serilog;
using Services.CarrierService.Interfaces;
using Services.WebshopConfigServices.Interfaces;
using Utility.Constants;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole + "," + IdentityRoleNames.TestAdmin)]
    public class CarrierController : Controller
    {
        private readonly ICarrierService _carrierService;
        private readonly IFreeShippingThresholdManager _freeShippingThresholdManager;

        public CarrierController(ICarrierService carrierService, IFreeShippingThresholdManager freeShippingThresholdManager)
        {
            _carrierService = carrierService;
            _freeShippingThresholdManager = freeShippingThresholdManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _freeShippingThresholdManager.GetFreeShippingThresholdAsync());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Carrier/Index while retrieving free shipping threshold.");
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
            }
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            try
            {
                return View(await _carrierService.GetCarrierAsync(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Carrier/Upsert while retrieving carrier with id={Id}", id);
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> Upsert(Carrier carrier)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _carrierService.Upsert(carrier);
                    TempData["success"] = carrier.Id == 0 ? "Carrier added successfully" : "Carrier updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(carrier);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Carrier/Upsert while saving carrier with id={Id}", carrier?.Id);
                TempData["error"] = "Something went wrong, try again later";
                return View(carrier);
            }
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll(string filter)
        {
            try
            {
                var carriers = await _carrierService.GetAllCarriers();
                return Json(new { data = carriers });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Carrier/GetAll.");
                return Json(new { data = new object[0] });
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _carrierService.DeleteById(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete carrier with id={Id}", id);
                return Json(new { success = false });
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpPut]
        public async Task<IActionResult> SetFreeShippingFromPrice([FromBody] decimal? price)
        {
            try
            {
                await _freeShippingThresholdManager.UpdateFreeShippingThresholdAsync(price);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update free shipping from price. Input price={Price}", price);
                return Json(new { success = false });
            }
        }

        #endregion
    }
}
