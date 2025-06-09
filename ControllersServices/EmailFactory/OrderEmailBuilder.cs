using System.Text.Encodings.Web;
using Services.EmailFactory.interfaces;
using Models.DatabaseRelatedModels;
using Services.EmailFactory.models;

namespace Services.EmailFactory
{
    public class OrderEmailBuilder : IOrderEmailBuilder
    {
        public string Build(OrderEmailInput input)
        {
            if (input.OrderDetails == null)
                throw new ArgumentNullException(nameof(input.OrderDetails), "OrderDetails cannot be null when building the order email.");

            var itemRows = input.OrderDetails.Select(item =>
            {
                var subtotal = item.Price * item.Quantity;
                return $@"
                <p style='font-size: 16px; color: #495057; margin-bottom: 10px;'>
                    <strong>{item.ProductName}</strong><br />
                    Quantity: {item.Quantity}<br />
                    Unit Price: {item.Price:0.00} {input.Currency}<br />
                    Subtotal: {subtotal:0.00} {input.Currency}
                </p><br />";
            });

            var totalWithoutShipping = input.OrderDetails.Sum(i => i.Price * i.Quantity);
            var grandTotal = totalWithoutShipping + input.ShippingPrice;

            var encodedTrackingLink = string.IsNullOrEmpty(input.TrackingLink)
                ? null
                : HtmlEncoder.Default.Encode(input.TrackingLink);

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px;'>
                    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                        <h2 style='color: #343a40;'>Your Order Summary</h2>
                        <p style='font-size: 16px; color: #495057;'>{input.Message}</p>
                        {string.Join("\n", itemRows)}
                        <hr style='border: none; border-top: 1px solid #dee2e6; margin: 20px 0;' />
                        <p style='font-size: 16px; color: #495057;'><strong>Shipping:</strong> {input.ShippingPrice:0.00} {input.Currency}</p>
                        <p style='font-size: 18px; font-weight: bold; color: #212529;'>Total: {grandTotal:0.00} {input.Currency}</p>
                        {(encodedTrackingLink != null
                            ? $@"<p style='text-align: center; margin: 30px 0;'>
                                    <a href='{encodedTrackingLink}'
                                       style='background-color: #007bff; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold;'>
                                       Track Your Order
                                    </a>
                                </p>"
                            : "")}
                        <p style='font-size: 14px; color: #868e96;'>Thank you for shopping with us!</p>
                    </div>
                </body>
                </html>";
        }
    }
}
