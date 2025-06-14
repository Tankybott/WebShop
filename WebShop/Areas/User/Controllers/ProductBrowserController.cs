using ControllersServices.ProductBrowserService.Interfaces;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.ProductFilterOptions;
using Serilog;

namespace WebShop.Areas.User.Controllers
{
    [Area("User")]
    public class ProductBrowserController : Controller
    {
        private readonly IProductBrowserService _productBrowserService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductBrowserController(IProductBrowserService productBrowserService, IUnitOfWork unitOfWork)
        {
            _productBrowserService = productBrowserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var productBrowserVm = await _productBrowserService.GetProductBrowserVM();
                return View(productBrowserVm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load ProductBrowser view.");
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
            }
        }

        #region ApiCalls

        [HttpGet]
        public async Task<IActionResult> GetChoosenProducts([FromQuery] ProductFilterOptionsRequest filterOption)
        {
            try
            {
                var products = await _productBrowserService.GetFilteredProductsDTO(filterOption);
                return Json(new { data = products });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching filtered products with options: {@FilterOption}", filterOption);
                return Json(new { data = Array.Empty<object>(), success = false });
            }
        }

        #endregion
    }
}
