using Models;
using Models.DatabaseRelatedModels;
using Models.FormModel;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderHeaderManager
    {
        void AddUserDataToOrderHeader(OrderHeader orderHeader, ApplicationUser currentApplicationUser);
        Task<OrderHeader> CreateAsync(OrderFormModel formModel);
    }
}