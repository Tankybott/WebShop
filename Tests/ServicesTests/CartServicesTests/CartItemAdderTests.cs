using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Models.FormModel;
using Services.CartServices;
using Services.CartServices.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartItemAdderTests
    {
        private Mock<ICartRetriver> _mockCartRetriver;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICartItemCreator> _mockCartItemCreator;
        private ICartItemAdder _cartItemAdder;

        [SetUp]
        public void Setup()
        {
            _mockCartRetriver = new Mock<ICartRetriver>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCartItemCreator = new Mock<ICartItemCreator>();

            _mockUnitOfWork.Setup(u => u.Cart.Update(It.IsAny<Cart>())); // ✅ Mocking Update() method
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // ✅ Mock SaveAsync() to avoid exceptions

            _cartItemAdder = new CartItemAdder(
                _mockCartRetriver.Object,
                _mockUnitOfWork.Object,
                _mockCartItemCreator.Object
            );
        }

        private Cart CreateTestCart(int cartId, params CartItem[] items)
        {
            return new Cart { Id = cartId, Items = new List<CartItem>(items) };
        }

        [Test]
        public async Task AddItemAsync_ShouldAddNewItem_WhenProductIsNotInCart()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 2 };
            var newItem = new CartItem { ProductId = 1, ProductQuantity = 2, CurrentPrice = 100m };
            var userCart = CreateTestCart(1); // Empty cart

            _mockCartRetriver.Setup(r => r.RetriveUserCartAsync()).ReturnsAsync(userCart);
            _mockCartItemCreator.Setup(c => c.CreateCartItemAsync(formModel)).ReturnsAsync(newItem);

            // Act
            await _cartItemAdder.AddItemAsync(formModel);

            // Assert
            Assert.That(userCart.Items.Count, Is.EqualTo(1));
            Assert.That(userCart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId), Is.EqualTo(newItem));
            _mockUnitOfWork.Verify(u => u.Cart.Update(userCart), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task AddItemAsync_ShouldIncreaseQuantity_WhenProductAlreadyExistsInCart()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 3 };
            var existingItem = new CartItem { ProductId = 1, ProductQuantity = 2, CurrentPrice = 100m };
            var userCart = CreateTestCart(1, existingItem);

            _mockCartRetriver.Setup(r => r.RetriveUserCartAsync()).ReturnsAsync(userCart);

            // Act
            await _cartItemAdder.AddItemAsync(formModel);

            // Assert
            Assert.That(userCart.Items.Count, Is.EqualTo(1)); // Should still be one item
            Assert.That(existingItem.ProductQuantity, Is.EqualTo(5)); // Updated quantity (2 + 3)
            _mockUnitOfWork.Verify(u => u.Cart.Update(userCart), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task AddItemAsync_ShouldCallCreateCartItem_WhenNewProductIsAdded()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 2, ProductQuantity = 1 };
            var newItem = new CartItem { ProductId = 2, ProductQuantity = 1, CurrentPrice = 50m };
            var userCart = CreateTestCart(1); // Empty cart

            _mockCartRetriver.Setup(r => r.RetriveUserCartAsync()).ReturnsAsync(userCart);
            _mockCartItemCreator.Setup(c => c.CreateCartItemAsync(formModel)).ReturnsAsync(newItem);

            // Act
            await _cartItemAdder.AddItemAsync(formModel);

            // Assert
            _mockCartItemCreator.Verify(c => c.CreateCartItemAsync(formModel), Times.Once);
            Assert.That(userCart.Items.Count, Is.EqualTo(1));
            Assert.That(userCart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId), Is.EqualTo(newItem));
            _mockUnitOfWork.Verify(u => u.Cart.Update(userCart), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
