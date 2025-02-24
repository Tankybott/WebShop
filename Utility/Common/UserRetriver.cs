using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Utility.Common.Interfaces;

namespace Utility.Common
{
    public class UserRetriver : IUserRetriver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRetriver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null || httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
                throw new InvalidOperationException("User is not logged in.");

            return httpContext.User;
        }

        public string GetCurrentUserId()
        {
            var user = GetCurrentUser();
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; 

            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("User ID is missing in claims.");

            return userId;
        }
    }
}
