using Models.DatabaseRelatedModels;
using Services.EmailFactory.interfaces;
using Services.EmailFactory.models;



namespace Services.EmailFactory
{

    public class EmailFactory : IEmailFactory
    {
        private readonly ILinkEmailBuilder _linkEmailBuilder;
        private readonly IInformationEmailBuilder _informationEmailBuilder;
        private readonly IOrderEmailBuilder _orderEmailBuilder;

        public EmailFactory()
        {
            _linkEmailBuilder = new LinkEmailBuilder();
            _informationEmailBuilder = new InformationEmailBuilder();
            _orderEmailBuilder = new OrderEmailBuilder();
        }

        public string BuildLinkEmail(string message, string link)
        {
            return _linkEmailBuilder.Build(new LinkEmailInput(message, link));
        }

        public string BuildInformationEmail(string message, IEnumerable<string> paragraphs)
        {
            return _informationEmailBuilder.Build(new InformationEmailInput(message, paragraphs));
        }

        public string BuildOrderEmail(OrderHeader orderHeader, string message, string currency, decimal shippingPrice)
        {
            var input = new OrderEmailInput(message, currency, shippingPrice, orderHeader.OrderDetails, orderHeader.TrackingLink);

            return _orderEmailBuilder.Build(input);
        }
    }
}