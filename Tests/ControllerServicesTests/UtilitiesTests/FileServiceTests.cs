using NUnit.Framework;
using ControllersServices.Utilities;
using ControllersServices.Utilities.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Serilog;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ControllersServices.Utilities
{
    [TestFixture]
    public class FileServiceTests
    {
        private FileService _fileService;

        [SetUp]
        public void Setup()
        {
            _fileService = new FileService();
        }

        [Test]
        public async Task CreateFileAsync_ShouldCreateFileInSpecifiedDirectory()
        {
            // Arrange
            var directory = Path.Combine(Path.GetTempPath(), "TestDirectory");
            var fileName = "TestFile.txt";
            var filePath = Path.Combine(directory, fileName);

            var mockFile = new Mock<IFormFile>();
            var fileContent = "Test content";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns<Stream, System.Threading.CancellationToken>((stream, _) =>
                    {
                        return memoryStream.CopyToAsync(stream);
                    });

            // Act
            await _fileService.CreateFileAsync(mockFile.Object, directory, fileName);

            // Assert
            Assert.That(File.Exists(filePath), Is.True);

            // Cleanup
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        [Test]
        public async Task DeleteFileAsync_ShouldDeleteExistingFile()
        {
            // Arrange
            var directory = Path.Combine(Path.GetTempPath(), "TestDirectory");
            var fileName = "TestFile.txt";
            var filePath = Path.Combine(directory, fileName);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, "Test content");

            // Act
            await _fileService.DeleteFileAsync(filePath);

            // Assert
            Assert.That(File.Exists(filePath), Is.False);

            // Cleanup
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }
}