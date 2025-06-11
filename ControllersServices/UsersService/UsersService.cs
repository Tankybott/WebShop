using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.DTOs;


namespace Services.UsersServices
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UsersTableDTO>> GetUsersTableDtoAsync()
        {
            var users = await _unitOfWork.ApplicationUser.GetAllAsync();
            var dtos = new List<UsersTableDTO>();


            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "NoRole";
                var isLockedOut = await _userManager.IsLockedOutAsync(user);

                dtos.Add(new UsersTableDTO
                {
                    Email = user.Email,
                    Role = role,
                    isUserBanned = isLockedOut
                });
            }

            return dtos;
        }

        public async Task<bool> ToggleBanUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var isLockedOut = await _userManager.IsLockedOutAsync(user);

            var result = isLockedOut
                ? await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow) // unban
                : await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); // ban

            return result.Succeeded;
        }
    }
}