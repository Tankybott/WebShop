using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Services.CartServices.Interfaces;
using System.Security.Claims;

namespace Services.CartServices
{
    public class CartRetriver : ICartRetriver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        public CartRetriver(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<Cart> RetriveUserCartAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User must be authenticated to retrieve a cart.");
            }

            var userCart = await _unitOfWork.Cart.GetAsync(c => c.UserId == userId, includeProperties: "Items");
            if (userCart == null)
            {
                userCart = new Cart();
                userCart.UserId = userId;
                _unitOfWork.Cart.Add(userCart);
                await _unitOfWork.SaveAsync();
            }

            return userCart;
        }
    }
}
