using Models.DTOs;

namespace Services.CartServices.Interfaces
{
    public interface ICartItemQuantityValidator
    {
        Task<IEnumerable<CartItemQuantityDTO>> ValidateItemsQuantity(IEnumerable<CartItemQuantityDTO> DTOSToCheck);
    }
}