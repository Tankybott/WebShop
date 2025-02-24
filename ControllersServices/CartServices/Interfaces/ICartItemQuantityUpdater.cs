
using Models;

namespace Services.CartServices.Interfaces
{
    public interface ICartItemQuantityUpdater
    {
        Task UpdateQantityAsync(int cartItemId, int newQuantity);
        Task UpdateQantityAsync(CartItem cartItem, int newQuantity);
    }
}