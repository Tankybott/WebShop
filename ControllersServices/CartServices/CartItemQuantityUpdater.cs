
using DataAccess.Repository.IRepository;
using Models;
using Services.CartServices.CustomExeptions;
using Services.CartServices.Interfaces;

namespace Services.CartServices
{
    public class CartItemQuantityUpdater : ICartItemQuantityUpdater
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartItemRemover _cartItemRemover;

        public CartItemQuantityUpdater(IUnitOfWork unitOfWork, ICartItemRemover cartItemRemover)
        {
            _unitOfWork = unitOfWork;
            _cartItemRemover = cartItemRemover;
        }

        public async Task UpdateQantityAsync(int cartItemId, int newQuantity)
        {
            var cartItemToUpdate = await _unitOfWork.CartItem.GetAsync(i => i.Id == cartItemId, includeProperties: "Product");
            if (cartItemToUpdate != null)
            {
                await UpdateQantityAsync(cartItemToUpdate, newQuantity);
            }
            else
            {
                throw new ArgumentException("Cart item doesnt exist in database");
            }
        }

        public async Task UpdateQantityAsync(CartItem cartItem, int newQuantity)
        {
            if (cartItem.Product.StockQuantity < newQuantity) throw new NotEnoughQuantityException(maxAvailableQuantity: cartItem.Product.StockQuantity);
            cartItem.ProductQuantity = newQuantity;
            if (cartItem.ProductQuantity <= 0)
            {
                await _cartItemRemover.RemoveAsync(cartItem.Id);
            }
            else 
            {
                _unitOfWork.CartItem.Update(cartItem);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}