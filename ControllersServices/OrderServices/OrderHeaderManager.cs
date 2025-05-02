using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.FormModel;
using Utility.Constants;
using Utility.Common.Interfaces;
using Services.OrderServices.Interfaces;

namespace Services.OrderServices
{
    public class OrderHeaderManager : IOrderHeaderManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRetriver _userRetriver;

        public OrderHeaderManager(IUnitOfWork unitOfWork, IUserRetriver userRetriver)
        {
            _unitOfWork = unitOfWork;
            _userRetriver = userRetriver;
        }

        public async Task<OrderHeader> CreateAsync(OrderFormModel formModel)
        {
            var orderHeader = new OrderHeader();
            orderHeader.ApplicationUserId = _userRetriver.GetCurrentUserId();
            orderHeader.CarrierId = formModel.carrierId;
            orderHeader.Name = formModel.Name;
            orderHeader.PhoneNumber = formModel.PhoneNumber;
            orderHeader.City = formModel.City;
            orderHeader.Country = formModel.Country;
            orderHeader.PostalCode = formModel.PostalCode;
            orderHeader.Region = formModel.Region;
            orderHeader.StreetAdress = formModel.StreetAdress;
            orderHeader.CreationDate = DateTime.Now;
            orderHeader.OrderStatus = OrderStatuses.CreatedStatus;
            _unitOfWork.OrderHeader.Add(orderHeader);
            await _unitOfWork.SaveAsync();

            return orderHeader;
        }
        public void AddUserDataToOrderHeader(OrderHeader orderHeader, ApplicationUser currentApplicationUser)
        {
            orderHeader.ApplicationUserId = currentApplicationUser.Id;
            orderHeader.Name = currentApplicationUser.Name;
            orderHeader.PhoneNumber = currentApplicationUser.PhoneNumber;
            orderHeader.StreetAdress = currentApplicationUser.StreetAdress;
            orderHeader.City = currentApplicationUser.City;
            orderHeader.Region = currentApplicationUser.Region;
            orderHeader.PostalCode = currentApplicationUser.PostalCode;
            orderHeader.Country = currentApplicationUser.Country;
        }

    }
}
