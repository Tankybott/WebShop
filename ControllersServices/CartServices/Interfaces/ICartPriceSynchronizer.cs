namespace Services.CartServices.Interfaces
{
    public interface ICartPriceSynchronizer
    {
        Task<IEnumerable<int>> Synchronize(int cartId);
    }
}