using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.DatabaseRelatedModels;
using Models.ViewModels;
using Services.OrderServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common.Interfaces;

namespace Services.OrderServices
{
    public class OrderVMManager : IOrderVMManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderHeaderManager _headerManager;
        private readonly IUserRetriver _userRetriver;
        public OrderVMManager(IUnitOfWork unitOfWork, IOrderHeaderManager headerManager, IUserRetriver userRetriver)
        {
            _unitOfWork = unitOfWork;
            _headerManager = headerManager;
            _userRetriver = userRetriver;
        }

        public async Task<OrderVM> CreateVmForNewOrderAsync()
        {
            var currentApplicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == _userRetriver.GetCurrentUserId());
            if (currentApplicationUser == null)
            {
                throw new InvalidOperationException("User not logged in");
            }

            var vm = new OrderVM();
            vm.OrderHeader = new OrderHeader();
            _headerManager.AddUserDataToOrderHeader(vm.OrderHeader, currentApplicationUser);

            var userCart = await _unitOfWork.Cart.GetAsync(c => c.UserId == currentApplicationUser.Id, includeProperties: "Items,Items.Product");
            if (userCart != null)
            {
                vm.OrderDetailsAsCartItems = userCart.Items;
            }

            var webpageConfig = await _unitOfWork.WebshopConfig.GetAsync();
            if (webpageConfig != null)
            {
                vm.Currency = webpageConfig.Currency;
            }

            var carriers = await _unitOfWork.Carrier.GetAllAsync();
            if (carriers != null)
            {
                vm.CarrierListItems = carriers.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            return vm;
        }

        public async Task<OrderVM> GetVmByIdAsync(int orderHeaderId) 
        {
            var vm = new OrderVM();
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
               h => h.Id == orderHeaderId,
               includeProperties: "OrderDetails,ApplicationUser,Carrier");
            var webshopConfig = await _unitOfWork.WebshopConfig.GetAsync();
            var currency = webshopConfig.Currency;
            if (orderHeader != null) 
            {
                vm.OrderHeader = orderHeader;
                vm.Currency = currency;
            }

            return vm;
        }
    }
}
