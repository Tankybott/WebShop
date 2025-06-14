﻿using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.FormModel;
using Services.CartServices.CustomExeptions;
using Services.CartServices.Interfaces;
using Serilog;

namespace WebShop.Areas.User.Controllers
{
    [Area("User")]
    public class ProductDetailsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartServices _cartServices;

        public ProductDetailsController(IUnitOfWork unitOfWork, ICartServices cartServices)
        {
            _unitOfWork = unitOfWork;
            _cartServices = cartServices;
        }

        public async Task<IActionResult> Details(int productId)
        {
            try
            {
                var product = await _unitOfWork.Product.GetAsync(p => p.Id == productId);
                var webshopConfig = await _unitOfWork.WebshopConfig.GetAsync();
                ViewBag.Currency = webshopConfig.Currency;
                return View("~/Views/Shared/ProductDetails.cshtml", product);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load product details for productId={ProductId}", productId);
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
            }
        }

        #region ApiCalls
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddItemToCart([FromForm] CartItemFormModel model)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("AddItemToCart called with invalid model state: {@Model}", model);
                return Json(new
                {
                    success = false,
                    message = "Something went wrong, try again later."
                });
            }

            try
            {
                await _cartServices.AddItemToCartAsync(model);
                return Json(new
                {
                    success = true,
                    message = "Item successfully added to cart!"
                });
            }
            catch (DiscountOutOfDateException)
            {
                try
                {
                    model.IsAddedWithDiscount = false;
                    await _cartServices.AddItemToCartAsync(model);
                    return Json(new
                    {
                        success = true,
                        message = "Unfortunately, the discount is no longer available. The item was added to the cart at the regular price."
                    });
                }
                catch (Exception fallbackEx)
                {
                    Log.Error(fallbackEx, "Fallback after DiscountOutOfDateException failed for model: {@Model}", model);
                    return Json(new
                    {
                        success = false,
                        message = "Something went wrong, try again later."
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add item to cart for model: {@Model}", model);
                return Json(new
                {
                    success = false,
                    message = "Something went wrong, try again later."
                });
            }
        }

        #endregion
    }
}
