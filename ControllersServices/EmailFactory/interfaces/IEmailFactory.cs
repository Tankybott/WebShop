using Models.DatabaseRelatedModels;
using Services.EmailFactory.models;


namespace Services.EmailFactory.interfaces
{
    public interface IEmailFactory
    {
        string BuildLinkEmail(string message, string link);
        string BuildOrderEmail(OrderHeader orderHeader, string message, string currency, decimal shippingPrice);
        string BuildInformationEmail(string message, IEnumerable<string> paragraphs);
    }
}
