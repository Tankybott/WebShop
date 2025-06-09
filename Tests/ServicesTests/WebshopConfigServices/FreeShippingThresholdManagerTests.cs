using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Moq;
using NUnit.Framework;
using Services.WebshopConfigServices;

namespace Services.WebshopConfigServices.Tests
{
    [TestFixture]
    public class FreeShippingThresholdManagerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IWebshopConfigRepository> _mockWebshopConfigRepository;
        private FreeShippingThresholdManager _thresholdManager;

        [SetUp]
        public void Setup()
        {
            _mockWebshopConfigRepository = new Mock<IWebshopConfigRepository>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.WebshopConfig).Returns(_mockWebshopConfigRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _thresholdManager = new FreeShippingThresholdManager(_mockUnitOfWork.Object);
        }

        #region UpdateFreeShippingThresholdAsync

        [Test]
        public async Task UpdateFreeShippingThresholdAsync_ShouldUpdateAndSave_WhenThresholdIsDifferent()
        {
            // Arrange
            var config = new WebshopConfig { FreeShippingFrom = 100 };
            SetupMockForGetAsync(config);

            // Act
            await _thresholdManager.UpdateFreeShippingThresholdAsync(200);

            // Assert
            _mockUnitOfWork.Verify(u => u.WebshopConfig.Update(config), Times.Once);
        }

        [Test]
        public async Task UpdateFreeShippingThresholdAsync_ShouldNotUpdate_WhenThresholdIsSame()
        {
            // Arrange
            var config = new WebshopConfig { FreeShippingFrom = 150 };
            SetupMockForGetAsync(config);

            // Act
            await _thresholdManager.UpdateFreeShippingThresholdAsync(150);

            // Assert
            _mockUnitOfWork.Verify(u => u.WebshopConfig.Update(It.IsAny<WebshopConfig>()), Times.Never);
        }

        [Test]
        public async Task UpdateFreeShippingThresholdAsync_ShouldCallSaveAsync_WhenThresholdIsChanged()
        {
            // Arrange
            var config = new WebshopConfig { FreeShippingFrom = null };
            SetupMockForGetAsync(config);

            // Act
            await _thresholdManager.UpdateFreeShippingThresholdAsync(99);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateFreeShippingThresholdAsync_ShouldNotCallSaveAsync_WhenThresholdIsSame()
        {
            // Arrange
            var config = new WebshopConfig { FreeShippingFrom = 75 };
            SetupMockForGetAsync(config);

            // Act
            await _thresholdManager.UpdateFreeShippingThresholdAsync(75);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        #endregion

        #region GetFreeShippingThresholdAsync

        [Test]
        public async Task GetFreeShippingThresholdAsync_ShouldReturnThreshold_WhenCalled()
        {
            // Arrange
            var config = new WebshopConfig { FreeShippingFrom = 123 };
            SetupMockForGetAsync(config);

            // Act
            var result = await _thresholdManager.GetFreeShippingThresholdAsync();

            // Assert
            Assert.That(result, Is.EqualTo(123));
        }

        #endregion

        #region Helpers

        private void SetupMockForGetAsync(WebshopConfig config)
        {
            _mockWebshopConfigRepository
                .Setup(r => r.GetAsync())
                .ReturnsAsync(config);
        }

        #endregion
    }
}
