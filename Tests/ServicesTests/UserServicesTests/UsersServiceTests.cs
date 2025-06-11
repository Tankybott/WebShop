using Moq;
using NUnit.Framework;
using Services.UsersServices;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.DTOs;
using System.Linq.Expressions;

namespace ServicesTests.UsersServices
{
    [TestFixture]
    public class UsersServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserStore<ApplicationUser>> _mockUserStore;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IApplicationUserRepository> _mockAppUserRepo;
        private UsersService _usersService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAppUserRepo = new Mock<IApplicationUserRepository>();

            _mockUnitOfWork.Setup(u => u.ApplicationUser).Returns(_mockAppUserRepo.Object);

            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                _mockUserStore.Object, null, null, null, null, null, null, null, null);

            _usersService = new UsersService(_mockUnitOfWork.Object, _mockUserManager.Object);
        }

        #region GetUsersTableDtoAsync

        [Test]
        public async Task GetUsersTableDtoAsync_ShouldReturnCorrectEmail_WhenUserExists()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            SetupMockAppUserRepoWithUsers(new[] { user });
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
            _mockUserManager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false);

            var result = await _usersService.GetUsersTableDtoAsync();

            Assert.That(result.First().Email, Is.EqualTo("test@example.com"));
        }

        [Test]
        public async Task GetUsersTableDtoAsync_ShouldReturnCorrectRole_WhenUserHasRole()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            SetupMockAppUserRepoWithUsers(new[] { user });
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _mockUserManager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false);

            var result = await _usersService.GetUsersTableDtoAsync();

            Assert.That(result.First().Role, Is.EqualTo("User"));
        }

        [Test]
        public async Task GetUsersTableDtoAsync_ShouldReturnNoRole_WhenUserHasNoRoles()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            SetupMockAppUserRepoWithUsers(new[] { user });
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            _mockUserManager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false);

            var result = await _usersService.GetUsersTableDtoAsync();

            Assert.That(result.First().Role, Is.EqualTo("NoRole"));
        }

        [Test]
        public async Task GetUsersTableDtoAsync_ShouldReturnTrue_WhenUserIsLockedOut()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            SetupMockAppUserRepoWithUsers(new[] { user });
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());
            _mockUserManager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(true);

            var result = await _usersService.GetUsersTableDtoAsync();

            Assert.That(result.First().isUserBanned, Is.True);
        }

        #endregion

        #region ToggleBanUserAsync

        [Test]
        public async Task ToggleBanUserAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            _mockUserManager.Setup(m => m.FindByEmailAsync("test@example.com")).ReturnsAsync((ApplicationUser?)null);

            var result = await _usersService.ToggleBanUserAsync("test@example.com");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ToggleBanUserAsync_ShouldUnbanUser_WhenUserIsLocked()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _mockUserManager.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _usersService.ToggleBanUserAsync(user.Email);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ToggleBanUserAsync_ShouldBanUser_WhenUserIsNotLocked()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _mockUserManager.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsLockedOutAsync(user)).ReturnsAsync(false);
            _mockUserManager.Setup(m => m.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _usersService.ToggleBanUserAsync(user.Email);

            Assert.That(result, Is.True);
        }

        #endregion

        #region Helpers

        private void SetupMockAppUserRepoWithUsers(IEnumerable<ApplicationUser> users)
        {
            _mockAppUserRepo
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Expression<Func<ApplicationUser, object>>>()))
                .ReturnsAsync(users);
        }

        #endregion
    }
}
