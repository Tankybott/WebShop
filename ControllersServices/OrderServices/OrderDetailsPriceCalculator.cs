using Models.DatabaseRelatedModels;
using Services.OrderServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OrderServices
{
    public class OrderDetailsPriceCalculator : IOrderDetailsPriceCalculator
    {
        public decimal CalculateDetailsPrice(IEnumerable<OrderDetail> orderDetails, Carrier carrier)
        {
            if (!carrier.IsPricePerKg)
            {
                return carrier.Price;
            }

            decimal totalShipping = orderDetails.Sum(od =>
                od.ShippingPriceFactor * od.Quantity * carrier.Price);

            decimal finalShippingPrice = totalShipping < carrier.MinimalShippingPrice
                ? carrier.MinimalShippingPrice
                : totalShipping;

            return finalShippingPrice;
        }
    }
}
