using Models.DTOs;

namespace Services.CartServices.Interfaces
{
    public interface ICartItemQuantitySyncService
    {
        Task<IEnumerable<CartItemQuantityDTO>> AdjustOvercountedItemsQuantityAsync(IEnumerable<CartItemQuantityDTO> DTOSToCheck);
    }
}