

using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity.UI.Services;
using Models.DatabaseRelatedModels;
using Models.DTOs;
using Models.FormModel;
using Models.ViewModels;
using Services.EmailFactory.interfaces;
using Services.OrderServices.Interfaces;
using Stripe.Checkout;
using Stripe.Climate;
using Utility.Common.Interfaces;
using Utility.Constants;

namespace Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IOrderCreator _orderCreator;
        private readonly IOrderVMManager _orderVMManager;
        private readonly IOrderHeaderManager _orderHeaderManager;
        private readonly IOrderStatusManager _orderStatusManager;   
        private readonly IOrderTableDtoRetriever _orderTableDtoRetriever;
        private readonly IOrderSuccessPaymentProcessor _orderSuccessPaymentProcessor;

        public OrderService(IUnitOfWork unitOfWork, IOrderCreator orderCreator, IOrderVMManager orderVMManager, IOrderStockReducer orderStockReducer, IOrderHeaderManager orderHeaderManager, IOrderStatusManager orderStatusManager, IOrderTableDtoRetriever orderTableDtoRetriever, IOrderSuccessPaymentProcessor orderSuccessPaymentProcessor)
        {
            _orderCreator = orderCreator;
            _orderVMManager = orderVMManager;
            _orderHeaderManager = orderHeaderManager;
            _orderStatusManager = orderStatusManager;
            _orderTableDtoRetriever = orderTableDtoRetriever;
            _orderSuccessPaymentProcessor = orderSuccessPaymentProcessor;
        }

        public async Task<bool> ProcessSucessPayementAsync(int orderHeaderId)
        {
            return await _orderSuccessPaymentProcessor.ProcessAsync(orderHeaderId);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrderTableDTOEntitiesAsync() 
        {
           return await _orderTableDtoRetriever.GetEntitiesAsync();
        }

        public async Task StartProcessingOrderAsync(int orderHeaderId)
        {
            await _orderStatusManager.StartProcessingAsync(orderHeaderId);          
        }

        public async Task SendOrderAsync(int orderHeaderId)
        {
            await _orderStatusManager.SendAsync(orderHeaderId);
        }

        public async Task<string> CreateOrderAsync(OrderFormModel formModel) 
        {
            return await _orderCreator.CreateAsync(formModel);
        }

        public async Task<OrderVM> GetVmForNewOrderAsync()
        {
            return await _orderVMManager.CreateVmForNewOrderAsync();
        }

        public async Task <OrderVM> GetOrderVMByIdAsync(int orderHeaderId)  
        {
           return await _orderVMManager.GetVmByIdAsync(orderHeaderId);
        }

        public async Task UpdateOrderHeaderAsync(OrderHeader orderHeader) 
        {
             await _orderHeaderManager.UpdateAsync(orderHeader);
        }
    }
}
