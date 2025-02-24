using Models.FormModel;

namespace Services.CartServices.Interfaces
{
    public interface ICartItemAdder
    {
        Task AddItemAsync(CartItemFormModel formModel);
    }
}