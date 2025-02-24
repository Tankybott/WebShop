
using DataAccess.Repository.IRepository;
using Models;
using Models.DTOs;
using Services.CartServices.Interfaces;

namespace Services.CartServices
{
    public class CartItemQuantityValidator : ICartItemQuantityValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartItemQuantityUpdater _cartItemQuantityUpdater;

        public CartItemQuantityValidator(IUnitOfWork unitOfWork, ICartItemQuantityUpdater cartItemQuantityUpdater)
        {
            _unitOfWork = unitOfWork;
            _cartItemQuantityUpdater = cartItemQuantityUpdater;
        }

        public async Task<IEnumerable<CartItemQuantityDTO>> ValidateItemsQuantity(IEnumerable<CartItemQuantityDTO> DTOSToCheck)
        {
            var processedDTOs = DTOSToCheck.ToList();
            var cartItemIds = DTOSToCheck.Select(d => d.CartItemId).ToList();
            var cartItemsToCheck = await _unitOfWork.CartItem.GetAllAsync(i => cartItemIds.Contains(i.Id), includeProperties: "Product");
            int indexer = 0;
            foreach (var Dto in DTOSToCheck)
            {
                await ValidateItemQuantity(cartItemsToCheck, Dto.CartItemId, processedDTOs[indexer]);
            } 
            return processedDTOs;
        }

        private async  Task<CartItemQuantityDTO> ValidateItemQuantity(IEnumerable<CartItem> cartItemsToCheck, int cartItemId, CartItemQuantityDTO dtoToValidate) 
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
            return dtoToValidate;
        }
    }
}
