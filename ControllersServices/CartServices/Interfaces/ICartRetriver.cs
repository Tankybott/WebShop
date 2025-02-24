using Models;

namespace Services.CartServices.Interfaces
{
    public interface ICartRetriver
    {
        Task<Cart> RetriveUserCartAsync();
    }
}