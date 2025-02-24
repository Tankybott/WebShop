using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Services.CartServices.Interfaces;

namespace Services.CartServices
{
    public class CartItemsQuantityRetriver: ICartItemsQuantityRetriver
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartItemsQuantityRetriver(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetItemsQantityAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }

            var cart = await _unitOfWork.Cart.GetAsync(c => c.UserId == userId, includeProperties: "Items");

            if (cart != null)
            {
                if (cart.Items == null || cart.Items.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return cart.Items.Count;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
