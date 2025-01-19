using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.PhotoService;
using Utility.Common.Interfaces;
using Models.DatabaseRelatedModels;

namespace Services.PhotoService.Tests
{
    [TestFixture]
    public class PhotoServiceTests
    {
        private Mock<IPathCreator> _mockPathCreator;
        private Mock<IImageProcessor> _mockImageProcessor;
        private Mock<IFileService> _mockFileService;

        private PhotoService _photoService;

        [SetUp]
        public void Setup()
        {
            _mockPathCreator = new Mock<IPathCreator>();
            _mockImageProcessor = new Mock<IImageProcessor>();
            _mockFileService = new Mock<IFileService>();

            _photoService = new PhotoService(
                _mockPathCreator.Object,
                _mockImageProcessor.Object,
                null, // Removed the synchronizer dependency as it is no longer used
                _mockFileService.Object
            );
        }

        #region UploadPhotosFromSet

        [Test]
        public async Task UploadPhotosFromSet_ShouldCallImageProcessorMethods_WhenPhotoAndPathsAreProvided()
        {
            // Arrange
            var photo = new Mock<IFormFile>().Object;
            var thumbnailPhotoName = "thumbnail.jpg";
            var fullSizePhotoName = "fullsize.jpg";
            var imageDirectory = "product/images";

            _mockPathCreator.Setup(p => p.GetRootPath()).Returns("/root");
            _mockPathCreator.Setup(p => p.CombinePaths(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns((string p1, string p2) => $"{p1}/{p2}");

            _mockImageProcessor.Setup(ip => ip.CreateThumbnailAsync(photo, It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockImageProcessor.Setup(ip => ip.CreateFullSizeImageAsync(photo, It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await _photoService.UploadPhotosFromSet(photo, thumbnailPhotoName, fullSizePhotoName, imageDirectory);

            // Assert
            _mockImageProcessor.Verify(ip => ip.CreateThumbnailAsync(photo, "/root/product/images/thumbnail.jpg"), Times.Once);
            _mockImageProcessor.Verify(ip => ip.CreateFullSizeImageAsync(photo, "/root/product/images/fullsize.jpg"), Times.Once);
        }

        #endregion

        #region DeletePhotosFromServer

        [Test]
        public async Task DeletePhotosFromServer_ShouldCallDeleteFileMethodForEachPhoto_WhenPhotoSetIsProvided()
        {
            {
                // Arrange
                var photoSet = new PhotoUrlSet
                {
                    ThumbnailPhotoUrl = "images/thumbnail.jpg",
                    BigPhotoUrl = "images/fullsize.jpg"
                };

                _mockPathCreator.Setup(p => p.GetRootPath()).Returns("/root");
                _mockPathCreator.Setup(p => p.CombinePaths(It.IsAny<string>(), It.IsAny<string>()))
                                .Returns((string p1, string p2) => $"{p1}/{p2}");

                _mockFileService.Setup(fs => fs.DeleteFileAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

                // Act
                await _photoService.DeletePhotosFromServer(photoSet);

                // Assert
                _mockFileService.Verify(fs => fs.DeleteFileAsync("/root/images/thumbnail.jpg"), Times.Once);
                _mockFileService.Verify(fs => fs.DeleteFileAsync("/root/images/fullsize.jpg"), Times.Once);
            }

            #endregion
        }
    }
}
