using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Moq;
using NUnit.Framework;
using Services.CarrierService;
using System.Linq.Expressions;

namespace Services.CarrierService.Tests
{
    [TestFixture]
    public class CarrierServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ICarrierRepository> _mockCarrierRepository;
        private CarrierService _carrierService;

        [SetUp]
        public void Setup()
        {
            _mockCarrierRepository = new Mock<ICarrierRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.Carrier).Returns(_mockCarrierRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _carrierService = new CarrierService(_mockUnitOfWork.Object);
        }

        #region GetAllCarriers

        [Test]
        public async Task GetAllCarriers_ShouldReturnAllCarriers_WhenCalled()
        {
            // Arrange
            var carriers = new List<Carrier> { new Carrier(), new Carrier() };
            _mockCarrierRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Carrier, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Carrier, object>>>()
            )).ReturnsAsync(carriers);

            // Act
            var result = await _carrierService.GetAllCarriers();

            // Assert
            Assert.That(result, Is.EqualTo(carriers));
        }

        #endregion

        #region GetCarrierAsync

        [Test]
        public async Task GetCarrierAsync_ShouldReturnCarrier_WhenIdIsProvided()
        {
            // Arrange
            var expected = new Carrier { Id = 1 };
            SetupMockForGetAsync(expected);

            // Act
            var result = await _carrierService.GetCarrierAsync(1);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetCarrierAsync_ShouldReturnNewCarrier_WhenIdIsNull()
        {
            // Act
            var result = await _carrierService.GetCarrierAsync(null);

            // Assert
            Assert.That(result, Is.TypeOf<Carrier>());
        }

        #endregion

        #region DeleteById

        [Test]
        public async Task DeleteById_ShouldRemoveCarrier_WhenFound()
        {
            // Arrange
            var carrier = new Carrier { Id = 1 };
            SetupMockForGetAsync(carrier);

            // Act
            await _carrierService.DeleteById(1);

            // Assert
            _mockCarrierRepository.Verify(r => r.Remove(carrier), Times.Once);
        }

        [Test]
        public void DeleteById_ShouldThrowException_WhenNotFound()
        {
            // Arrange
            SetupMockForGetAsync(null);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _carrierService.DeleteById(123));
        }

        #endregion

        #region Upsert

        [Test]
        public async Task Upsert_ShouldAddCarrier_WhenIdIsZero()
        {
            // Arrange
            var carrier = new Carrier { Id = 0 };

            // Act
            await _carrierService.Upsert(carrier);

            // Assert
            _mockCarrierRepository.Verify(r => r.Add(carrier), Times.Once);
        }

        [Test]
        public async Task Upsert_ShouldUpdateCarrier_WhenIdIsNonZero()
        {
            // Arrange
            var carrier = new Carrier { Id = 5 };

            // Act
            await _carrierService.Upsert(carrier);

            // Assert
            _mockCarrierRepository.Verify(r => r.Update(carrier), Times.Once);
        }

        #endregion

        #region Helpers

        private void SetupMockForGetAsync(Carrier? carrier)
        {
            _mockCarrierRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Carrier, bool>>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>()
            )).ReturnsAsync(carrier);
        }

        #endregion
    }
}
