using Models.DTOs;

namespace Services.UsersServices
{
    public interface IUsersService
    {
        Task<IEnumerable<UsersTableDTO>> GetUsersTableDtoAsync();
        Task<bool> ToggleBanUserAsync(string email);
    }
}