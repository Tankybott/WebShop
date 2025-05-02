using Models.DatabaseRelatedModels;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderDetailsPriceCalculator
    {
        decimal CalculateDetailsPrice(IEnumerable<OrderDetail> orderDetails, Carrier carrier);
    }
}