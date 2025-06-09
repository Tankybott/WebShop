

using Models.DatabaseRelatedModels;

namespace Services.EmailFactory.models
{
    public record OrderEmailInput(string Message , string Currency, decimal ShippingPrice, IEnumerable<OrderDetail> OrderDetails, string TrackingLink);
}
