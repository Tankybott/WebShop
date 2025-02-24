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
    public class CartItemsQuantityRetriverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ICartRepository> _mockCartRepository;
        private ICartItemsQuantityRetriver _cartItemsQuantityRetriver;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockCartRepository = new Mock<ICartRepository>();

            _mockUnitOfWork.Setup(u => u.Cart).Returns(_mockCartRepository.Object);

            _cartItemsQuantityRetriver = new CartItemsQuantityRetriver(
                _mockUnitOfWork.Object,
                _mockHttpContextAccessor.Object
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
        public async Task GetItemsQantityAsync_ShouldReturnZero_WhenUserIsNotAuthenticated()
        {
            // Arrange
            SetupMockForUser(null);

            // Act
            var result = await _cartItemsQuantityRetriver.GetItemsQantityAsync();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetItemsQantityAsync_ShouldReturnZero_WhenCartDoesNotExist()
        {
            // Arrange
            SetupMockForUser("user123");
            SetupMockForGetCartAsync(null); // No cart found

            // Act
            var result = await _cartItemsQuantityRetriver.GetItemsQantityAsync();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetItemsQantityAsync_ShouldReturnZero_WhenCartHasNoItems()
        {
            // Arrange
            SetupMockForUser("user123");
            SetupMockForGetCartAsync(new Cart { Id = 1, UserId = "user123", Items = new List<CartItem>() });

            // Act
            var result = await _cartItemsQuantityRetriver.GetItemsQantityAsync();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetItemsQantityAsync_ShouldReturnCorrectItemCount_WhenCartHasItems()
        {
            // Arrange
            SetupMockForUser("user123");
            var cart = new Cart
            {
                Id = 1,
                UserId = "user123",
                Items = new List<CartItem>
                {
                    new CartItem { Id = 1 },
                    new CartItem { Id = 2 }
                }
            };
            SetupMockForGetCartAsync(cart);

            // Act
            var result = await _cartItemsQuantityRetriver.GetItemsQantityAsync();

            // Assert
            Assert.That(result, Is.EqualTo(cart.Items.Count));
        }
    }
}
