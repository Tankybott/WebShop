

using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models;
using Models.DatabaseRelatedModels;
using Models.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;
using Utility.Common.Interfaces;
using Utility.Constants;

namespace Services.OrderServices
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRetriver _userRetriver;

        public OrderService(IUnitOfWork unitOfWork, IUserRetriver userRetriver)
        {
            _unitOfWork = unitOfWork;
            _userRetriver = userRetriver;
        }

        public async Task CreateOrderVM(IEnumerable<OrderDetail> OrderDetails)
        {
            var currentApplicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == _userRetriver.GetCurrentUserId());
            if (currentApplicationUser != null) 
            {
                var orderHeader = new OrderHeader
                {
                    ApplicationUserId = currentApplicationUser.Id,
                    OrderStatus = OrderStatuses.CreatedStatus,
                    Name = currentApplicationUser.Name,
                    PhoneNumber = currentApplicationUser.PhoneNumber,
                    StreetAdress = currentApplicationUser.StreetAdress,
                    City = currentApplicationUser.City,
                    Region = currentApplicationUser.Region,
                    PostalCode = currentApplicationUser.PostalCode,
                    Country = currentApplicationUser.Country
                };
                var vm = new OrderVM
                {
                    OrderDetails = OrderDetails,
                    OrderHeader = orderHeader,
                };
            }
        }

        //public async Task<OrderHeader> CreateOrderHeader()
        //{   
        //    var user = _unitOfWork.
        //    var orderHeader = new OrderHeader
        //    {
        //        ApplicationUserId = _userRetriver.GetCurrentUserId(),

        //    };
        //}
    }
}



//public DateTime OrderDate { get; set; }
//public DateTime ShippingDate { get; set; }
//public double OrderTotal { get; set; }

//public string? OrderStatus { get; set; }
//public string? PaymentStatus { get; set; }
//public string? TrackingNumber { get; set; }
//public string? TrackingLink { get; set; }
//public string? Carrier { get; set; }


//public DateTime PaymentDate { get; set; }

//public string? SessionId { get; set; }
//public string? PaymentIntentId { get; set; }

//[Required]
//public string Name { get; set; }
//[Required]
//public string PhoneNumber { get; set; }
//[Required]
//public string StreetAdress { get; set; }
//[Required]
//public string City { get; set; }
//[Required]
//public string Region { get; set; }
//[Required]
//public string PostalCode { get; set; }
//[Required]
//public string? Country { get; set; }
//[ValidateNever]
//public IEnumerable<OrderDetail> OrderDetails { get; set; }