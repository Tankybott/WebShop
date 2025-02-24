using Models;
using Models.FormModel;

namespace Services.CartServices.Interfaces
{
    public interface ICartItemCreator
    {
        Task<CartItem> CreateCartItemAsync(CartItemFormModel formModel);
    }
}