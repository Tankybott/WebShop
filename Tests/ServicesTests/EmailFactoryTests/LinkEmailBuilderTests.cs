using NUnit.Framework;
using Services.EmailFactory;
using Services.EmailFactory.models;
using System.Text.Encodings.Web;

namespace Services.EmailFactory.Tests
{
    [TestFixture]
    public class LinkEmailBuilderTests
    {
        private LinkEmailBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new LinkEmailBuilder();
        }

        #region Build

        [Test]
        public void Build_ShouldIncludeMessage_WhenCalled()
        {
            // Arrange
            var input = new LinkEmailInput("This is a test message", "https://example.com");

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result, Does.Contain("This is a test message"));
        }

        [Test]
        public void Build_ShouldIncludeEncodedLink_WhenCalled()
        {
            // Arrange
            var rawLink = "https://example.com/test?query=abc&val=1";
            var encoded = HtmlEncoder.Default.Encode(rawLink);
            var input = new LinkEmailInput("Message", rawLink);

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result, Does.Contain(encoded));
        }

        [Test]
        public void Build_ShouldReturnHtml_WhenCalled()
        {
            // Arrange
            var input = new LinkEmailInput("Test message", "https://example.com");

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result.TrimStart(), Does.StartWith("<html>"));
        }

        #endregion
    }
}
