using DataAccess.Repository.IRepository;
using Models;
using Services.CartServices.Interfaces;
using Services.ProductService.Interfaces;

namespace Services.CartServices
{
    public class CartPriceSynchronizer : ICartPriceSynchronizer
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductPriceRetriver _productPriceRetriver;

        public CartPriceSynchronizer(IUnitOfWork unitOfWork, IProductPriceRetriver productPriceRetriver)
        {
            _unitOfWork = unitOfWork;
            _productPriceRetriver = productPriceRetriver;
        }

        /// <summary>
        /// Synchronizes the prices of all cart items associated with the given cart ID.
        /// If any item's price has changed, it updates the item's price and saves the cart.
        /// </summary>
        /// <param name="cartId">The ID of the cart whose items need to be synchronized.</param>
        /// <returns>
        /// A task that returns a list of cart item IDs that had their prices updated during the synchronization process.
        /// </returns>
        public async Task<IEnumerable<int>> Synchronize(int cartId)
        {
            var cart = await _unitOfWork.Cart.GetAsync(c => c.Id == cartId, includeProperties: "Items,Items.Product");
            var synchronizedCartItemsIds = new List<int>();
            if (cart != null)
            {
                bool wasAnyItemPriceChanged = false;
                foreach (var item in cart.Items)
                {
                    var actualProductPrice = await _productPriceRetriver.GetProductPriceAsync(item.ProductId);
                    if (item.CurrentPrice != actualProductPrice)
                    {
                        item.CurrentPrice = actualProductPrice;
                        wasAnyItemPriceChanged = true;
                        synchronizedCartItemsIds.Add(item.Id);
                    }
                }
                if (wasAnyItemPriceChanged)
                {
                    _unitOfWork.Cart.Update(cart);
                    await _unitOfWork.SaveAsync();
                }
            }

            return synchronizedCartItemsIds;
        }
    }
}
