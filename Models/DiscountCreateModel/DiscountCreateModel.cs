namespace Models.DiscountCreateModel
{
    public class DiscountCreateModel
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Percentage { get; set; }
        public bool? IsDiscountChanged { get; set; }
        public int? DiscountId { get; set; }
    }
}
