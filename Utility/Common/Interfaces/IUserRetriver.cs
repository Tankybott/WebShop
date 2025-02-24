using System.Security.Claims;

namespace Utility.Common.Interfaces
{
    public interface IUserRetriver
    {
        ClaimsPrincipal GetCurrentUser();

        string GetCurrentUserId();
    }
}