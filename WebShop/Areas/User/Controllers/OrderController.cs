using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.FormModel;
using Models.ViewModels;
using Serilog;
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
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load orders list.");
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
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to prepare new order.");
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
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load order details for id={Id}", id);
                TempData["error"] = "Unexpected error while loading order details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = $"{IdentityRoleNames.AdminRole},{IdentityRoleNames.HeadAdminRole}")]
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
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update order header for id={Id}", vm?.OrderHeader?.Id);
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
                    TempData["error"] = "Something went wrong. Please check your order status...";
                    return RedirectToAction("Index", "ProductBrowser");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during order confirmation for orderHeaderId={Id}", orderHeaderId);
                TempData["error"] = "Something went wrong. Please check your order status...";
                return RedirectToAction("Index", "ProductBrowser");
            }
        }

        #region API Calls

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCarrier(int carrierId)
        {
            try
            {
                var carrier = await _unitOfWork.Carrier.GetAsync(c => c.Id == carrierId);
                return carrier != null
                    ? Json(new { success = true, carrier })
                    : Json(new { success = false });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get carrier info for carrierId={Id}", carrierId);
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
                return DTOs != null
                    ? Json(new { success = true, dtos = DTOs })
                    : Json(new { success = false });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch orders for current user.");
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
                Log.Error(ex, "Stripe redirect failed during order creation.");
                return Json(new
                {
                    success = true,
                    paymentRedirectionScuess = false,
                    messege = "There was a problem redirecting to the payment page..."
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create order.");
                return Json(new
                {
                    success = false,
                    paymentRedirectionSuccess = false,
                    message = "Something went wrong, try again later"
                });
            }
        }

        [Authorize(Roles = $"{IdentityRoleNames.AdminRole},{IdentityRoleNames.HeadAdminRole}")]
        [HttpPost]
        public async Task<IActionResult> StartProcessing([FromBody] int orderId)
        {
            try
            {
                await _orderService.StartProcessingOrderAsync(orderId);
                TempData["success"] = "Order status updated";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to start processing for orderId={Id}", orderId);
                return Json(new { success = false });
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
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to mark order as sent. orderId={Id}", orderId);
                return Json(new { success = false });
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
                Log.Error(ex, "Failed to generate PDF for order id={Id}", id);
                TempData["error"] = "Failed to generate the PDF. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion
    }
}