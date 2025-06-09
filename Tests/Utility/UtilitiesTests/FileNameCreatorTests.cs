using NUnit.Framework;
using Utility.Common;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework.Legacy;

namespace UtilityTests.Common
{
    [TestFixture]
    public class FileNameCreatorTests
    {
        private FileNameCreator _creator;

        [SetUp]
        public void Setup()
        {
            _creator = new FileNameCreator();
        }

        #region CreateFileName (IFormFile)

        [Test]
        public void CreateFileName_ShouldReturnFileNameWithSameExtension_WhenFormFileProvided()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.png");

            var result = _creator.CreateFileName(mockFile.Object);

            StringAssert.EndsWith(".png", result);
        }

        #endregion

        #region CreateFileName (string)

        [Test]
        public void CreateFileName_ShouldReturnFileNameWithGivenExtension_WhenStringExtensionProvided()
        {
            var result = _creator.CreateFileName("txt");

            StringAssert.EndsWith(".txt", result);
        }

        #endregion

        #region CreateJpegFileName

        [Test]
        public void CreateJpegFileName_ShouldReturnFileNameWithJpegExtension_WhenCalled()
        {
            var result = _creator.CreateJpegFileName();

            StringAssert.EndsWith(".jpeg", result);
        }

        #endregion
    }
}
