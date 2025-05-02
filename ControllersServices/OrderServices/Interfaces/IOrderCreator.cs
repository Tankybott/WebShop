using Models.FormModel;

namespace Services.OrderServices.Interfaces
{
    public interface IOrderCreator
    {
        Task<string> CreateAsync(OrderFormModel formModel);
    }
}