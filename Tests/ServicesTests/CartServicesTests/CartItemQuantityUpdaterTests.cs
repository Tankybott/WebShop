using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Services.CartServices;
using Services.CartServices.CustomExeptions;
using Services.CartServices.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartItemQuantityUpdaterTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICartItemRemover> _mockCartItemRemover;
        private Mock<ICartItemRepository> _mockCartItemRepository;
        private ICartItemQuantityUpdater _cartItemQuantityUpdater;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCartItemRemover = new Mock<ICartItemRemover>();
            _mockCartItemRepository = new Mock<ICartItemRepository>();

            _mockUnitOfWork.Setup(u => u.CartItem).Returns(_mockCartItemRepository.Object);
            _mockUnitOfWork.Setup(u => u.CartItem.Update(It.IsAny<CartItem>()));
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _cartItemQuantityUpdater = new CartItemQuantityUpdater(
                _mockUnitOfWork.Object,
                _mockCartItemRemover.Object
            );
        }

        private void SetupMockForGetCartItemAsync(CartItem? cartItem)
        {
            _mockCartItemRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<CartItem, bool>>>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .ReturnsAsync(cartItem);
        }

        [Test]
        public async Task UpdateQantityAsync_ShouldUpdateQuantity_WhenValidRequest()
        {
            // Arrange
            var cartItem = new CartItem
            {
                Id = 1,
                Product = new Product { StockQuantity = 10 },
                ProductQuantity = 2
            };
            var newQuantity = 5;

            SetupMockForGetCartItemAsync(cartItem);

            // Act
            await _cartItemQuantityUpdater.UpdateQantityAsync(cartItem.Id, newQuantity);

            // Assert
            Assert.That(cartItem.ProductQuantity, Is.EqualTo(newQuantity));
            _mockUnitOfWork.Verify(u => u.CartItem.Update(cartItem), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public void UpdateQantityAsync_ShouldThrowArgumentException_WhenCartItemDoesNotExist()
        {
            // Arrange
            SetupMockForGetCartItemAsync(null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _cartItemQuantityUpdater.UpdateQantityAsync(999, 2));
            Assert.That(ex.Message, Is.EqualTo("Cart item doesnt exist in database"));
        }

        [Test]
        public void UpdateQantityAsync_ShouldThrowNotEnoughQuantityException_WhenStockIsTooLow()
        {
            // Arrange
            var cartItem = new CartItem
            {
                Id = 1,
                Product = new Product { StockQuantity = 3 },
                ProductQuantity = 2
            };
            var newQuantity = 5; // More than available stock

            SetupMockForGetCartItemAsync(cartItem);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotEnoughQuantityException>(() => _cartItemQuantityUpdater.UpdateQantityAsync(cartItem.Id, newQuantity));
            Assert.That(ex.MaxAvailableQuantity, Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateQantityAsync_ShouldRemoveCartItem_WhenQuantityIsZero()
        {
            // Arrange
            var cartItem = new CartItem
            {
                Id = 1,
                Product = new Product { StockQuantity = 10 },
                ProductQuantity = 2
            };
            var newQuantity = 0;

            SetupMockForGetCartItemAsync(cartItem);

            // Act
            await _cartItemQuantityUpdater.UpdateQantityAsync(cartItem.Id, newQuantity);

            // Assert
            _mockCartItemRemover.Verify(r => r.RemoveAsync(cartItem.Id), Times.Once);
            _mockUnitOfWork.Verify(u => u.CartItem.Update(cartItem), Times.Never); // Should not update, should remove instead
        }

        [Test]
        public async Task UpdateQantityAsync_ShouldRemoveCartItem_WhenQuantityIsNegative()
        {
            // Arrange
            var cartItem = new CartItem
            {
                Id = 1,
                Product = new Product { StockQuantity = 10 },
                ProductQuantity = 2
            };
            var newQuantity = -2;

            SetupMockForGetCartItemAsync(cartItem);

            // Act
            await _cartItemQuantityUpdater.UpdateQantityAsync(cartItem.Id, newQuantity);

            // Assert
            _mockCartItemRemover.Verify(r => r.RemoveAsync(cartItem.Id), Times.Once);
            _mockUnitOfWork.Verify(u => u.CartItem.Update(cartItem), Times.Never); // Should not update, should remove instead
        }
    }
}
