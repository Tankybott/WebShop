using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be greater than 0.")]
        public int ProductQuantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price when added must be greater than 0.")]
        public decimal CurrentPrice { get; set; }
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [ValidateNever]
        public Cart Cart { get; set; }
        public bool IsAddedWithDiscount { get; set; }
    }
}
