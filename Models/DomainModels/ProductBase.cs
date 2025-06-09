using System.ComponentModel.DataAnnotations;


namespace Models.DatabaseRelatedModels
{
    public class ProductBase
    {
        [Required]
        public virtual int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public virtual int CategoryId { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        public virtual int? DiscountId { get; set; }
        public string? FullDescription { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Shipping Price Factor must be greater than 0.")]
        public decimal ShippingPriceFactor { get; set; }
    }
}

