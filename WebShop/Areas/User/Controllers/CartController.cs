using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Serilog;
using Services.CartServices.CustomExeptions;
using Services.CartServices.Interfaces;
using Services.ProductManagement.Interfaces;
using Services.ProductService.Interfaces;
using System.Security.Claims;

namespace WebShop.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartServices _cartServices;
        private readonly IProductPriceRetriver _productPriceRetriver;

        public CartController(IUnitOfWork unitOfWork, IProductService productService, ICartServices cartServices, IProductPriceRetriver productPriceRetriver)
        {
            _unitOfWork = unitOfWork;
            _cartServices = cartServices;
            _productPriceRetriver = productPriceRetriver;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var webshopConfig = await _unitOfWork.WebshopConfig.GetAsync();
                ViewBag.Currency = webshopConfig.Currency;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userCart = await _unitOfWork.Cart.GetAsync(c => c.UserId == userId, includeProperties: "Items,Items.Product,Items.Product.PhotosUrlSets");

                return View(userCart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while loading cart for user.");
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
            }
        }

        #region ApiCalls

        [HttpGet]
        public async Task<IActionResult> GetCurrentProductPrice(int productId)
        {
            try
            {
                var currentProductPrice = await _productPriceRetriver.GetProductPriceAsync(productId);
                return Json(new { success = true, data = currentProductPrice, message = "" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get current price for productId={ProductId}", productId);
                return Json(new { success = false, message = "Something went wrong" });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> SynchronizeCartPrices(int cartId)
        {
            try
            {
                IEnumerable<int> modifiedCartItemsIds = await _cartServices.SynchronizeCartPrices(cartId);
                return Json(new { success = true, modifiedCartItemsIds = modifiedCartItemsIds, message = "" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to synchronize prices for cartId={CartId}", cartId);
                return Json(new { success = false, message = "Something went wrong" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            try
            {
                await _cartServices.RemoveCartItemAsync(id);
                TempData["success"] = "Item deleted successfully";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete cart item with id={CartItemId}", id);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCartItemQuantity(int cartItemId, int newQuantity)
        {
            try
            {
                await _cartServices.UpdateCartItemQantityAsync(cartItemId, newQuantity);
                return Json(new { success = true });
            }
            catch (NotEnoughQuantityException ex)
            {
                return Json(new { success = false, quantityLeft = ex.MaxAvailableQuantity });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update quantity for cartItemId={CartItemId}", cartItemId);
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItemsQuantity()
        {
            try
            {
                var newQuantity = await _cartServices.GetCartItemsQantityAsync();
                return Json(new { success = true, newQuantity = newQuantity });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get cart item quantities");
                return Json(new { success = false });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> ValidateCartQuantity([FromForm] IEnumerable<CartItemQuantityDTO> CollectionOfDTOs)
        {
            try
            {
                var maxQuantityOfItems = await _cartServices.ValidateCartProductsQuantityAsync(CollectionOfDTOs);
                return Json(new { success = true, itemsWithMaxQuantity = maxQuantityOfItems });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to validate cart quantities");
                return Json(new { success = false });
            }
        }

        #endregion
    }
}
