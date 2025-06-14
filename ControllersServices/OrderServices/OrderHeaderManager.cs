using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Models;
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
            orderHeader.CreationDate = DateTime.UtcNow;
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

        public async Task UpdateAsync(OrderHeader orderHeader) 
        {
            var orderHeaderToUpdate = await _unitOfWork.OrderHeader.GetAsync(o => o.Id == orderHeader.Id);
            if (orderHeaderToUpdate != null) 
            {
                orderHeaderToUpdate.Name = orderHeader.Name;
                orderHeaderToUpdate.PhoneNumber = orderHeader.PhoneNumber;
                orderHeaderToUpdate.StreetAdress = orderHeader.StreetAdress;
                orderHeaderToUpdate.City = orderHeader.City;
                orderHeaderToUpdate.Region = orderHeader.Region;
                orderHeaderToUpdate.PostalCode = orderHeader.PostalCode;
                orderHeaderToUpdate.TrackingLink = orderHeader.TrackingLink;
                orderHeaderToUpdate.ShippingDate = orderHeader.ShippingDate;

                _unitOfWork.OrderHeader.Update(orderHeaderToUpdate);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
