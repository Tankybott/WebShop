using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Models.DTOs;
using Services.CartServices;
using Services.CartServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartItemQuantityValidatorTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICartItemQuantityUpdater> _mockCartItemQuantityUpdater;
        private Mock<ICartItemRepository> _mockCartItemRepository;
        private ICartItemQuantityValidator _cartItemQuantityValidator;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCartItemQuantityUpdater = new Mock<ICartItemQuantityUpdater>();
            _mockCartItemRepository = new Mock<ICartItemRepository>();

            _mockUnitOfWork.Setup(u => u.CartItem).Returns(_mockCartItemRepository.Object);

            _cartItemQuantityValidator = new CartItemQuantityValidator(
                _mockUnitOfWork.Object,
                _mockCartItemQuantityUpdater.Object
            );
        }

        private void SetupMockForGetAllCartItemsAsync(List<CartItem>? cartItems)
        {
            _mockCartItemRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<CartItem, bool>>>(), It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<Expression<Func<CartItem, object>>>()))
                .ReturnsAsync(cartItems);
        }

        [Test]
        public async Task ValidateItemsQuantity_ShouldUpdateQuantity_WhenRequestedExceedsStock()
        {
            // Arrange
            var cartItem = new CartItem
            {
                Id = 1,
                Product = new Product { StockQuantity = 5 },
                ProductQuantity = 3
            };

            var cartItemDto = new CartItemQuantityDTO { CartItemId = 1, Quantity = 10 }; // Requested more than stock

            SetupMockForGetAllCartItemsAsync(new List<CartItem> { cartItem });

            // Act
            var result = await _cartItemQuantityValidator.ValidateItemsQuantity(new List<CartItemQuantityDTO> { cartItemDto });

            // Assert
            Assert.That(result.First().Quantity, Is.EqualTo(cartItem.Product.StockQuantity));
            _mockCartItemQuantityUpdater.Verify(u => u.UpdateQantityAsync(cartItem, cartItem.Product.StockQuantity), Times.Once);
        }

        [Test]
        public async Task ValidateItemsQuantity_ShouldSetQuantityToZero_WhenProductIsNull()
        {
            // Arrange
            var cartItem = new CartItem
            {
                Id = 1,
                Product = null, // Product no longer exists
                ProductQuantity = 3
            };

            var cartItemDto = new CartItemQuantityDTO { CartItemId = 1, Quantity = 3 };

            SetupMockForGetAllCartItemsAsync(new List<CartItem> { cartItem });

            // Act
            var result = await _cartItemQuantityValidator.ValidateItemsQuantity(new List<CartItemQuantityDTO> { cartItemDto });

            // Assert
            Assert.That(result.First().Quantity, Is.EqualTo(0));
            _mockCartItemQuantityUpdater.Verify(u => u.UpdateQantityAsync(cartItem, 0), Times.Once);
        }

        [Test]
        public async Task ValidateItemsQuantity_ShouldNotChangeQuantity_WhenRequestedIsWithinStock()
        {
            // Arrange
            var cartItem = new CartItem
            {
                Id = 1,
                Product = new Product { StockQuantity = 10 },
                ProductQuantity = 5
            };

            var cartItemDto = new CartItemQuantityDTO { CartItemId = 1, Quantity = 5 };

            SetupMockForGetAllCartItemsAsync(new List<CartItem> { cartItem });

            // Act
            var result = await _cartItemQuantityValidator.ValidateItemsQuantity(new List<CartItemQuantityDTO> { cartItemDto });

            // Assert
            Assert.That(result.First().Quantity, Is.EqualTo(cartItemDto.Quantity));
            _mockCartItemQuantityUpdater.Verify(u => u.UpdateQantityAsync(It.IsAny<CartItem>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void ValidateItemsQuantity_ShouldThrowArgumentException_WhenCartItemDoesNotExist()
        {
            // Arrange
            var cartItemDto = new CartItemQuantityDTO { CartItemId = 1, Quantity = 3 };

            SetupMockForGetAllCartItemsAsync(null); // No cart items returned

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _cartItemQuantityValidator.ValidateItemsQuantity(new List<CartItemQuantityDTO> { cartItemDto }));
            Assert.That(ex.Message, Is.EqualTo("At least one of cart items passed to validate is not existing"));
        }
    }
}
