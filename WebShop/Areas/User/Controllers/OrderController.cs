using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.FormModel;
using Models.ViewModels;
using Services.OrderServices.Interfaces;
using Stripe;
using Utility.Common.Interfaces;
using Utility.Constants;

namespace WebShop.Areas.User.Controllers
{
    [Area("User")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPdfFileGenerator _pdfFileGenerator;
        private readonly IOrderTableHTMLBuilder _orderTableHtmlBuilder;
        private readonly IFileService _fileService;

        public OrderController(IOrderService orderService, IUnitOfWork unitOfWork, IPdfFileGenerator pdfFileGenerator, IOrderTableHTMLBuilder htmlBuilder, IFileService fileService)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
            _pdfFileGenerator = pdfFileGenerator;
            _orderTableHtmlBuilder = htmlBuilder;
            _fileService = fileService;
        }

        [Authorize]
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch
            {
                TempData["error"] = "Unexpected error while loading the orders list.";
                return Redirect("/");
            }
        }

        [Authorize]
        public async Task<IActionResult> CreateNewOrder()
        {
            try
            {
                var vm = await _orderService.GetVmForNewOrderAsync();
                return View(vm);
            }
            catch
            {
                TempData["error"] = "Unexpected error while preparing a new order.";
                return Redirect("/");
            }
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var vm = await _orderService.GetOrderVMByIdAsync(id);
                return View(vm);
            }
            catch
            {
                TempData["error"] = "Unexpected error while loading order details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(OrderVM vm)
        {
            try
            {
                var orderHeader = vm.OrderHeader;

                if (orderHeader != null)
                {
                    await _orderService.UpdateOrderHeaderAsync(orderHeader);
                    TempData["success"] = "Order details updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
                }
                else
                {
                    TempData["error"] = "Order header not found.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                TempData["error"] = "Unexpected error while updating order.";
                return Redirect("/");
            }
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

        [Authorize(Roles = $"{IdentityRoleNames.AdminRole},{IdentityRoleNames.HeadAdminRole}")]
        [HttpPost]
        public async Task<IActionResult> StartProcessing([FromBody] int orderId) 
        {
            try 
            {
                await _orderService.StartProcessingOrderAsync(orderId);
                TempData["success"] = "Order status updated";
                return Json(new
                {
                    success = true,
                });
            } catch 
            {
                return Json(new
                {
                    success = false,
                });
            }
        }

        [Authorize(Roles = $"{IdentityRoleNames.AdminRole},{IdentityRoleNames.HeadAdminRole}")]
        [HttpPost]
        public async Task<IActionResult> SetOrderSent([FromBody] int orderId) 
        {
            try 
            {
                await _orderService.SendOrderAsync(orderId);
                TempData["success"] = "Order status updated";
                return Json(new
                {
                    success = true,
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                });
            }
        }

        [Authorize(Roles = $"{IdentityRoleNames.AdminRole},{IdentityRoleNames.HeadAdminRole}")]
        public async Task<IActionResult> DownloadOrderPdf(int id)
        {
            try
            {
                var order = await _unitOfWork.OrderHeader.GetAsync(o => o.Id == id, includeProperties: "OrderDetails");
                if (order == null)
                {
                    TempData["error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                var html = _orderTableHtmlBuilder.BuildHtml(order);
                var filePath = await _pdfFileGenerator.GeneratePdfFromHtmlAsync(html);

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                await _fileService.DeleteFileAsync(filePath);

                return File(fileBytes, "application/pdf", $"order_{order.Id}.pdf");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to generate the PDF. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
