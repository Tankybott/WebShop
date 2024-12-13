using ControllersServices.ProductService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;

namespace WebShop.Areas.Admin.Controllers
{
    // delete product allong with deleted categories
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // make try catch 
        public async Task<IActionResult> Index()
        {
            var productVM = await _productService.GetProductVMForIndexAsync();
            return View(productVM);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            try
            {
                var categoryVM = await _productService.GetProductVMAsync(id);
                return View(categoryVM);
            }
            catch (Exception)
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index");
            }

        }

        #region Api Calls

        [HttpGet]
        public async Task<IActionResult> GetAllForTable(string? categoryFilter)
        {
            try
            {
                var products = await _productService.GetProductsForTableAsync(categoryFilter);
                return Json(new { data = products });
            }
            catch (Exception)
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
        }

        [HttpPost]
        // usunac return jsony zmienic handling tego na froncie w product handler i dodac redirect tutaj 
        // dodac handling exception z Discountem 
        public async Task<IActionResult> UpsertAjax([FromForm] ProductFormModel model)
        {
            var extraTopicsJson = Request.Form["ExtraTopics"];
            if (!string.IsNullOrEmpty(extraTopicsJson))
            {
                model.ExtraTopics = JsonConvert.DeserializeObject<List<ProductBase.ExtraTopic>>(extraTopicsJson);
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Something went wrong, try again later";
                return Json(new { success = false });
            }

            try
            {
                await _productService.UpsertAsync(model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, try again later";
                return Json(new { success = false });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) 
        {
            try
            {
                await _productService.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        #endregion
    }
}
