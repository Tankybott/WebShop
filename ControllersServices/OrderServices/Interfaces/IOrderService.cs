using Models.DatabaseRelatedModels;
using Models.DTOs;
using Models.FormModel;
using Models.ViewModels;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderService
    {
        Task<OrderVM> GetVmForNewOrderAsync();
        Task<string> CreateOrderAsync(OrderFormModel formModel);
        Task<bool> ProcessSucessPayementAsync(int orderHeaderId);
        Task<IEnumerable<OrderDTO>> GetOrderTableDTOEntitiesAsync();
        Task<OrderVM> GetOrderVMByIdAsync(int orderHeaderId);
        Task StartProcessingAsync(int orderHeaderId);
        Task UpdateOrderHeaderAsync(OrderHeader orderHeader);
        Task SetOrderSentAsync(int orderHeaderId);
    }
}