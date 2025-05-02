using DataAccess.Repository.IRepository;
using Models;
using Models.DatabaseRelatedModels;
using Services.OrderServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OrderServices
{
    public class OrderStockReducer : IOrderStockReducer
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderStockReducer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ReduceStockByOrderDetailsAsync(IEnumerable<OrderDetail> details)
        {
            List<Product> productsToUpdate = new List<Product>();
            foreach (var detail in details)
            {
                var product = await _unitOfWork.Product.GetAsync(p => p.Id == detail.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= detail.Quantity;
                    productsToUpdate.Add(product);
                }
            }
            _unitOfWork.Product.UpdateRange(productsToUpdate);
            await _unitOfWork.SaveAsync();
        }
    }
}
