using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.DatabaseRelatedModels;
using Utility.Common.Interfaces;
using DataAccess.Repository.IRepository;
using Services.PhotoService.Interfaces;
using Services.PhotoSetService;

namespace Services.PhotoSetService.Tests
{
    [TestFixture]
    public class PhotoSetServiceTests
    {
        private Mock<IFileNameCreator> _mockFileNameCreator;
        private Mock<IPhotoService> _mockPhotoService;
        private Mock<IPathCreator> _mockPathCreator;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private PhotoSetService _photoSetService;

        [SetUp]
        public void Setup()
        {
            _mockFileNameCreator = new Mock<IFileNameCreator>();
            _mockPhotoService = new Mock<IPhotoService>();
            _mockPathCreator = new Mock<IPathCreator>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _photoSetService = new PhotoSetService(
                _mockFileNameCreator.Object,
                _mockPhotoService.Object,
                _mockPathCreator.Object,
                _mockUnitOfWork.Object
            );
        }

        #region CreatePhotoSetAsync

        [Test]
        public async Task CreatePhotoSetAsync_ShouldCallUploadPhotosFromSet_WhenPhotoAndFileNamesAreProvided()
        {
            // Arrange
            var photo = new Mock<IFormFile>().Object;
            _mockFileNameCreator.Setup(fc => fc.CreateFileName("webp")).Returns("filename.webp");

            // Act
            await _photoSetService.CreatePhotoSetAsync(photo, true);

            // Assert
            _mockPhotoService.Verify(ps => ps.UploadPhotosFromSet(photo, "filename.webp", "filename.webp", "images/product"), Times.Once);
        }

        [Test]
        public async Task CreatePhotoSetAsync_ShouldReturnCorrectPhotoUrlSet_WhenCalledWithValidPhoto()
        {
            // Arrange
            var photo = new Mock<IFormFile>().Object;
            _mockFileNameCreator.Setup(fc => fc.CreateFileName("webp")).Returns("filename.webp");
            _mockPathCreator.Setup(pc => pc.CreateUrlPath(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns((string dir, string file) => $"{dir}/{file}");

            // Act
            var result = await _photoSetService.CreatePhotoSetAsync(photo, true);

            // Assert
            Assert.That(result.ThumbnailPhotoUrl, Is.EqualTo("images/product/filename.webp"));
            Assert.That(result.BigPhotoUrl, Is.EqualTo("images/product/filename.webp"));
            Assert.That(result.IsMainPhoto, Is.True);
        }

        #endregion

        #region DeletePhotoSetAsync

        [Test]
        public async Task DeletePhotoSetAsync_ShouldCallDeletePhotosFromServer_WhenPhotoUrlSetExists()
        {
            // Arrange
            var photoUrlSet = new PhotoUrlSet { ThumbnailPhotoUrl = "images/product/thumbnail.jpg" };
            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .ReturnsAsync(photoUrlSet);

            // Act
            await _photoSetService.DeletePhotoSetAsync("images/product/thumbnail.jpg");

            // Assert
            _mockPhotoService.Verify(ps => ps.DeletePhotosFromServer(photoUrlSet), Times.Once);
        }

        [Test]
        public async Task DeletePhotoSetAsync_ShouldRemovePhotoUrlSet_WhenPhotoUrlSetExists()
        {
            // Arrange
            var photoUrlSet = new PhotoUrlSet { ThumbnailPhotoUrl = "images/product/thumbnail.jpg" };
            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .ReturnsAsync(photoUrlSet);

            // Act
            await _photoSetService.DeletePhotoSetAsync("images/product/thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.PhotoUrlSets.Remove(photoUrlSet), Times.Once);
        }

        [Test]
        public async Task DeletePhotoSetAsync_ShouldCallSave_WhenPhotoUrlSetExists()
        {
            // Arrange
            var photoUrlSet = new PhotoUrlSet { ThumbnailPhotoUrl = "images/product/thumbnail.jpg" };
            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .ReturnsAsync(photoUrlSet);

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            await _photoSetService.DeletePhotoSetAsync("images/product/thumbnail.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeletePhotoSetAsync_ShouldNotCallDeletePhotosFromServer_WhenPhotoUrlSetDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .ReturnsAsync((PhotoUrlSet)null);

            // Act
            await _photoSetService.DeletePhotoSetAsync("images/product/nonexistent.jpg");

            // Assert
            _mockPhotoService.Verify(ps => ps.DeletePhotosFromServer(It.IsAny<PhotoUrlSet>()), Times.Never);
        }

        [Test]
        public async Task DeletePhotoSetAsync_ShouldNotCallSave_WhenPhotoUrlSetDoesNotExist()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.PhotoUrlSets.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PhotoUrlSet, bool>>>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .ReturnsAsync((PhotoUrlSet)null);

            // Act
            await _photoSetService.DeletePhotoSetAsync("images/product/nonexistent.jpg");

            // Assert
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        #endregion
    }
}
