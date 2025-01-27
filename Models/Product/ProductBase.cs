using System.ComponentModel.DataAnnotations;


namespace Models.ProductModel
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
        public decimal Price { get; set; }
        public virtual int? DiscountId { get; set; }
        public string? FullDescription { get; set; }
    }
}

