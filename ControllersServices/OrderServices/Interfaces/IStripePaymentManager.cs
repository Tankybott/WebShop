using Models.DatabaseRelatedModels;

namespace Services.OrderServices.Interfaces
{
    public interface IStripePaymentManager
    {
        Task MakeStripePaymentAsync(int orderHeaderId);
        Task MakeStripePaymentAsync(OrderHeader orderHeader);
    }
}