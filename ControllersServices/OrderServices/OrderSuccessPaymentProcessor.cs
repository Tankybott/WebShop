

using DataAccess.Repository.IRepository;
using Utility.Constants;
using Stripe.Checkout;
using Services.OrderServices.Interfaces;
using Services.EmailFactory.interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Services.OrderServices
{
    public class OrderSuccessPaymentProcessor : IOrderSuccessPaymentProcessor
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderStockReducer _orderStockReducer;
        private readonly IOrderDetailsPriceCalculator _orderDetailsPriceCalculator;
        private readonly IEmailFactory _emailFactory;
        private readonly IEmailSender _emailSender;

        public OrderSuccessPaymentProcessor(IUnitOfWork unitOfWork, IOrderStockReducer orderStockReducer, IOrderDetailsPriceCalculator orderDetailsPriceCalculator, IEmailFactory emailFactory, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _orderStockReducer = orderStockReducer;
            _orderDetailsPriceCalculator = orderDetailsPriceCalculator;
            _emailFactory = emailFactory;
            _emailSender = emailSender;
        }

        public async Task<bool> ProcessAsync(int orderHeaderId)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                h => h.Id == orderHeaderId,
                includeProperties: "OrderDetails,ApplicationUser,Carrier");

            if (orderHeader == null)
                return false;

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                orderHeader.OrderStatus = OrderStatuses.PaymentConfirmed;
                orderHeader.PaymentDate = DateTime.Now;

                _unitOfWork.OrderHeader.Update(orderHeader);
                await _unitOfWork.SaveAsync();

                await _orderStockReducer.ReduceStockByOrderDetailsAsync(orderHeader.OrderDetails);

                await SendOrderConfirmationEmailAsync(orderHeader);
                return true;
            }

            return false;
        }

        private async Task SendOrderConfirmationEmailAsync(Models.DatabaseRelatedModels.OrderHeader orderHeader)
        {
            if (orderHeader.ApplicationUser?.Email == null)
                return;

            var webshopConfig = await _unitOfWork.WebshopConfig.GetAsync();
            if (webshopConfig == null)
                return;

            decimal shippingPrice = 0;
            if (orderHeader.Carrier != null)
            {
                shippingPrice = _orderDetailsPriceCalculator.CalculateDetailsPrice(orderHeader.OrderDetails, orderHeader.Carrier);
            }

            if (shippingPrice == 0)
                return;

            var emailBody = _emailFactory.BuildOrderEmail(
                orderHeader,
                "Your payment has been successfully processed. Below is the summary of your order:",
                webshopConfig.Currency,
                shippingPrice
            );

            await _emailSender.SendEmailAsync(
                orderHeader.ApplicationUser.Email,
                "Order Confirmation",
                emailBody
            );
        }

    }
}
