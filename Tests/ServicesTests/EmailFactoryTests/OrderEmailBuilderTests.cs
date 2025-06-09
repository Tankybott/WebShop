using NUnit.Framework;
using Services.EmailFactory;
using Services.EmailFactory.models;
using Models.DatabaseRelatedModels;
using System.Text.Encodings.Web;

namespace Services.EmailFactory.Tests
{
    [TestFixture]
    public class OrderEmailBuilderTests
    {
        private OrderEmailBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new OrderEmailBuilder();
        }

        #region Build

        [Test]
        public void Build_ShouldThrowArgumentNullException_WhenOrderDetailsIsNull()
        {
            // Arrange
            var input = new OrderEmailInput("Message", "USD", 0, null!, null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _builder.Build(input));
        }

        [Test]
        public void Build_ShouldIncludeMessage_WhenCalled()
        {
            // Arrange
            var input = CreateInput("Order received!");

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result, Does.Contain("Order received!"));
        }

        [Test]
        public void Build_ShouldIncludeEachProductName_WhenCalled()
        {
            // Arrange
            var input = CreateInput("Message");

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result, Does.Contain("Product A").And.Contain("Product B"));
        }

        [Test]
        public void Build_ShouldIncludeCorrectTotal_WhenCalled()
        {
            // Arrange
            var input = CreateInput("Message", shipping: 10); // subtotal = 2x10 + 1x20 = 40 → +10 = 50

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result, Does.Contain("Total: 50,00 USD"));
        }

        [Test]
        public void Build_ShouldIncludeEncodedTrackingLink_WhenTrackingLinkIsProvided()
        {
            // Arrange
            var rawLink = "https://track.com/abc?x=1&y=2";
            var encoded = HtmlEncoder.Default.Encode(rawLink);
            var input = CreateInput("Message", trackingLink: rawLink);

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result, Does.Contain(encoded));
        }

        [Test]
        public void Build_ShouldNotIncludeTrackingLink_WhenTrackingLinkIsNull()
        {
            // Arrange
            var input = CreateInput("Message", trackingLink: null);

            // Act
            var result = _builder.Build(input);

            // Assert
            Assert.That(result, Does.Not.Contain("Track Your Order"));
        }

        #endregion

        #region Helpers

        private OrderEmailInput CreateInput(string message, decimal shipping = 0, string? trackingLink = null)
        {
            return new OrderEmailInput(
                message,
                "USD",
                shipping,
                new List<OrderDetail>
                {
                    new OrderDetail { ProductName = "Product A", Quantity = 2, Price = 10 },
                    new OrderDetail { ProductName = "Product B", Quantity = 1, Price = 20 }
                },
                trackingLink
            );
        }

        #endregion
    }
}
