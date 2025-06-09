using DataAccess.Repository.IRepository;
using Models.DatabaseRelatedModels;
using Services.OrderServices.Interfaces;
using Stripe;
using Stripe.Checkout;
using Utility.Common.Interfaces;

namespace Services.OrderServices
{
    public class StripePaymentManager : IStripePaymentManager
    {
        private readonly IOrderDetailsPriceCalculator _orderDetailsPriceCalculator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseUrlRetriever _baseUrlRetriver;

        public StripePaymentManager(IUnitOfWork unitOfowrk, IOrderDetailsPriceCalculator orderDetailsPriceCalculator, IBaseUrlRetriever baseUrlRetriver)
        {
            _unitOfWork = unitOfowrk;
            _orderDetailsPriceCalculator = orderDetailsPriceCalculator;
            _baseUrlRetriver = baseUrlRetriver;
        }

        public async Task MakeStripePaymentAsync(int orderHeaderId)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                h => h.Id == orderHeaderId,
                includeProperties: "OrderDetails,Carrier");

            if (orderHeader == null)
            {
                throw new Exception("Order not found");
            }

            await MakeStripePaymentAsync(orderHeader);
        }

        public async Task MakeStripePaymentAsync(OrderHeader orderHeader)
        {
            var webshopConfig = await _unitOfWork.WebshopConfig.GetAsync();
            var currency = webshopConfig.Currency;

            orderHeader.OrderDetails = await _unitOfWork.OrderDetail.GetAllAsync(d => d.OrderHeaderId == orderHeader.Id);
            var session = await CreateStripeSessionAsync(orderHeader, currency);

            orderHeader.SessionId = session.Id;
            orderHeader.PaymentLink = session.Url;
            _unitOfWork.OrderHeader.Update(orderHeader);
            await _unitOfWork.SaveAsync();
        }

        private async Task<Session> CreateStripeSessionAsync(OrderHeader orderHeader, string currency)
        {
            var lineItems = GetLineItemsForProducts(orderHeader.OrderDetails, currency);

            var shippingItem = await GetLineItemForShippingAsync(orderHeader, currency);
            if (shippingItem != null)
            {
                lineItems.Add(shippingItem);
            }

            var baseUrl = _baseUrlRetriver.GetBaseUrl();
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{baseUrl}/User/Order/OrderConfirmation?orderHeaderId={orderHeader.Id}",
                CancelUrl = "https://example.com/cancel",
                LineItems = lineItems,
                Mode = "payment"
            };

            try
            {
                var service = new SessionService();
                return service.Create(options);
            }
            catch (StripeException ex)
            {
                throw new StripeException("Stripe session creation failed: " + ex.StripeError?.Message, ex);
            }
        }


        private List<SessionLineItemOptions> GetLineItemsForProducts(IEnumerable<OrderDetail> orderDetails, string currency)
        {
            return orderDetails.Select(od => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = currency,
                    UnitAmountDecimal = od.Price * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = od.ProductName
                    }
                },
                Quantity = od.Quantity
            }).ToList();
        }

        private async Task<SessionLineItemOptions?> GetLineItemForShippingAsync(OrderHeader orderHeader, string currency)
        {
            if (orderHeader.Carrier == null) 
            {
                orderHeader.Carrier = await _unitOfWork.Carrier.GetAsync(c => c.Id == orderHeader.CarrierId);
            }
            var shippingPrice = _orderDetailsPriceCalculator.CalculateDetailsPrice(orderHeader.OrderDetails, orderHeader.Carrier);

            if (shippingPrice <= 0)
                return null;

            return new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = currency,
                    UnitAmountDecimal = shippingPrice * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Shipping"
                    }
                },
                Quantity = 1
            };
        }
    }
}
