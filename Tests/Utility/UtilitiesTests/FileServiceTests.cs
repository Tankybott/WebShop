using NUnit.Framework;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Utility.Common;
using Serilog;

namespace Utility.Common.Tests
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
        public async Task CreateFileAsync_ShouldCreateFile_WhenValidInputStreamAndOutputPathProvided()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var outputPath = Path.Combine(tempDirectory, "testfile.txt");
            var inputData = "Test data";
            await using var inputStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(inputData));

            // Act
            await _fileService.CreateFileAsync(inputStream, outputPath);

            // Assert
            Assert.That(File.Exists(outputPath), Is.True, "File should exist at the specified output path");

            // Cleanup
            Directory.Delete(tempDirectory, true);
        }

        [Test]
        public async Task CreateFileAsync_ShouldCreateDirectory_WhenDirectoryDoesNotExist()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var outputPath = Path.Combine(tempDirectory, "testfile.txt");
            var inputData = "Test data";
            await using var inputStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(inputData));

            // Act
            await _fileService.CreateFileAsync(inputStream, outputPath);

            // Assert
            Assert.That(Directory.Exists(tempDirectory), Is.True, "Directory should be created if it does not exist");

            // Cleanup
            Directory.Delete(tempDirectory, true);
        }

        [Test]
        public async Task DeleteFileAsync_ShouldDeleteFile_WhenFileExists()
        {
            // Arrange
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var filePath = Path.Combine(tempDirectory, "testfile.txt");
            Directory.CreateDirectory(tempDirectory);
            await File.WriteAllTextAsync(filePath, "Test data");

            // Act
            await _fileService.DeleteFileAsync(filePath);

            // Assert
            Assert.That(File.Exists(filePath), Is.False, "File should be deleted if it exists");

            // Cleanup
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}
