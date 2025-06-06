

using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Models.DTOs;
using Models.FormModel;
using Models.ViewModels;
using Services.OrderServices.Interfaces;
using Stripe.Checkout;
using Stripe.Climate;
using Utility.Common.Interfaces;
using Utility.Constants;

namespace Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderCreator _orderCreator;
        private readonly IOrderVMManager _orderVMManager;
        private readonly IOrderStockReducer _orderStockReducer;
        private readonly IUserRetriver _userRetriver;
        private readonly IOrderHeaderManager _orderHeaderManager;

        public OrderService(IUnitOfWork unitOfWork, IUserRetriver userRetriver, IOrderCreator orderCreator, IOrderVMManager orderVMManager, IOrderStockReducer orderStockReducer, IOrderHeaderManager orderHeaderManager)
        {
            _unitOfWork = unitOfWork;
            _orderCreator = orderCreator;
            _orderVMManager = orderVMManager;
            _orderStockReducer = orderStockReducer;
            _userRetriver = userRetriver;
            _orderHeaderManager = orderHeaderManager;
        }

        public async Task<bool> ProcessSucessPayementAsync(int orderHeaderId) 
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(h => h.Id == orderHeaderId, includeProperties: "OrderDetails");
            if (orderHeader != null)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower()=="paid") 
                {
                    orderHeader.PaymentIntentId = session.PaymentIntentId;
                    orderHeader.OrderStatus = OrderStatuses.PaymentConfirmed;
                    orderHeader.PaymentDate = DateTime.Now;
                    _unitOfWork.OrderHeader.Update(orderHeader);
                    await _unitOfWork.SaveAsync();
                    await _orderStockReducer.ReduceStockByOrderDetailsAsync(orderHeader.OrderDetails);
                    return true;
                } else 
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }

        public async Task<IEnumerable<OrderDTO>> GetOrderTableDTOEntitiesAsync() 
        {
            var DTOs = await _unitOfWork.OrderHeader.GetOrderTableDtoAsync();
            var currentUser = _userRetriver.GetCurrentUser();
            var currentUserId = _userRetriver.GetCurrentUserId();
            if (currentUser.IsInRole(IdentityRoleNames.HeadAdminRole) || currentUser.IsInRole(IdentityRoleNames.AdminRole))
            {
                return DTOs;
            }
            else 
            {
                return DTOs.Where(d => d.ApplicationUserId == currentUserId);
            }
        }

        public async Task StartProcessingAsync(int orderHeaderId) 
        {
            var orderToUpdateHeader =  await  _unitOfWork.OrderHeader.GetAsync(o => o.Id == orderHeaderId);
            if (orderToUpdateHeader != null) 
            {
                orderToUpdateHeader.OrderStatus = OrderStatuses.Processing;
                _unitOfWork.OrderHeader.Update(orderToUpdateHeader);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task SetOrderSentAsync(int orderHeaderId) 
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(o => o.Id == orderHeaderId);
            if (orderHeader != null) 
            {
                if (!ValidateOrderToSend(orderHeader)) throw new InvalidOperationException("some of order values are null or empty");

                orderHeader.OrderStatus = OrderStatuses.Shipped;
                _unitOfWork.OrderHeader.Update(orderHeader);
                await _unitOfWork.SaveAsync();
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
