using NUnit.Framework;
using Moq;
using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;

namespace Services.PhotoSetService.Tests
{
    [TestFixture]
    public class MainPhotoSetSynchronizerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private MainPhotoSetSynchronizer _synchronizer;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _synchronizer = new MainPhotoSetSynchronizer(_mockUnitOfWork.Object);
        }

        #region SynchronizeMainPhotoSetAsync

        [Test]
        public async Task SynchronizeMainPhotoSetAsync_ShouldNotCallUpdate_WhenCurrentMainPhotoMatchesNewThumbnailUrl()
        {
            // Arrange
            var currentMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "thumbnail.jpg",
                IsMainPhoto = true
            };

            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(currentMainPhotoSet);

            // Act
            await _synchronizer.SynchronizeMainPhotoSetAsync("thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.PhotoUrlSets.Update(It.IsAny<PhotoUrlSet>()), Times.Never);
        }

        [Test]
        public async Task SynchronizeMainPhotoSetAsync_ShouldNotCallSave_WhenCurrentMainPhotoMatchesNewThumbnailUrl()
        {
            // Arrange
            var currentMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "thumbnail.jpg",
                IsMainPhoto = true
            };

            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(currentMainPhotoSet);

            // Act
            await _synchronizer.SynchronizeMainPhotoSetAsync("thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task SynchronizeMainPhotoSetAsync_ShouldCallUpdateForCurrentMainPhoto_WhenNewMainPhotoSetExists()
        {
            // Arrange
            var currentMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "current-thumbnail.jpg",
                IsMainPhoto = true
            };

            var newMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "new-thumbnail.jpg",
                IsMainPhoto = false
            };

            _mockUnitOfWork.SetupSequence(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(currentMainPhotoSet) // First call
                .ReturnsAsync(newMainPhotoSet);   // Second call

            // Act
            await _synchronizer.SynchronizeMainPhotoSetAsync("new-thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.PhotoUrlSets.Update(It.Is<PhotoUrlSet>(s => s == currentMainPhotoSet && !s.IsMainPhoto)), Times.Once);
        }

        [Test]
        public async Task SynchronizeMainPhotoSetAsync_ShouldCallUpdateForNewMainPhoto_WhenNewMainPhotoSetExists()
        {
            // Arrange
            var currentMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "current-thumbnail.jpg",
                IsMainPhoto = true
            };

            var newMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "new-thumbnail.jpg",
                IsMainPhoto = false
            };

            _mockUnitOfWork.SetupSequence(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(currentMainPhotoSet) // First call
                .ReturnsAsync(newMainPhotoSet);   // Second call

            // Act
            await _synchronizer.SynchronizeMainPhotoSetAsync("new-thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.PhotoUrlSets.Update(It.Is<PhotoUrlSet>(s => s == newMainPhotoSet && s.IsMainPhoto)), Times.Once);
        }

        [Test]
        public async Task SynchronizeMainPhotoSetAsync_ShouldCallSave_WhenNewMainPhotoSetExists()
        {
            // Arrange
            var currentMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "current-thumbnail.jpg",
                IsMainPhoto = true
            };

            var newMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "new-thumbnail.jpg",
                IsMainPhoto = false
            };

            _mockUnitOfWork.SetupSequence(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(currentMainPhotoSet) // First call
                .ReturnsAsync(newMainPhotoSet);   // Second call

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            await _synchronizer.SynchronizeMainPhotoSetAsync("new-thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task SynchronizeMainPhotoSetAsync_ShouldNotCallUpdate_WhenNewMainPhotoSetDoesNotExist()
        {
            // Arrange
            var currentMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "current-thumbnail.jpg",
                IsMainPhoto = true
            };

            _mockUnitOfWork.SetupSequence(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(currentMainPhotoSet) // First call
                .ReturnsAsync((PhotoUrlSet)null);  // Second call

            // Act
            await _synchronizer.SynchronizeMainPhotoSetAsync("nonexistent-thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.PhotoUrlSets.Update(It.IsAny<PhotoUrlSet>()), Times.Never);
        }

        [Test]
        public async Task SynchronizeMainPhotoSetAsync_ShouldNotCallSave_WhenNewMainPhotoSetDoesNotExist()
        {
            // Arrange
            var currentMainPhotoSet = new PhotoUrlSet
            {
                ThumbnailPhotoUrl = "current-thumbnail.jpg",
                IsMainPhoto = true
            };

            _mockUnitOfWork.SetupSequence(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(currentMainPhotoSet) // First call
                .ReturnsAsync((PhotoUrlSet)null);  // Second call

            // Act
            await _synchronizer.SynchronizeMainPhotoSetAsync("nonexistent-thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        #endregion
    }
}
