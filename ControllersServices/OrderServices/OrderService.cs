

using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.DatabaseRelatedModels;
using Models.DTOs;
using Models.FormModel;
using Models.ViewModels;
using Serilog;
using Services.OrderServices.Interfaces;
using Stripe;
using Stripe.Checkout;
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

        public OrderService(IUnitOfWork unitOfWork, IUserRetriver userRetriver, IOrderCreator orderCreator, IOrderVMManager orderVMManager, IOrderStockReducer orderStockReducer)
        {
            _unitOfWork = unitOfWork;
            _orderCreator = orderCreator;
            _orderVMManager = orderVMManager;
            _orderStockReducer = orderStockReducer;
            _userRetriver = userRetriver;
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

        public async Task<string> CreateOrderAsync(OrderFormModel formModel) 
        {
            return await _orderCreator.CreateAsync(formModel);
        }

        public async Task<OrderVM> GetVmForNewOrderAsync()
        {
            return await _orderVMManager.CreateVmForNewOrderAsync();
        }


        //private async Task<OrderVM> CreateOrderVM(IEnumerable<OrderDetail>? orderDetails, OrderHeader orderHeader)
        //{
        //    var currentApplicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == _userRetriver.GetCurrentUserId());
        //    OrderVM vm = new OrderVM();
        //    if (currentApplicationUser != null)
        //    {
        //        vm = new OrderVM
        //        {
        //            OrderDetails = orderDetails,
        //            OrderHeader = orderHeader,
        //        };
        //    }

        //    return vm;
        //}    
    }
}
