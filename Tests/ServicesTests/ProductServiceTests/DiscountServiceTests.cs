using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;
using System.Linq.Expressions;
using Services.PhotoService.Interfaces.DiscountService;

namespace Tests.ControllersServices.ProductService
{
    [TestFixture]
    public class DiscountServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IDiscountRepository> _mockDiscountRepository;
        private Mock<IActivationDiscountQueue> _mockActivationDiscountQueue;
        private Mock<IDeletionDiscountQueue> _mockDeletionDiscountQueue;
        private DiscountService _discountService;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            _mockActivationDiscountQueue = new Mock<IActivationDiscountQueue>();
            _mockDeletionDiscountQueue = new Mock<IDeletionDiscountQueue>();

            _mockUnitOfWork.Setup(u => u.Discount).Returns(_mockDiscountRepository.Object);

            _discountService = new DiscountService(
                _mockUnitOfWork.Object,
                _mockActivationDiscountQueue.Object,
                _mockDeletionDiscountQueue.Object
            );

            SetupMockForDiscountRepository();
            SetupMockForQueues();
        }

        private void SetupMockForDiscountRepository()
        {
            _mockDiscountRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>(), It.IsAny<string?>(), It.IsAny<bool>()))
                                    .ReturnsAsync(new Discount
                                    {
                                        Id = 1,
                                        StartTime = DateTime.Now,
                                        EndTime = DateTime.Now.AddDays(1),
                                        Percentage = 20
                                    });

            _mockDiscountRepository.Setup(r => r.Add(It.IsAny<Discount>()));
            _mockDiscountRepository.Setup(r => r.Remove(It.IsAny<Discount>()));
        }

        private void SetupMockForQueues()
        {
            _mockActivationDiscountQueue.Setup(q => q.EnqueueAsync(It.IsAny<Discount>())).Returns(Task.CompletedTask);
            _mockActivationDiscountQueue.Setup(q => q.RemoveByIdAsync(It.IsAny<int>())).ReturnsAsync(true);

            _mockDeletionDiscountQueue.Setup(q => q.EnqueueAsync(It.IsAny<Discount>())).Returns(Task.CompletedTask);
            _mockDeletionDiscountQueue.Setup(q => q.RemoveByIdAsync(It.IsAny<int>())).ReturnsAsync(true);
        }

        #region CreateDiscountAsync Tests

        [Test]
        public async Task CreateDiscountAsync_ShouldAddDiscountToRepository()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddDays(1);
            var percentage = 20;

            // Act
            await _discountService.CreateDiscountAsync(startTime, endTime, percentage);

            // Assert
            _mockDiscountRepository.Verify(r => r.Add(It.Is<Discount>(d =>
                d.StartTime == startTime &&
                d.EndTime == endTime &&
                d.Percentage == percentage)), Times.Once);

            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task CreateDiscountAsync_ShouldSetActive_WhenStartTimeIsPastOrPresent()
        {
            // Arrange
            var startTime = DateTime.Now.AddMinutes(-30); // Already started
            var endTime = DateTime.Now.AddDays(1);
            var percentage = 20;

            // Act
            var result = await _discountService.CreateDiscountAsync(startTime, endTime, percentage);

            // Assert
            Assert.That(result.isActive, Is.True);
        }

        [Test]
        public async Task CreateDiscountAsync_ShouldEnqueueInActivationQueue_WhenStartTimeIsFuture()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(2); // Future start
            var endTime = DateTime.Now.AddDays(1);
            var percentage = 20;

            // Act
            await _discountService.CreateDiscountAsync(startTime, endTime, percentage);

            // Assert
            _mockActivationDiscountQueue.Verify(q => q.EnqueueAsync(It.Is<Discount>(d =>
                d.StartTime == startTime &&
                d.EndTime == endTime &&
                d.Percentage == percentage)), Times.Once);
        }

        [Test]
        public async Task CreateDiscountAsync_ShouldEnqueueInDeletionQueue()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddDays(1);
            var percentage = 20;

            // Act
            await _discountService.CreateDiscountAsync(startTime, endTime, percentage);

            // Assert
            _mockDeletionDiscountQueue.Verify(q => q.EnqueueAsync(It.Is<Discount>(d =>
                d.StartTime == startTime &&
                d.EndTime == endTime &&
                d.Percentage == percentage)), Times.Once);
        }

        [Test]
        public void CreateDiscountAsync_ShouldThrowException_WhenDiscountIsInvalid()
        {
            // Arrange
            var startTime = DateTime.Now.AddDays(1);
            var endTime = DateTime.Now; // Invalid: End time is before start time
            var percentage = 20;

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _discountService.CreateDiscountAsync(startTime, endTime, percentage));
        }

        #endregion

        #region UpdateDiscountAsync Tests

        [Test]
        public async Task UpdateDiscountAsync_ShouldRemoveFromActivationQueue_WhenDiscountExists()
        {
            // Arrange
            var discountId = 1;
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddDays(1);
            var percentage = 20;

            // Act
            await _discountService.UpdateDiscountAsync(discountId, startTime, endTime, percentage);

            // Assert
            _mockActivationDiscountQueue.Verify(q => q.RemoveByIdAsync(discountId), Times.Once);
        }

        [Test]
        public async Task UpdateDiscountAsync_ShouldRemoveFromDeletionQueue_WhenDiscountExists()
        {
            // Arrange
            var discountId = 1;
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddDays(1);
            var percentage = 20;

            // Act
            await _discountService.UpdateDiscountAsync(discountId, startTime, endTime, percentage);

            // Assert
            _mockDeletionDiscountQueue.Verify(q => q.RemoveByIdAsync(discountId), Times.Once);
        }

        [Test]
        public async Task UpdateDiscountAsync_ShouldCreateNewDiscount()
        {
            // Arrange
            var discountId = 1;
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddDays(1);
            var percentage = 20;

            // Act
            await _discountService.UpdateDiscountAsync(discountId, startTime, endTime, percentage);

            // Assert
            _mockDiscountRepository.Verify(r => r.Add(It.Is<Discount>(d =>
                d.StartTime == startTime &&
                d.EndTime == endTime &&
                d.Percentage == percentage)), Times.Once);
        }

        [Test]
        public void UpdateDiscountAsync_ShouldThrowException_WhenDiscountIsInvalid()
        {
            // Arrange
            var discountId = 1;
            var startTime = DateTime.Now.AddDays(1);
            var endTime = DateTime.Now; // Invalid: end time is before start time
            var percentage = 20;

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _discountService.UpdateDiscountAsync(discountId, startTime, endTime, percentage));
        }

        #endregion
        #region SetActive Tests

        [Test]
        public void SetActive_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var discount = new Discount { isActive = false };

            // Act
            _discountService.SetActive(discount);

            // Assert
            Assert.That(discount.isActive, Is.True);
        }

        #endregion
        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ShouldRemoveFromActivationQueue()
        {
            // Arrange
            var discountId = 1;

            // Act
            await _discountService.DeleteByIdAsync(discountId);

            // Assert
            _mockActivationDiscountQueue.Verify(q => q.RemoveByIdAsync(discountId), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveFromDeletionQueue()
        {
            // Arrange
            var discountId = 1;

            // Act
            await _discountService.DeleteByIdAsync(discountId);

            // Assert
            _mockDeletionDiscountQueue.Verify(q => q.RemoveByIdAsync(discountId), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldGetDiscountById()
        {
            // Arrange
            var discountId = 1;

            // Act
            await _discountService.DeleteByIdAsync(discountId);

            // Assert
            _mockDiscountRepository.Verify(r => r.GetAsync(It.Is<Expression<Func<Discount, bool>>>(d => d.Compile().Invoke(new Discount { Id = discountId })),
                                                            null, false), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveDiscountFromRepository_WhenDiscountExists()
        {
            // Arrange
            var discountId = 1;

            // Act
            await _discountService.DeleteByIdAsync(discountId);

            // Assert
            _mockDiscountRepository.Verify(r => r.Remove(It.Is<Discount>(d => d.Id == discountId)), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldNotRemoveDiscountFromRepository_WhenDiscountDoesNotExist()
        {
            // Arrange
            var discountId = 1;
            _mockDiscountRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Discount, bool>>>(), null, false))
                                   .ReturnsAsync((Discount?)null);

            // Act
            await _discountService.DeleteByIdAsync(discountId);

            // Assert
            _mockDiscountRepository.Verify(r => r.Remove(It.IsAny<Discount>()), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ShouldCallSaveAsync()
        {
            // Arrange
            var discountId = 1;

            // Act
            await _discountService.DeleteByIdAsync(discountId);

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        #endregion
    }
}