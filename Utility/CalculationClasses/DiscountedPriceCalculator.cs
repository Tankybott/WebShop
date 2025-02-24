using Models;

namespace Utility.CalculationClasses
{
    public static class DiscountedPriceCalculator
    {
        public static decimal CalculatePriceOfDiscount(decimal price, int discountPercentage) {
            return Math.Round(price * (1 - (decimal)discountPercentage / 100m), 2);
        }
    }
}
