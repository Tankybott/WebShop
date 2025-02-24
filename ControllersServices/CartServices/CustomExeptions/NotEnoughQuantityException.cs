namespace Services.CartServices.CustomExeptions
{
    public class NotEnoughQuantityException : Exception
    {
        public int MaxAvailableQuantity { get; }

        public NotEnoughQuantityException(int maxAvailableQuantity)
            : base("The discount is out of date and cannot be applied.") 
        {
            MaxAvailableQuantity = maxAvailableQuantity;
        }

        public NotEnoughQuantityException(string message, int maxAvailableQuantity)
            : base(message)
        {
            MaxAvailableQuantity = maxAvailableQuantity;
        }

        public NotEnoughQuantityException(string message, Exception innerException, int maxAvailableQuantity)
            : base(message, innerException)
        {
            MaxAvailableQuantity = maxAvailableQuantity;
        }
    }
}
