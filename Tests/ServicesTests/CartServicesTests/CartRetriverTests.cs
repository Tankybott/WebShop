using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Services.CartServices;
using Services.CartServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartRetriverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ICartRepository> _mockCartRepository;
        private ICartRetriver _cartRetriver;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockCartRepository = new Mock<ICartRepository>();

            _mockUnitOfWork.Setup(u => u.Cart).Returns(_mockCartRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.Cart.Add(It.IsAny<Cart>()));

            _cartRetriver = new CartRetriver(
                _mockHttpContextAccessor.Object,
                _mockUnitOfWork.Object
            );
        }

        private void SetupMockForGetCartAsync(Cart? cart)
        {
            _mockCartRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .ReturnsAsync(cart);
        }

        private void SetupMockForUser(string? userId)
        {
            var claims = userId != null
                ? new[] { new Claim(ClaimTypes.NameIdentifier, userId) }
                : Array.Empty<Claim>();

            var mockHttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims))
            };

            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext);
        }

        [Test]
        public async Task RetriveUserCartAsync_ShouldReturnExistingCart_WhenCartExists()
        {
            // Arrange
            SetupMockForUser("user123");
            var existingCart = new Cart { Id = 1, UserId = "user123", Items = new List<CartItem>() };
            SetupMockForGetCartAsync(existingCart);

            // Act
            var result = await _cartRetriver.RetriveUserCartAsync();

            // Assert
            Assert.That(result, Is.EqualTo(existingCart));
            _mockUnitOfWork.Verify(u => u.Cart.Add(It.IsAny<Cart>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task RetriveUserCartAsync_ShouldCreateNewCart_WhenCartDoesNotExist()
        {
            // Arrange
            SetupMockForUser("user123");
            SetupMockForGetCartAsync(null); // No existing cart

            // Act
            var result = await _cartRetriver.RetriveUserCartAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo("user123"));
            _mockUnitOfWork.Verify(u => u.Cart.Add(It.IsAny<Cart>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public void RetriveUserCartAsync_ShouldThrowException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            SetupMockForUser(null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _cartRetriver.RetriveUserCartAsync());
        }

    }
}
