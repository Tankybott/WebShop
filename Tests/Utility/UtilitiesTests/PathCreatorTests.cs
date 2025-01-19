using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Utility.Common;

namespace Utility.Common.Tests
{
    [TestFixture]
    public class PathCreatorTests
    {
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private PathCreator _pathCreator;

        [SetUp]
        public void Setup()
        {
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("/var/wwwroot");
            _pathCreator = new PathCreator(_mockWebHostEnvironment.Object);
        }

        #region GetRootPath

        [Test]
        public void GetRootPath_ShouldReturnWwwRootPath_WhenCalled()
        {
            // Act
            var result = _pathCreator.GetRootPath();

            // Assert
            Assert.That(result, Is.EqualTo("/var/wwwroot"), "GetRootPath should return the correct web root path");
        }

        #endregion

        #region CombinePaths

        [Test]
        public void CombinePaths_ShouldCombineTwoPathsIntoSystemFormat_WhenPathsAreProvided()
        {
            // Arrange
            var firstPath = "folder1";
            var secondPath = "folder2/file.txt";

            // Act
            var result = _pathCreator.CombinePaths(firstPath, secondPath);

            // Assert
            var expectedPath = Path.Combine("folder1", "folder2/file.txt")
                                  .Replace('\\', '/'); // Normalize for comparison
            Assert.That(result, Is.EqualTo(expectedPath), "CombinePaths should combine two paths in system format");
        }

        [Test]
        public void CombinePaths_ShouldNormalizeBackslashesToForwardSlashes_WhenPathsContainBackslashes()
        {
            // Arrange
            var inputPath = "folder\\subfolder\\file.txt";

            // Act
            var result = _pathCreator.CombinePaths(inputPath, string.Empty);

            // Assert
            Assert.That(result, Is.EqualTo("folder/subfolder/file.txt"), "Paths should be normalized to forward slashes");
        }

        #endregion

        #region CreateUrlPath

        [Test]
        public void CreateUrlPath_ShouldCreateValidUrlPath_WhenDirectoryAndFileNameAreProvided()
        {
            // Arrange
            var directory = "images";
            var fileName = "photo.jpg";

            // Act
            var result = _pathCreator.CreateUrlPath(directory, fileName);

            // Assert
            Assert.That(result, Is.EqualTo("/images/photo.jpg"), "CreateUrlPath should return a valid URL path");
        }

        [Test]
        public void CreateUrlPath_ShouldAddLeadingSlash_WhenResultDoesNotStartWithSlash()
        {
            // Arrange
            var directory = "folder";
            var fileName = "file.txt";

            // Act
            var result = _pathCreator.CreateUrlPath(directory, fileName);

            // Assert
            Assert.That(result, Is.EqualTo("/folder/file.txt"), "CreateUrlPath should add a leading slash if missing");
        }

        #endregion
    }
}