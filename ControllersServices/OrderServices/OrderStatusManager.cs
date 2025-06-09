using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity.UI.Services;
using Models.DatabaseRelatedModels;
using Services.EmailFactory.interfaces;
using Services.OrderServices.Interfaces;
using Utility.Constants;

namespace Services.OrderServices
{
    public class OrderStatusManager : IOrderStatusManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailFactory _emailFactory;
        private readonly IEmailSender _emailSender;

        public OrderStatusManager(IUnitOfWork unitOfWork, IEmailFactory emailFactory, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailFactory = emailFactory;
            _emailSender = emailSender;
        }

        public async Task StartProcessingAsync(int orderHeaderId)
        {
            var orderToUpdateHeader = await _unitOfWork.OrderHeader.GetAsync(
                o => o.Id == orderHeaderId,
                includeProperties: "ApplicationUser");

            if (orderToUpdateHeader != null)
            {
                orderToUpdateHeader.OrderStatus = OrderStatuses.Processing;
                _unitOfWork.OrderHeader.Update(orderToUpdateHeader);
                await _unitOfWork.SaveAsync();

                var emailBody = _emailFactory.BuildInformationEmail(
                    "Your Order is Being Processed",
                    new List<string>
                    {
                "We’ve received your order and our team has started processing it.",
                "We’ll notify you once it’s shipped and on its way.",
                "Thanks for choosing us!"
                    }
                );

                if (orderToUpdateHeader.ApplicationUser?.Email != null)
                {
                    await _emailSender.SendEmailAsync(
                    orderToUpdateHeader.ApplicationUser.Email,
                    "Your Order is Now Processing",
                    emailBody
                );
                }
            }
        }

        public async Task SendAsync(int orderHeaderId)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                o => o.Id == orderHeaderId,
                includeProperties: "ApplicationUser");

            if (orderHeader != null)
            {
                if (!ValidateOrderToSend(orderHeader))
                    throw new InvalidOperationException("Some of the order values are null or empty");

                orderHeader.OrderStatus = OrderStatuses.Shipped;
                _unitOfWork.OrderHeader.Update(orderHeader);
                await _unitOfWork.SaveAsync();

                var emailBody = _emailFactory.BuildInformationEmail(
                    "Your Order Has Been Shipped",
                    new List<string>
                    {
                "We wanted to let you know that your order has been handed over to the carrier and is now on its way to you.",
                "You can expect delivery soon. If your order includes a tracking link, you’ll find it in your account or email shortly.",
                "Thank you for shopping with us!"
                    }
                );

                if (orderHeader.ApplicationUser?.Email != null)
                {
                    await _emailSender.SendEmailAsync(
                        orderHeader.ApplicationUser.Email,
                        "Your Order Is On Its Way",
                        emailBody
                        );
                }
            }
        }

        private bool ValidateOrderToSend(OrderHeader orderHeader)
        {
            if (orderHeader == null)
                return false;

            if (string.IsNullOrWhiteSpace(orderHeader.ApplicationUserId)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.OrderStatus)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.TrackingLink)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.SessionId)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.PaymentIntentId)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.PaymentLink)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.Name)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.PhoneNumber)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.StreetAdress)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.City)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.Region)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.PostalCode)) return false;
            if (string.IsNullOrWhiteSpace(orderHeader.Country)) return false;

            if (orderHeader.ShippingDate == default) return false;
            if (orderHeader.CreationDate == default) return false;
            if (orderHeader.PaymentDate == default) return false;

            if (!orderHeader.CarrierId.HasValue) return false;

            return true;

        }
    }
}
