using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Services.CartServices;
using Services.CartServices.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartItemRemoverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICartItemRepository> _mockCartItemRepository;
        private ICartItemRemover _cartItemRemover;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCartItemRepository = new Mock<ICartItemRepository>();

            _mockUnitOfWork.Setup(u => u.CartItem).Returns(_mockCartItemRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CartItem.Remove(It.IsAny<CartItem>()));

            _cartItemRemover = new CartItemRemover(_mockUnitOfWork.Object);
        }

        private void SetupMockForGetCartItemAsync(CartItem? cartItem)
        {
            _mockCartItemRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<CartItem, bool>>>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .ReturnsAsync(cartItem);
        }

        [Test]
        public async Task RemoveAsync_ById_ShouldRemoveItem_WhenItemExists()
        {
            // Arrange
            var cartItem = new CartItem { Id = 1 };
            SetupMockForGetCartItemAsync(cartItem);

            // Act
            await _cartItemRemover.RemoveAsync(cartItem.Id);

            // Assert
            _mockCartItemRepository.Verify(r => r.Remove(cartItem), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public void RemoveAsync_ById_ShouldThrowArgumentException_WhenItemDoesNotExist()
        {
            // Arrange
            SetupMockForGetCartItemAsync(null); 

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _cartItemRemover.RemoveAsync(999));
            Assert.That(ex.Message, Is.EqualTo("Cart item doenst exist in database"));
        }

        [Test]
        public async Task RemoveAsync_ByCartItem_ShouldRemoveItem()
        {
            // Arrange
            var cartItem = new CartItem { Id = 1 };
            SetupMockForGetCartItemAsync(cartItem); 

            // Act
            await _cartItemRemover.RemoveAsync(cartItem.Id);

            // Assert
            _mockCartItemRepository.Verify(r => r.Remove(cartItem), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
