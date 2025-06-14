using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.FormModel;
using Services.ProductManagement.Interfaces;
using Utility.Constants;
using Serilog;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole + "," + IdentityRoleNames.TestAdmin)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var productVM = await _productService.GetProductVMForIndexAsync();
                return View(productVM);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Product/Index.");
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
            }
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            try
            {
                var productVM = await _productService.GetProductVMAsync(id);
                return View(productVM);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Product/Upsert for id {ProductId}.", id);
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index");
            }
        }

        #region Api Calls

        [HttpGet]
        public async Task<IActionResult> GetAllForTable(int? categoryFilter, string? productFilterOption)
        {
            try
            {
                var products = await _productService.GetProductsForTableAsync(categoryFilter, productFilterOption);
                return Json(new { data = products });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetAllForTable. categoryFilter={CategoryFilter}, productFilterOption={ProductFilterOption}", categoryFilter, productFilterOption);
                return Json(new { data = new object[0] });
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> UpsertAjax([FromForm] ProductFormModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Log.Warning("UpsertAjax called with invalid model. Errors: {Errors}", errorMessages);
                TempData["error"] = "Something went wrong, try again later";
                return Json(new { });
            }

            try
            {
                await _productService.UpsertAsync(model);
                TempData["success"] = model.Id != 0 ? "Product updated successfully" : "Product added successfully";
                return Json(new { });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in UpsertAjax for product {ProductId}.", model.Id);
                TempData["error"] = "Something went wrong, try again later";
                return Json(new { });
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                TempData["success"] = "Product deleted successfully";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting product with id {ProductId}.", id);
                TempData["error"] = "There was an error when deleting product";
                return Json(new { success = false });
            }
        }

        #endregion
    }
}
