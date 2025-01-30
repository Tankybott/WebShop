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
        public Product Product { get; set; }
        [Required]
        public decimal PriceWhenAdded { get; set; }
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [ValidateNever]
        public Cart Cart { get; set; }
    }
}
