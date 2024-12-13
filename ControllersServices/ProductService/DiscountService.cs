using ControllersServices.ProductService.Interfaces;
using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductService
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Discount> CreateDiscountAsync(DateTime startDate, DateTime endDate, int percentage)
        {
            var discount = new Discount
            {
                StartTime = startDate,
                EndTime = endDate,
                Percentage = percentage
            };

            _unitOfWork.Discount.Add(discount);
            await _unitOfWork.SaveAsync();

            return discount;
        }
    }
}
