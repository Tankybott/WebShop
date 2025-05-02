

using System.Globalization;

namespace Utility.Common
{
    public static class CurrenciesGetter
    {
        public static IReadOnlyDictionary<string, string> GetAllCurrencies()
        {
            var currencies = new Dictionary<string, string>();

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                try
                {
                    var region = new RegionInfo(culture.Name);
                    if (!currencies.ContainsKey(region.ISOCurrencySymbol))
                    {
                        currencies.Add(region.ISOCurrencySymbol, region.CurrencyEnglishName);
                    }
                }
                catch
                {
                    // Ignore cultures without region info
                }
            }

            return currencies;
        }
    }
}
