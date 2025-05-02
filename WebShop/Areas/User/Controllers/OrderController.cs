using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DatabaseRelatedModels;
using Models.FormModel;
using Services.OrderServices.Interfaces;
using Stripe;

namespace WebShop.Areas.User.Controllers
{
    [Area("User")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IOrderService orderService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Details(int? orderId) 
        {
            return View();
        }

        //public async Task<IActionResult> OrderSucess(int orderHeaderId)
        //{
        //    try
        //    {
        //        await _orderService.ProcessSucessPayementAsync(orderHeaderId);
        //        return Json(new
        //        {
        //            success = true,
        //            paymentRedirectionScuess = true,
        //        });
        //    }
        //    catch (StripeException)
        //    {
        //        return Json(new { success = true, paymentRedirectionScuess = false, messege = "There was a problem redirecting to the payment page. Please try to retry the payment from your orders list. If the payment is not completed within 1 hour, the order will be deleted. If the problem persists, please contact the administration." });
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new
        //        {
        //            success = true,
        //            paymentRedirectionSuccess = false,
        //            message = "Something went wrong, try again later"
        //        });
        //    };
        //}
        [Authorize]
        public async Task<IActionResult> CreateNewOrder() {
            var vm = await _orderService.GetVmForNewOrderAsync();
            return View(vm);
        }

        public async Task<IActionResult> OrderConfirmation(int orderHeaderId)
        {
            try
            {
                var isPaid = await _orderService.ProcessSucessPayementAsync(orderHeaderId);
                if (isPaid)
                {
                    return View();
                }
                else
                {
                    TempData["error"] = "Something went wrong. Please check your order status in the orders table and verify it via email. If you encounter any issues, contact the administration.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex) 
            {
                TempData["error"] = "Something went wrong. Please check your order status in the orders table and verify it via email. If you encounter any issues, contact the administration.";
                return RedirectToAction("Index", "Home");
            }
        }

        #region ApiCalls 
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCarrier(int carrierId) 
        {
            try {
                var carrier = await _unitOfWork.Carrier.GetAsync(c => c.Id == carrierId);
                if (carrier != null)
                {
                    return Json(new { success = true, carrier = carrier });
                }
                else 
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders() 
        {
            try
            {
                var DTOs = await _orderService.GetOrderTableDTOEntitiesAsync();
                if (DTOs != null)
                {
                    return Json(new { success = true, dtos = DTOs });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] OrderFormModel formModel) 
        {
            try
            {
                var redirectString = await _orderService.CreateOrderAsync(formModel);
                return Json(new
                {
                    success = true,
                    paymentRedirectionSuccess = true,
                    message = redirectString
                });
            }
            catch (StripeException ex)
            {
                return Json(new { success = true, paymentRedirectionScuess = false, messege = "There was a problem redirecting to the payment page. Please try to retry the payment from your orders list. If the payment is not completed within 1 hour, the order will be deleted. If the problem persists, please contact the administration." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    paymentRedirectionSuccess = false,
                    message = "Something went wrong, try again later"
                });
            };
        }

        #endregion
    }
}
