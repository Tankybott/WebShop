using Stripe.Checkout;


namespace Tests.DummySessionService
{
    internal class StripeSessionRetriever : IStripeSessionRetriever
    {
        public Session Get(string sessionId)
        {
            var service = new SessionService();
            return service.Get(sessionId);
        }
    }

}
