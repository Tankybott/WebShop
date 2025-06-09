using NUnit.Framework;
using Models.DatabaseRelatedModels;
using Services.OrderServices;
using Services.OrderServices.Interfaces;

namespace ServicesTests.OrderServices
{
    [TestFixture]
    public class OrderDetailsPriceCalculatorTests
    {
        private IOrderDetailsPriceCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new OrderDetailsPriceCalculator();
        }

        #region CalculateDetailsPrice

        [Test]
        public void CalculateDetailsPrice_ShouldReturnFlatCarrierPrice_WhenIsPricePerKgIsFalse()
        {
            var carrier = new Carrier { IsPricePerKg = false, Price = 25m };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Quantity = 2, ShippingPriceFactor = 1 },
                new OrderDetail { Quantity = 1, ShippingPriceFactor = 2 }
            };

            var result = _calculator.CalculateDetailsPrice(orderDetails, carrier);

            Assert.That(result, Is.EqualTo(25m));
        }

        [Test]
        public void CalculateDetailsPrice_ShouldReturnTotalPrice_WhenAboveMinimumShipping()
        {
            var carrier = new Carrier { IsPricePerKg = true, Price = 2m, MinimalShippingPrice = 5m };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Quantity = 2, ShippingPriceFactor = 1 }, // 2 * 1 * 2 = 4
                new OrderDetail { Quantity = 1, ShippingPriceFactor = 2 }  // 1 * 2 * 2 = 4
            };

            var result = _calculator.CalculateDetailsPrice(orderDetails, carrier);

            Assert.That(result, Is.EqualTo(8m)); // 4 + 4 = 8 > 5
        }

        [Test]
        public void CalculateDetailsPrice_ShouldReturnMinimalShippingPrice_WhenBelowMinimumShipping()
        {
            var carrier = new Carrier { IsPricePerKg = true, Price = 2m, MinimalShippingPrice = 10m };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Quantity = 1, ShippingPriceFactor = 1 } // 1 * 1 * 2 = 2
            };

            var result = _calculator.CalculateDetailsPrice(orderDetails, carrier);

            Assert.That(result, Is.EqualTo(10m));
        }

        #endregion
    }
}
