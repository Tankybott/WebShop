using DataAccess.Repository.IRepository;
using Models.FormModel;
using Serilog;
using Services.OrderServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OrderServices
{
    public class OrderCreator : IOrderCreator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderHeaderManager _orderHeaderManager;
        private readonly IOrderDetailsCreator _orderItemsCreator;
        private readonly IStripePaymentManager _stripePaymentManager;
        public OrderCreator(IUnitOfWork unitOfWork, IOrderHeaderManager orderHeaderManager, IOrderDetailsCreator orderItemsCreator, IStripePaymentManager stripePaymentManager)
        {
            _unitOfWork = unitOfWork;
            _orderHeaderManager = orderHeaderManager;
            _orderItemsCreator = orderItemsCreator;
            _stripePaymentManager = stripePaymentManager;
        }

        public async Task<string> CreateAsync(OrderFormModel formModel)
        {
            var orderHeader = await _orderHeaderManager.CreateAsync(formModel);
            try
            {
                await _orderItemsCreator.CreateDetailsAsync(formModel.cartId, orderHeader.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create order items");
                _unitOfWork.OrderHeader.Remove(orderHeader);
                await _unitOfWork.SaveAsync();
                throw new Exception("An error occurred while creating the order items.", ex);
            }

            await _stripePaymentManager.MakeStripePaymentAsync(orderHeader);
            return orderHeader.PaymentLink;
        }
    }
}
