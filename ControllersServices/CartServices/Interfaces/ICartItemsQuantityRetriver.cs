namespace Services.CartServices.Interfaces
{
    public interface ICartItemsQuantityRetriver
    {
        Task<int> GetItemsQantityAsync();
    }
}