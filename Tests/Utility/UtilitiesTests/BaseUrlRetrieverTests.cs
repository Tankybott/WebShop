using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Utility.Common;

namespace Utility.Common.Tests
{
    [TestFixture]
    public class BaseUrlRetrieverTests
    {
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private BaseUrlRetriever _baseUrlRetriever;

        [SetUp]
        public void Setup()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _baseUrlRetriever = new BaseUrlRetriever(_mockHttpContextAccessor.Object);
        }

        #region GetBaseUrl

        [Test]
        public void GetBaseUrl_ShouldReturnUrl_WhenRequestExists()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("example.com");
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(context);

            // Act
            var result = _baseUrlRetriever.GetBaseUrl();

            // Assert
            Assert.That(result, Is.EqualTo("https://example.com"));
        }

        [Test]
        public void GetBaseUrl_ShouldReturnEmptyString_WhenRequestIsNull()
        {
            // Arrange
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

            // Act
            var result = _baseUrlRetriever.GetBaseUrl();

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        #endregion
    }
}
