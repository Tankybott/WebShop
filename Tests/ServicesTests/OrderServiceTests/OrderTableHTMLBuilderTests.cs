using NUnit.Framework;
using Services.OrderServices;
using Models.DatabaseRelatedModels;
using System.Collections.Generic;
using NUnit.Framework.Legacy;

namespace ServicesTests.OrderServices
{
    [TestFixture]
    public class OrderTableHTMLBuilderTests
    {
        private OrderTableHTMLBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new OrderTableHTMLBuilder();
        }

        #region BuildHtml

        [Test]
        public void BuildHtml_ShouldIncludeOrderIdAndProductRows_WhenValidOrderIsProvided()
        {
            var order = new OrderHeader
            {
                Id = 42,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { ProductId = 1, ProductName = "Item A", Price = 9.99m, Quantity = 2 },
                    new OrderDetail { ProductId = 2, ProductName = "Item B", Price = 19.99m, Quantity = 1 }
                }
            };

            var result = _builder.BuildHtml(order);

            StringAssert.Contains("Order ID:</strong> 42", result);
        }

        [Test]
        public void BuildHtml_ShouldIncludeAllProductDetails_WhenOrderDetailsArePresent()
        {
            var order = new OrderHeader
            {
                Id = 7,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { ProductId = 100, ProductName = "Test Product", Price = 15.5m, Quantity = 3 }
                }
            };

            var result = _builder.BuildHtml(order);

            StringAssert.Contains("<td>100</td>", result);
            StringAssert.Contains("<td>Test Product</td>", result);
            StringAssert.Contains("<td>3</td>", result);
        }

        #endregion
    }
}
