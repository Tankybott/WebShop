using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models;
using Services.ProductService;
using Services.ProductService.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Utility.CalculationClasses;
using Models.DatabaseRelatedModels;

namespace Tests.Services.ProductServiceTests
{
    [TestFixture]
    public class ProductPriceRetriverTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IProductRepository> _mockProductRepository;
        private IProductPriceRetriver _productPriceRetriver;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(u => u.Product).Returns(_mockProductRepository.Object);

            _productPriceRetriver = new ProductPriceRetriver(_mockUnitOfWork.Object);
        }

        private void SetupMockForGetAsync(Product? product)
        {
            _mockProductRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(product);
        }

        [Test]
        public async Task GetProductPriceAsync_ShouldReturnProductPrice_WhenNoDiscountIsApplied()
        {
            // Arrange
            var product = new Product { Id = 1, Price = 100m, DiscountId = null };
            SetupMockForGetAsync(product);

            // Act
            var result = await _productPriceRetriver.GetProductPriceAsync(product.Id);

            // Assert
            Assert.That(result, Is.EqualTo(100m));
        }

        [Test]
        public async Task GetProductPriceAsync_ShouldReturnDiscountedPrice_WhenDiscountIsApplied()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Price = 200m,
                DiscountId = 1,
                Discount = new Discount { Percentage = 20 } // 20% discount
            };

            SetupMockForGetAsync(product);

            // Act
            var result = await _productPriceRetriver.GetProductPriceAsync(product.Id);

            // Assert
            var expectedPrice = DiscountedPriceCalculator.CalculatePriceOfDiscount(product.Price, product.Discount.Percentage);
            Assert.That(result, Is.EqualTo(expectedPrice));
        }

        [Test]
        public void GetProductPriceAsync_ShouldThrowArgumentException_WhenProductNotFound()
        {
            // Arrange
            SetupMockForGetAsync(null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _productPriceRetriver.GetProductPriceAsync(99));
            Assert.That(ex.Message, Is.EqualTo("Product with id not fount"));
        }
    }
}
