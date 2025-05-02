using Models.DatabaseRelatedModels;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderStockReducer
    {
        Task ReduceStockByOrderDetailsAsync(IEnumerable<OrderDetail> details);
    }
}