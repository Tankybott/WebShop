using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DatabaseRelatedModels
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        [Required]
        public decimal ShippingPriceFactor { get; set; }
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [ForeignKey("OrderHeader")]
        public int OrderHeaderId { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

    }
}
