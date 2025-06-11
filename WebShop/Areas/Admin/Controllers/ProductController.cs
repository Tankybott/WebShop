using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.FormModel;
using Services.ProductManagement.Interfaces;
using Utility.Constants;


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
            catch (Exception)
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index");
            }

        }

        #region Api Calls
        [HttpGet]
        public async Task<IActionResult> GetAllForTable(int? categoryFilter, string? productFilterOption)
        {
            var products = await _productService.GetProductsForTableAsync(categoryFilter, productFilterOption);
            return Json(new { data = products });
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> UpsertAjax([FromForm] ProductFormModel model)
        {
            if (!ModelState.IsValid)
            {
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
                return Json(new { success = true,});
            }
            catch (Exception ex)
            {
                TempData["error"] = "There was en error when deleting product";
                return Json(new { success = false,});
            }
        }
        #endregion
    }
}
