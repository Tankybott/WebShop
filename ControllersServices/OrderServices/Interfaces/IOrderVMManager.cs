using Models.ViewModels;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderVMManager
    {
        Task<OrderVM> CreateVmForNewOrderAsync();
        Task<OrderVM> GetVmByIdAsync(int orderHeaderId);
    }
}