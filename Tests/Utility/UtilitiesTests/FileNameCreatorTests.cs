//using NUnit.Framework;
//using Microsoft.AspNetCore.Http;
//using Moq;
//using Utility.Common;


//namespace Tests.ControllersServices.Utilities
//{
//    [TestFixture]
//    public class FileNameCreatorTests
//    {
//        private FileNameCreator _fileNameCreator;

//        [SetUp]
//        public void Setup()
//        {
//            _fileNameCreator = new FileNameCreator();
//        }

//        [Test]
//        public void CreateFileName_ShouldGenerateUniqueFileName()
//        {
//            // Arrange
//            var mockFile = new Mock<IFormFile>();
//            mockFile.Setup(f => f.FileName).Returns("example.txt");

//            // Act
//            var fileName1 = _fileNameCreator.CreateFileName(mockFile.Object);
//            var fileName2 = _fileNameCreator.CreateFileName(mockFile.Object);

//            // Assert
//            Assert.That(fileName1, Is.Not.EqualTo(fileName2)); // Verify filenames are unique
//        }

//        [Test]
//        public void CreateFileName_ShouldIncludeFileExtension()
//        {
//            // Arrange
//            var mockFile = new Mock<IFormFile>();
//            mockFile.Setup(f => f.FileName).Returns("example.txt");

//            // Act
//            var fileName = _fileNameCreator.CreateFileName(mockFile.Object);

//            // Assert
//            Assert.That(fileName, Does.EndWith(".txt")); // Verify correct extension
//        }

//        [Test]
//        public void CreateFileName_ShouldIncludeValidGuid()
//        {
//            // Arrange
//            var mockFile = new Mock<IFormFile>();
//            mockFile.Setup(f => f.FileName).Returns("example.txt");

//            // Act
//            var fileName = _fileNameCreator.CreateFileName(mockFile.Object);

//            // Assert
//            var guidPart = fileName.Split('.')[0];
//            Assert.That(Guid.TryParse(guidPart, out _), Is.True); // Verify GUID is valid
//        }
//    }
//}