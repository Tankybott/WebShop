using DataAccess.Repository.IRepository;
using Models;
using Services.CartServices.Interfaces;

namespace Services.CartServices
{
    public class CartItemRemover : ICartItemRemover
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartItemRemover(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RemoveAsync(int cartItemId)
        {
            var cartToDelete = await _unitOfWork.CartItem.GetAsync(i => i.Id == cartItemId);
            if (cartToDelete != null)
            {
                _unitOfWork.CartItem.Remove(cartToDelete);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new ArgumentException("Cart item doenst exist in database");
            }
        }

        public async Task RemoveAsync(CartItem cartItem)
        {
            _unitOfWork.CartItem.Remove(cartItem);
            await _unitOfWork.SaveAsync();
        }
    }
}
