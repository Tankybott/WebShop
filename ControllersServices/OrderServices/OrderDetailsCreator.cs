using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Services.OrderServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OrderServices
{
    public class OrderDetailsCreator : IOrderDetailsCreator
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailsCreator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateDetailsAsync(int cartId, int orderHeaderId)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            var cart = await _unitOfWork.Cart.GetAsync(c => c.Id == cartId, includeProperties: "Items,Items.Product");
            if (cart != null)
            {
                foreach (var item in cart.Items)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductName = item.Product.Name,
                        ProductId = item.Product.Id,
                        Price = item.CurrentPrice,
                        Quantity = item.ProductQuantity,
                        OrderHeaderId = orderHeaderId,
                        ShippingPriceFactor = item.Product.ShippingPriceFactor
                    };
                    if(orderDetail.Quantity > 0) orderDetails.Add(orderDetail);
                }
            }
            else
            {
                throw new ArgumentNullException("cart not fount");
            };

            if (orderDetails.Count() <= 0) throw new InvalidOperationException("Cannot create order with empty order details");

            _unitOfWork.OrderDetail.AddRange(orderDetails);
            await _unitOfWork.SaveAsync();
            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.SaveAsync();
            // double save to remove cart only when order were sucesfully created 
        }
    }
}
