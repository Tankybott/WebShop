using System.Security.Claims;

namespace Services.CartServices.Interfaces
{
    public interface ICartDeletionQueueManager
    {
        Task DequeueUsersCart(ClaimsPrincipal user);
        Task EnqueueUsersCart(ClaimsPrincipal user);
    }
}