using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Utility.Common.Interfaces;
using SkiaSharp;

namespace Utility.Common.Tests
{
    [TestFixture]
    public class ImageProcessorTests
    {
        private Mock<IFileService> _mockFileService;
        private ImageProcessor _imageProcessor;

        [SetUp]
        public void Setup()
        {
            _mockFileService = new Mock<IFileService>();
            _imageProcessor = new ImageProcessor(_mockFileService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            // Dispose of any resources if needed (specific to test logic)
        }

        #region CreateThumbnailAsync

        [Test]
        public async Task CreateThumbnailAsync_ShouldCreateThumbnailImage_WhenValidInputFileAndOutputPathProvided()
        {
            // Arrange
            var inputFileMock = CreateMockFormFile("test.jpg", 300, 300);
            var outputThumbnailPath = "output/thumbnail.jpg";

            _mockFileService
                .Setup(fs => fs.CreateFileAsync(It.IsAny<Stream>(), outputThumbnailPath))
                .Returns(Task.CompletedTask);

            // Act
            await _imageProcessor.CreateThumbnailAsync(inputFileMock, outputThumbnailPath);

            // Assert
            _mockFileService.Verify(fs => fs.CreateFileAsync(It.IsAny<Stream>(), outputThumbnailPath), Times.Once, "Thumbnail should be created and saved to the specified path");

            // Cleanup
            inputFileMock.OpenReadStream().Dispose();
        }

        #endregion

        #region CreateFullSizeImageAsync

        [Test]
        public async Task CreateFullSizeImageAsync_ShouldCreateFullSizeImage_WhenValidInputFileAndOutputPathProvided()
        {
            // Arrange
            var inputFileMock = CreateMockFormFile("test.jpg", 3000, 2000);
            var outputFullSizePath = "output/fullsize.jpg";

            _mockFileService
                .Setup(fs => fs.CreateFileAsync(It.IsAny<Stream>(), outputFullSizePath))
                .Returns(Task.CompletedTask);

            // Act
            await _imageProcessor.CreateFullSizeImageAsync(inputFileMock, outputFullSizePath);

            // Assert
            _mockFileService.Verify(fs => fs.CreateFileAsync(It.IsAny<Stream>(), outputFullSizePath), Times.Once, "Full-size image should be created and saved to the specified path");

            // Cleanup
            inputFileMock.OpenReadStream().Dispose();
        }

        #endregion

        #region Private Helper Methods

        private IFormFile CreateMockFormFile(string fileName, int width, int height)
        {
            // Create a blank bitmap image with the specified width and height
            var memoryStream = new MemoryStream();
            using (var bitmap = new SKBitmap(width, height))
            using (var image = SKImage.FromBitmap(bitmap))
            {
                image.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(memoryStream);
            }

            memoryStream.Position = 0;

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(f => f.FileName).Returns(fileName);
            mockFormFile.Setup(f => f.Length).Returns(memoryStream.Length);
            mockFormFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);

            return mockFormFile.Object;
        }

        #endregion
    }
}
