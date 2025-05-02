namespace Services.WebshopConfigServices.Interfaces
{
    public interface IFreeShippingThresholdManager
    {
        Task<decimal?> GetFreeShippingThresholdAsync();
        Task UpdateFreeShippingThresholdAsync(decimal? price);
    }
}