namespace Services.OrderServices.Interfaces
{
    public interface IOrderStatusManager
    {
        Task SendAsync(int orderHeaderId);
        Task StartProcessingAsync(int orderHeaderId);
    }
}