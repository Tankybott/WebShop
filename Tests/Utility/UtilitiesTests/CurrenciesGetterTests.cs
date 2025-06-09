using NUnit.Framework;
using System.Collections.Generic;
using Utility.Common;

namespace Utility.Common.Tests
{
    [TestFixture]
    public class CurrenciesGetterTests
    {
        #region GetAllCurrencies

        [Test]
        public void GetAllCurrencies_ShouldReturnDictionary_WhenCalled()
        {
            // Act
            var result = CurrenciesGetter.GetAllCurrencies();

            // Assert
            Assert.That(result, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void GetAllCurrencies_ShouldContainUSD_WhenCalled()
        {
            // Act
            var result = CurrenciesGetter.GetAllCurrencies();

            // Assert
            Assert.That(result.ContainsKey("USD"), Is.True);
        }

        [Test]
        public void GetAllCurrencies_ShouldHaveUniqueCurrencySymbols_WhenCalled()
        {
            // Act
            var result = CurrenciesGetter.GetAllCurrencies();

            // Assert
            var uniqueKeys = new HashSet<string>(result.Keys);
            Assert.That(uniqueKeys.Count, Is.EqualTo(result.Count));
        }

        #endregion
    }
}
