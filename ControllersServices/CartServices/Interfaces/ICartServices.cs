using Models;
using Models.DTOs;
using Models.FormModel;

namespace Services.CartServices.Interfaces
{
    public interface ICartServices
    {
        Task AddItemToCartAsync(CartItemFormModel formModel);
        Task RemoveCartItemAsync(int cartItemId);
        Task UpdateCartItemQantityAsync(int cartItemId, int newQuantity);
        Task<IEnumerable<int>> SynchronizeCartPrices(int cartId);
        Task<int> GetCartItemsQantityAsync();
        Task<IEnumerable<CartItemQuantityDTO>> ValidateCartProductsQuantityAsync(IEnumerable<CartItemQuantityDTO> DTOSToCheck);

    }
}