using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.CartServices.Interfaces;
using System.Security.Claims;
using Utility.Queues.Interfaces;

namespace Services.CartServices
{
    public class CartDeletionQueueManager : ICartDeletionQueueManager
    {
        private readonly ICartDeletionQueue _cartDeletionQueue;
        private readonly IUnitOfWork _unitOfWork;


        public CartDeletionQueueManager(ICartDeletionQueue cartDeletionQueue, IUnitOfWork unitOfWork)
        {
            _cartDeletionQueue = cartDeletionQueue;
            _unitOfWork = unitOfWork;
        }

        public async Task EnqueueUsersCart(ClaimsPrincipal user)
        {
            var cartToEnqueue = await getUsersCart(user);

            if (cartToEnqueue != null) 
            {
                cartToEnqueue.ExpiresTo = DateTime.Now.AddHours(2);
                _unitOfWork.Cart.Update(cartToEnqueue);
                await _unitOfWork.SaveAsync();
                await _cartDeletionQueue.EnqueueAsync(cartToEnqueue);
            }

        }

        public async Task DequeueUsersCart(ClaimsPrincipal user)
        {
            var cartToDequeue = await getUsersCart(user);
            if (cartToDequeue != null)
            {
                cartToDequeue.ExpiresTo = null;
                _unitOfWork.Cart.Update(cartToDequeue);
                await _unitOfWork.SaveAsync();

                await _cartDeletionQueue.RemoveByIdAsync(cartToDequeue.Id);
            }
        }

        private async Task<Cart?> getUsersCart(ClaimsPrincipal user)
        {
            string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }
            var existingCart = await _unitOfWork.Cart.GetAsync(c => c.UserId == userId, tracked: false);
            return existingCart;
        }
    }
}
