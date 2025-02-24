using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Models.FormModel;
using Services.CartServices;
using Services.CartServices.CustomExeptions;
using Services.CartServices.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Utility.CalculationClasses;
using Models.DatabaseRelatedModels;

namespace Tests.Services.CartServicesTests
{
    [TestFixture]
    public class CartItemCreatorTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductRepository> _mockProductRepository;
        private ICartItemCreator _cartItemCreator;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(u => u.Product).Returns(_mockProductRepository.Object);

            _cartItemCreator = new CartItemCreator(_mockUnitOfWork.Object);
        }

        private void SetupMockForGetProductAsync(Product? product)
        {
            _mockProductRepository
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, false))
                .ReturnsAsync(product);
        }

        [Test]
        public async Task CreateCartItemAsync_ShouldReturnCartItem_WithCorrectPrice_WhenNoDiscount()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 2, IsAddedWithDiscount = false };
            var product = new Product { Id = 1, Price = 100m, StockQuantity = 10, DiscountId = null };

            SetupMockForGetProductAsync(product);

            // Act
            var result = await _cartItemCreator.CreateCartItemAsync(formModel);

            // Assert
            Assert.That(result.CurrentPrice, Is.EqualTo(100m));
            Assert.That(result.ProductId, Is.EqualTo(formModel.ProductId));
            Assert.That(result.ProductQuantity, Is.EqualTo(formModel.ProductQuantity));
        }

        [Test]
        public async Task CreateCartItemAsync_ShouldThrowArgumentException_WhenStockIsInsufficient()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 15, IsAddedWithDiscount = false };
            var product = new Product { Id = 1, Price = 100m, StockQuantity = 10 };

            SetupMockForGetProductAsync(product);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _cartItemCreator.CreateCartItemAsync(formModel));
            Assert.That(ex.Message, Is.EqualTo("Requested quantity exceeds available stock."));
        }

        [Test]
        public async Task CreateCartItemAsync_ShouldApplyDiscount_WhenDiscountIsActive()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 1, IsAddedWithDiscount = true };
            var product = new Product
            {
                Id = 1,
                Price = 200m,
                StockQuantity = 5,
                DiscountId = 1,
                Discount = new Discount { Percentage = 20 } // 20% discount
            };

            SetupMockForGetProductAsync(product);

            // Act
            var result = await _cartItemCreator.CreateCartItemAsync(formModel);

            // Assert
            var expectedPrice = DiscountedPriceCalculator.CalculatePriceOfDiscount(product.Price, product.Discount.Percentage);
            Assert.That(result.CurrentPrice, Is.EqualTo(expectedPrice));
        }

        [Test]
        public void CreateCartItemAsync_ShouldThrowDiscountOutOfDateException_WhenDiscountIsNotActive()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 1, IsAddedWithDiscount = true };
            var product = new Product
            {
                Id = 1,
                Price = 200m,
                StockQuantity = 5,
                DiscountId = 0, // Discount is inactive
                Discount = new Discount { Percentage = 20 }
            };

            SetupMockForGetProductAsync(product);

            // Act & Assert
            Assert.ThrowsAsync<DiscountOutOfDateException>(() => _cartItemCreator.CreateCartItemAsync(formModel));
        }

        [Test]
        public async Task CreateCartItemAsync_ShouldReturnCartItem_WithNormalPrice_WhenDiscountIsNotApplied()
        {
            // Arrange
            var formModel = new CartItemFormModel { ProductId = 1, ProductQuantity = 1, IsAddedWithDiscount = false };
            var product = new Product { Id = 1, Price = 150m, StockQuantity = 10, DiscountId = null };

            SetupMockForGetProductAsync(product);

            // Act
            var result = await _cartItemCreator.CreateCartItemAsync(formModel);

            // Assert
            Assert.That(result.CurrentPrice, Is.EqualTo(150m));
            Assert.That(result.ProductId, Is.EqualTo(formModel.ProductId));
            Assert.That(result.ProductQuantity, Is.EqualTo(formModel.ProductQuantity));
        }
    }
}
