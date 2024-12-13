using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models.DatabaseRelatedModels;

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

        [Required]
        [ForeignKey("Discount")]
        public override int? DiscountId { get; set; }
        [ValidateNever]
        public Discount? Discount { get; set; }

        [Required]
        public override string MainPhotoUrl { get; set; }

    }
}