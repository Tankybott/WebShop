namespace Services.CartServices.Interfaces
{
    public interface ICartItemRemover
    {
        Task RemoveAsync(int cartItemId);
    }
}