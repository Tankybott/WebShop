using DataAccess.Repository.IRepository;
using Models;
using Models.DTOs;
using Services.CartServices.Interfaces;

namespace Services.CartServices
{
    public class CartItemQuantitySyncService : ICartItemQuantitySyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartItemQuantityUpdater _cartItemQuantityUpdater;

        public CartItemQuantitySyncService(IUnitOfWork unitOfWork, ICartItemQuantityUpdater cartItemQuantityUpdater)
        {
            _unitOfWork = unitOfWork;
            _cartItemQuantityUpdater = cartItemQuantityUpdater;
        }

        /// <summary>
        /// Ensures that cart item quantities do not exceed available product stock.
        /// If a cart item's desired quantity is greater than the product's stock, it updates the quantity
        /// both in the database and in the returned DTOs.
        /// 
        /// The returned DTOs allow the frontend to highlight which items had their quantity adjusted.
        /// </summary>
        /// <param name="DTOSToCheck">A collection of DTOs representing cart items with desired quantities.</param>
        /// <returns>
        /// A collection of DTOs containing the cart item ID and the enforced quantity, 
        /// reflecting any necessary adjustments due to stock limitations.
        /// </returns>
        public async Task<IEnumerable<CartItemQuantityDTO>> AdjustOvercountedItemsQuantityAsync(IEnumerable<CartItemQuantityDTO> DTOSToCheck)
        {
            var processedDTOs = DTOSToCheck.ToList();
            var cartItemIds = DTOSToCheck.Select(d => d.CartItemId).ToList();
            var cartItemsToCheck = await _unitOfWork.CartItem.GetAllAsync(i => cartItemIds.Contains(i.Id), includeProperties: "Product");
            int indexer = 0;
            foreach (var Dto in DTOSToCheck)
            {
                await AdjustOvercountedItemQuantityAsync(cartItemsToCheck, Dto.CartItemId, processedDTOs[indexer]);
            }
            return processedDTOs;
        }

        private async Task AdjustOvercountedItemQuantityAsync(IEnumerable<CartItem> cartItemsToCheck, int cartItemId, CartItemQuantityDTO dtoToValidate)
        {
            var cartItemToCheck = cartItemsToCheck?.FirstOrDefault(c => c.Id == cartItemId);
            if (cartItemToCheck != null)
            {
                if (cartItemToCheck.Product == null)
                {
                    await _cartItemQuantityUpdater.UpdateQantityAsync(cartItemToCheck, 0);
                    dtoToValidate.Quantity = 0;
                }
                else if (dtoToValidate.Quantity > cartItemToCheck.Product.StockQuantity)
                {
                    await _cartItemQuantityUpdater.UpdateQantityAsync(cartItemToCheck, cartItemToCheck.Product.StockQuantity);
                    dtoToValidate.Quantity = cartItemToCheck.Product.StockQuantity;
                }
            }
            else
            {
                throw new ArgumentException("At least one of cart items passed to validate is not existing");
            }
        }
    }
}
