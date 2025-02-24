namespace Services.CartServices.CustomExeptions
{
    public class DiscountOutOfDateException : Exception
    {
        public DiscountOutOfDateException()
            : base("The discount is out of date and cannot be applied.") { }

        public DiscountOutOfDateException(string message)
            : base(message) { }

        public DiscountOutOfDateException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
