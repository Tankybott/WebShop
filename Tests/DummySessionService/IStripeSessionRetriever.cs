using Stripe.Checkout;


namespace Tests.DummySessionService
{
    internal interface IStripeSessionRetriever
    {
        Session Get(string sessionId);
    }
}
