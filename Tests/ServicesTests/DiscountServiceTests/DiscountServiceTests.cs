using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Moq;
using NUnit.Framework;
using Services.DiscountService;
using System.Linq.Expressions;
using Utility.DiscountQueues.Interfaces;

namespace Services.DiscountService.Tests
{
    [TestFixture]
    public class DiscountServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IDiscountRepository> _mockDiscountRepository;
        private Mock<IDiscountActivationQueue> _mockActivationQueue;
        private Mock<IDiscountDeletionQueue> _mockDeletionQueue;
        private DiscountService _discountService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            _mockUnitOfWork.Setup(u => u.Discount).Returns(_mockDiscountRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            _mockActivationQueue = new Mock<IDiscountActivationQueue>();
            _mockDeletionQueue = new Mock<IDiscountDeletionQueue>();

            _discountService = new DiscountService(
                _mockUnitOfWork.Object,
                _mockActivationQueue.Object,
                _mockDeletionQueue.Object
            );
        }

        #region CreateDiscountAsync

        [Test]
        public async Task CreateDiscountAsync_ShouldAddAndReturnDiscount_WhenStartTimeIsInPast()
        {
            // Arrange
            var now = DateTime.Now.AddMinutes(-5);
            var end = DateTime.Now.AddMinutes(10);

            // Act
            var result = await _discountService.CreateDiscountAsync(now, end, 10);

            // Assert
            _mockDiscountRepository.Verify(r => r.Add(It.Is<Discount>(d => d.Percentage == 10)), Times.Once);
        }

        [Test]
        public async Task CreateDiscountAsync_ShouldEnqueueActivation_WhenStartTimeIsInFuture()
        {
            // Arrange
            var start = DateTime.Now.AddMinutes(10);
            var end = DateTime.Now.AddMinutes(20);

            // Act
            var result = await _discountService.CreateDiscountAsync(start, end, 15);

            // Assert
            _mockActivationQueue.Verify(q => q.EnqueueAsync(It.IsAny<Discount>()), Times.Once);
        }

        [Test]
        public async Task CreateDiscountAsync_ShouldEnqueueDeletion_WhenCalled()
        {
            // Arrange
            var start = DateTime.Now.AddMinutes(1);
            var end = DateTime.Now.AddMinutes(5);

            // Act
            var result = await _discountService.CreateDiscountAsync(start, end, 25);

            // Assert
            _mockDeletionQueue.Verify(q => q.EnqueueAsync(It.IsAny<Discount>()), Times.Once);
        }

        [Test]
        public void CreateDiscountAsync_ShouldThrowException_WhenDiscountIsInvalid()
        {
            // Arrange
            var start = DateTime.Now.AddMinutes(10);
            var end = DateTime.Now.AddMinutes(5); // invalid: end < start

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() =>
                _discountService.CreateDiscountAsync(start, end, 10));
        }

        #endregion

        #region UpdateDiscountAsync

        [Test]
        public async Task UpdateDiscountAsync_ShouldRemoveOldDiscount_WhenExists()
        {
            // Arrange
            var existing = new Discount { Id = 1 };
            SetupMockForGetAsync(existing);

            // Act
            await _discountService.UpdateDiscountAsync(1, DateTime.Now.AddMinutes(1), DateTime.Now.AddMinutes(10), 20);

            // Assert
            _mockDiscountRepository.Verify(r => r.Remove(existing), Times.Once);
        }

        [Test]
        public async Task UpdateDiscountAsync_ShouldRemoveFromQueues_WhenOldExists()
        {
            // Arrange
            var existing = new Discount { Id = 5 };
            SetupMockForGetAsync(existing);

            // Act
            await _discountService.UpdateDiscountAsync(5, DateTime.Now.AddMinutes(1), DateTime.Now.AddMinutes(5), 30);

            // Assert
            _mockActivationQueue.Verify(q => q.RemoveByIdAsync(5), Times.Once);
            _mockDeletionQueue.Verify(q => q.RemoveByIdAsync(5), Times.Once);
        }

        #endregion

        #region DeleteByIdAsync

        [Test]
        public async Task DeleteByIdAsync_ShouldRemoveFromQueuesAndRepo_WhenDiscountExists()
        {
            // Arrange
            var existing = new Discount { Id = 7 };
            SetupMockForGetAsync(existing);

            // Act
            await _discountService.DeleteByIdAsync(7);

            // Assert
            _mockActivationQueue.Verify(q => q.RemoveByIdAsync(7), Times.Once);
            _mockDeletionQueue.Verify(q => q.RemoveByIdAsync(7), Times.Once);
            _mockDiscountRepository.Verify(r => r.Remove(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteByIdAsync_ShouldNotRemoveFromRepo_WhenDiscountIsNull()
        {
            // Arrange
            SetupMockForGetAsync(null);

            // Act
            await _discountService.DeleteByIdAsync(999);

            // Assert
            _mockDiscountRepository.Verify(r => r.Remove(It.IsAny<Discount>()), Times.Never);
        }

        #endregion

        #region Helpers

        private void SetupMockForGetAsync(Discount? discount)
        {
            _mockDiscountRepository
                .Setup(r => r.GetAsync(
                    It.IsAny<Expression<Func<Discount, bool>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>()
                ))
                .ReturnsAsync(discount);
        }

        #endregion
    }
}
