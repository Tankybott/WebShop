using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Services.CartServices;
using Services.CartServices.Interfaces;
using Services.ProductService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartPriceSynchronizerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductPriceRetriver> _mockProductPriceRetriver;
        private Mock<ICartRepository> _mockCartRepository;
        private ICartPriceSynchronizer _cartPriceSynchronizer;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductPriceRetriver = new Mock<IProductPriceRetriver>();
            _mockCartRepository = new Mock<ICartRepository>();

            _mockUnitOfWork.Setup(u => u.Cart).Returns(_mockCartRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.Cart.Update(It.IsAny<Cart>()));

            _cartPriceSynchronizer = new CartPriceSynchronizer(
                _mockUnitOfWork.Object,
                _mockProductPriceRetriver.Object
            );
        }

        private void SetupMockForGetCartAsync(Cart? cart)
        {
            _mockCartRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .ReturnsAsync(cart);
        }

        private void SetupMockForProductPrice(int productId, decimal price)
        {
            _mockProductPriceRetriver
                .Setup(p => p.GetProductPriceAsync(productId))
                .ReturnsAsync(price);
        }

        [Test]
        public async Task Synchronize_ShouldReturnEmptyList_WhenCartDoesNotExist()
        {
            // Arrange
            SetupMockForGetCartAsync(null);

            // Act
            var result = await _cartPriceSynchronizer.Synchronize(1);

            // Assert
            Assert.That(result, Is.Empty);
            _mockUnitOfWork.Verify(u => u.Cart.Update(It.IsAny<Cart>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task Synchronize_ShouldReturnEmptyList_WhenCartHasNoItems()
        {
            // Arrange
            var cart = new Cart { Id = 1, Items = new List<CartItem>() };
            SetupMockForGetCartAsync(cart);

            // Act
            var result = await _cartPriceSynchronizer.Synchronize(cart.Id);

            // Assert
            Assert.That(result, Is.Empty);
            _mockUnitOfWork.Verify(u => u.Cart.Update(It.IsAny<Cart>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task Synchronize_ShouldReturnEmptyList_WhenAllPricesAreAlreadyCorrect()
        {
            // Arrange
            var cart = new Cart
            {
                Id = 1,
                Items = new List<CartItem>
                {
                    new CartItem { Id = 1, ProductId = 101, CurrentPrice = 50m, Product = new Product { Id = 101, Price = 50m } },
                    new CartItem { Id = 2, ProductId = 102, CurrentPrice = 30m, Product = new Product { Id = 102, Price = 30m } }
                }
            };

            SetupMockForGetCartAsync(cart);
            SetupMockForProductPrice(101, 50m);
            SetupMockForProductPrice(102, 30m);

            // Act
            var result = await _cartPriceSynchronizer.Synchronize(cart.Id);

            // Assert
            Assert.That(result, Is.Empty);
            _mockUnitOfWork.Verify(u => u.Cart.Update(It.IsAny<Cart>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task Synchronize_ShouldUpdatePricesAndReturnUpdatedItemIds_WhenPricesAreOutdated()
        {
            // Arrange
            var cart = new Cart
            {
                Id = 1,
                Items = new List<CartItem>
                {
                    new CartItem { Id = 1, ProductId = 101, CurrentPrice = 50m, Product = new Product { Id = 101, Price = 60m } },
                    new CartItem { Id = 2, ProductId = 102, CurrentPrice = 30m, Product = new Product { Id = 102, Price = 25m } }
                }
            };

            SetupMockForGetCartAsync(cart);
            SetupMockForProductPrice(101, 60m); // Price increased
            SetupMockForProductPrice(102, 25m); // Price decreased

            // Act
            var result = await _cartPriceSynchronizer.Synchronize(cart.Id);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That(cart.Items.ElementAt(0).CurrentPrice, Is.EqualTo(60m)); // ✅ Fixed indexing
            Assert.That(cart.Items.ElementAt(1).CurrentPrice, Is.EqualTo(25m)); // ✅ Fixed indexing

            _mockUnitOfWork.Verify(u => u.Cart.Update(cart), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task Synchronize_ShouldOnlyUpdateItemsThatChanged()
        {
            // Arrange
            var cart = new Cart
            {
                Id = 1,
                Items = new List<CartItem>
                {
                    new CartItem { Id = 1, ProductId = 101, CurrentPrice = 50m, Product = new Product { Id = 101, Price = 60m } },
                    new CartItem { Id = 2, ProductId = 102, CurrentPrice = 30m, Product = new Product { Id = 102, Price = 30m } } // No change
                }
            };

            SetupMockForGetCartAsync(cart);
            SetupMockForProductPrice(101, 60m); // Price increased
            SetupMockForProductPrice(102, 30m); // No change

            // Act
            var result = await _cartPriceSynchronizer.Synchronize(cart.Id);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<int> { 1 }));
            Assert.That(cart.Items.ElementAt(0).CurrentPrice, Is.EqualTo(60m)); // ✅ Fixed indexing
            Assert.That(cart.Items.ElementAt(1).CurrentPrice, Is.EqualTo(30m)); // ✅ Fixed indexing

            _mockUnitOfWork.Verify(u => u.Cart.Update(cart), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
