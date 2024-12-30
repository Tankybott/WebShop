using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;



namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IDiscountService _discountService;
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IProductService productService, IDiscountService discountService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _discountService = discountService;
            _unitOfWork = unitOfWork;
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

                TempData["success"] = model.Id != 0 ? "Product upserted successfully" : "Product added successfully";
                return Json(new { });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong, try again later";
                return Json(new { });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
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
