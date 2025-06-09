namespace Services.OrderServices.Interfaces
{
    public interface IOrderSuccessPaymentProcessor
    {
        Task<bool> ProcessAsync(int orderHeaderId);
    }
}