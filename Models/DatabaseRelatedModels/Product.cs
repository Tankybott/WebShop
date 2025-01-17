
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models.DatabaseRelatedModels;
using Models.ProductModel;

namespace Models
{
    public class Product : ProductBase
    {
        [Key]
        public override int Id { get; set; }

        [Required]
        [ForeignKey("Category")]
        public override int CategoryId { get; set; } 

        [ValidateNever]
        public Category Category { get; set; }

        [ForeignKey("Discount")]
        public override int? DiscountId { get; set; }
        [ValidateNever]
        public Discount? Discount { get; set; }
        [ValidateNever]
        public List<PhotoUrlSet>? PhotosUrlSets { get; set; } = new List<PhotoUrlSet>();
    }
}