using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Services.CartServices;
using Services.CartServices.Interfaces;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Utility.Queues.Interfaces;
using System.Linq.Expressions;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartDeletionQueueManagerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICartRepository> _mockCartRepository;
        private Mock<ICartDeletionQueue> _mockCartDeletionQueue;
        private CartDeletionQueueManager _cartDeletionQueueManager;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCartRepository = new Mock<ICartRepository>();
            _mockCartDeletionQueue = new Mock<ICartDeletionQueue>();

            _mockUnitOfWork.Setup(u => u.Cart).Returns(_mockCartRepository.Object);

            _cartDeletionQueueManager = new CartDeletionQueueManager(
                _mockCartDeletionQueue.Object,
                _mockUnitOfWork.Object
            );
        }

        private ClaimsPrincipal CreateMockUser(string? userId)
        {
            var claims = userId != null ? new[] { new Claim(ClaimTypes.NameIdentifier, userId) } : Array.Empty<Claim>();
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        private void SetupMockForGetCartAsync(Cart? cart)
        {
            _mockCartRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), null, false))
                .ReturnsAsync(cart);
        }

        [Test]
        public async Task EnqueueUsersCart_ShouldEnqueueCart_WhenUserHasCart()
        {
            // Arrange
            var userId = "user123";
            var user = CreateMockUser(userId);
            var cart = new Cart { Id = 1, UserId = userId, ExpiresTo = null };

            SetupMockForGetCartAsync(cart);

            // Act
            await _cartDeletionQueueManager.EnqueueUsersCart(user);

            // Assert
            Assert.That(cart.ExpiresTo, Is.Not.Null);
            _mockCartRepository.Verify(r => r.Update(cart), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
            _mockCartDeletionQueue.Verify(q => q.EnqueueAsync(cart), Times.Once);
        }

        [Test]
        public async Task EnqueueUsersCart_ShouldNotEnqueue_WhenUserHasNoCart()
        {
            // Arrange
            var user = CreateMockUser("user123");
            SetupMockForGetCartAsync(null);

            // Act
            await _cartDeletionQueueManager.EnqueueUsersCart(user);

            // Assert
            _mockCartRepository.Verify(r => r.Update(It.IsAny<Cart>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
            _mockCartDeletionQueue.Verify(q => q.EnqueueAsync(It.IsAny<Cart>()), Times.Never);
        }

        [Test]
        public async Task DequeueUsersCart_ShouldRemoveCartFromQueue_WhenUserHasCart()
        {
            // Arrange
            var userId = "user123";
            var user = CreateMockUser(userId);
            var cart = new Cart { Id = 1, UserId = userId, ExpiresTo = DateTime.Now.AddHours(2) };

            SetupMockForGetCartAsync(cart);

            // Act
            await _cartDeletionQueueManager.DequeueUsersCart(user);

            // Assert
            Assert.That(cart.ExpiresTo, Is.Null);
            _mockCartRepository.Verify(r => r.Update(cart), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
            _mockCartDeletionQueue.Verify(q => q.RemoveByIdAsync(cart.Id), Times.Once);
        }

        [Test]
        public async Task DequeueUsersCart_ShouldNotRemove_WhenUserHasNoCart()
        {
            // Arrange
            var user = CreateMockUser("user123");
            SetupMockForGetCartAsync(null);

            // Act
            await _cartDeletionQueueManager.DequeueUsersCart(user);

            // Assert
            _mockCartRepository.Verify(r => r.Update(It.IsAny<Cart>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
            _mockCartDeletionQueue.Verify(q => q.RemoveByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void EnqueueUsersCart_ShouldThrowException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var user = CreateMockUser(null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _cartDeletionQueueManager.EnqueueUsersCart(user));
            Assert.That(ex.Message, Is.EqualTo("User is not authenticated."));
        }

        [Test]
        public void DequeueUsersCart_ShouldThrowException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var user = CreateMockUser(null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _cartDeletionQueueManager.DequeueUsersCart(user));
            Assert.That(ex.Message, Is.EqualTo("User is not authenticated."));
        }
    }
}
