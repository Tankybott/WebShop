using DataAccess.Repository.IRepository;
using Models.DTOs;
using Models.FormModel;
using Services.CartServices.Interfaces;

namespace Services.CartServices
{
    public class CartServices : ICartServices
    {

        private readonly ICartPriceSynchronizer _cartPriceSynchronizer;
        private readonly ICartItemsQuantityRetriver _cartItemsQuantityRetriver;
        private readonly ICartItemRemover _cartItemRemover;
        private readonly ICartItemAdder _cartItemAdder;
        private readonly ICartItemQuantityUpdater _cartItemQuantityUpdater;
        private readonly ICartItemQuantitySyncService _cartItemQuantityValidator;

        public CartServices(
            ICartPriceSynchronizer cartPriceSynchronizer,
            ICartItemsQuantityRetriver cartItemsQuantityRetriver,
            ICartItemRemover cartItemRemover,
            ICartItemAdder cartItemAdder,
            ICartItemQuantityUpdater cartItemQuantityUpdater,
            ICartItemQuantitySyncService cartItemQuantityValidator
            )
        {
            _cartPriceSynchronizer = cartPriceSynchronizer;
            _cartItemsQuantityRetriver = cartItemsQuantityRetriver;
            _cartItemRemover = cartItemRemover;
            _cartItemAdder = cartItemAdder;
            _cartItemQuantityUpdater = cartItemQuantityUpdater;
            _cartItemQuantityValidator = cartItemQuantityValidator;
        }

        public async Task AddItemToCartAsync(CartItemFormModel formModel)
        {
           await _cartItemAdder.AddItemAsync(formModel);
        }

        public async Task RemoveCartItemAsync(int cartItemId) 
        {
            await _cartItemRemover.RemoveAsync(cartItemId);
        }

        public async Task UpdateCartItemQantityAsync(int cartItemId, int newQuantity) 
        {
            await _cartItemQuantityUpdater.UpdateQantityAsync(cartItemId, newQuantity);
        }

        public async Task<IEnumerable<CartItemQuantityDTO>> ValidateCartProductsQuantityAsync(IEnumerable<CartItemQuantityDTO> DTOSToCheck)
        {
            return await _cartItemQuantityValidator.AdjustOvercountedItemsQuantityAsync(DTOSToCheck);
        }

        public async Task<IEnumerable<int>> SynchronizeCartPrices(int cartId) 
        {
            return await _cartPriceSynchronizer.Synchronize(cartId);
        }

        public async Task<int> GetCartItemsQantityAsync() 
        {
            return await _cartItemsQuantityRetriver.GetItemsQantityAsync();
        }

    }
}
