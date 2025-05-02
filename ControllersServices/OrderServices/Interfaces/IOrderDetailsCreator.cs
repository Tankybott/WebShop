namespace Services.OrderServices.Interfaces
{
    public interface IOrderDetailsCreator
    {
        Task CreateDetailsAsync(int cartId, int orderHeaderId);
    }
}