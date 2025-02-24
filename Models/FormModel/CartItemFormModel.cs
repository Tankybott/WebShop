using System.ComponentModel.DataAnnotations;

namespace Models.FormModel
{
    public class CartItemFormModel
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be greater than 0.")]
        public int ProductQuantity { get; set; }
        public bool IsAddedWithDiscount { get; set; }
    }
}
