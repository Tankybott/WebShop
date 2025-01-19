using NUnit.Framework;
using Moq;
using Models;
using Models.DiscountCreateModel;
using Services.PhotoService.Interfaces.DiscountService.Interfaces;
using Models.DatabaseRelatedModels;

namespace Services.ProductService.Tests
{
    [TestFixture]
    public class ProductDiscountUpserterTests
    {
        private Mock<IDiscountService> _mockDiscountService;
        private ProductDiscountUpserter _productDiscountUpserter;

        [SetUp]
        public void Setup()
        {
            _mockDiscountService = new Mock<IDiscountService>();
            _productDiscountUpserter = new ProductDiscountUpserter(_mockDiscountService.Object);
        }

        #region HandleDiscountUpsertAsync

        [Test]
        public async Task HandleDiscountUpsertAsync_ShouldNotCallCreateOrUpdate_WhenDiscountIsUnchanged()
        {
            // Arrange
            var product = new Product();
            var discountCreateModel = new DiscountCreateModel
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                Percentage = 10,
                IsDiscountChanged = false
            };

            // Act
            await _productDiscountUpserter.HandleDiscountUpsertAsync(product, discountCreateModel);

            // Assert
            _mockDiscountService.Verify(d => d.CreateDiscountAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never, "CreateDiscountAsync should not be called when the discount is unchanged.");
            _mockDiscountService.Verify(d => d.UpdateDiscountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never, "UpdateDiscountAsync should not be called when the discount is unchanged.");
        }

        [Test]
        public async Task HandleDiscountUpsertAsync_ShouldCreateNewDiscount_WhenProductHasNoDiscount()
        {
            // Arrange
            var product = new Product();
            var discountCreateModel = new DiscountCreateModel
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                Percentage = 10,
                IsDiscountChanged = true
            };

            _mockDiscountService.Setup(d => d.CreateDiscountAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                                .ReturnsAsync(new Discount { Id = 1 });

            // Act
            await _productDiscountUpserter.HandleDiscountUpsertAsync(product, discountCreateModel);

            // Assert
            Assert.That(product.DiscountId, Is.EqualTo(1));
        }

        [Test]
        public async Task HandleDiscountUpsertAsync_ShouldUpdateExistingDiscount_WhenProductHasDiscount()
        {
            // Arrange
            var product = new Product { DiscountId = 1 };
            var discountCreateModel = new DiscountCreateModel
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                Percentage = 15,
                IsDiscountChanged = true
            };

            _mockDiscountService.Setup(d => d.UpdateDiscountAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                                .ReturnsAsync(new Discount { Id = 1 });

            // Act
            await _productDiscountUpserter.HandleDiscountUpsertAsync(product, discountCreateModel);

            // Assert
            _mockDiscountService.Verify(d => d.UpdateDiscountAsync(1, discountCreateModel.StartTime.Value, discountCreateModel.EndTime.Value, discountCreateModel.Percentage.Value), Times.Once);
        }

        [Test]
        public async Task HandleDiscountUpsertAsync_ShouldDeleteDiscount_WhenDiscountDataIsCleared()
        {
            // Arrange
            var product = new Product { DiscountId = 1 };
            var discountCreateModel = new DiscountCreateModel
            {
                StartTime = null,
                EndTime = null,
                Percentage = null,
                DiscountId = 1
            };

            _mockDiscountService.Setup(d => d.DeleteByIdAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            await _productDiscountUpserter.HandleDiscountUpsertAsync(product, discountCreateModel);

            // Assert
            _mockDiscountService.Verify(d => d.DeleteByIdAsync(1), Times.Once);
            Assert.That(product.DiscountId, Is.Null);
        }

        [Test]
        public void HandleDiscountUpsertAsync_ShouldThrowException_WhenDiscountDataIsInvalid()
        {
            // Arrange
            var product = new Product();
            var discountCreateModel = new DiscountCreateModel
            {
                StartTime = null,
                EndTime = DateTime.Now,
                Percentage = 10
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _productDiscountUpserter.HandleDiscountUpsertAsync(product, discountCreateModel));
            Assert.That(exception.Message, Is.EqualTo("Failed to add discount because of invalid data "));
        }

        #endregion
    }
}
