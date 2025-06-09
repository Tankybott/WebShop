using NUnit.Framework;
using Services.EmailFactory;
using Services.EmailFactory.models;

namespace Services.EmailFactory.Tests
{
    [TestFixture]
    public class InformationEmailBuilderTests
    {
        private InformationEmailBuilder _emailBuilder;

        [SetUp]
        public void Setup()
        {
            _emailBuilder = new InformationEmailBuilder();
        }

        #region Build

        [Test]
        public void Build_ShouldIncludeTitle_WhenCalled()
        {
            // Arrange
            var input = new InformationEmailInput("Important Update", new[] { "Paragraph one." });

            // Act
            var result = _emailBuilder.Build(input);

            // Assert
            Assert.That(result, Does.Contain("Important Update"));
        }

        [Test]
        public void Build_ShouldIncludeAllParagraphs_WhenCalled()
        {
            // Arrange
            var input = new InformationEmailInput("Title", new[] { "First", "Second" });

            // Act
            var result = _emailBuilder.Build(input);

            // Assert
            Assert.That(result, Does.Contain("First").And.Contain("Second"));
        }

        [Test]
        public void Build_ShouldReturnHtmlDocument_WhenCalled()
        {
            // Arrange
            var input = new InformationEmailInput("HTML Test", new[] { "Content" });

            // Act
            var result = _emailBuilder.Build(input);

            // Assert
            Assert.That(result.TrimStart(), Does.StartWith("<html>"));
        }

        #endregion
    }
}
