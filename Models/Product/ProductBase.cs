using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DatabaseRelatedModels;

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
        [StringLength(500)]
        public string ShortDescription { get; set; }
        //require price higher than 0 
        [Required]
        public decimal Price { get; set; }
        public virtual int? DiscountId { get; set; }

        public string? FullDescription { get; set; }
        public virtual string? MainPhotoUrl { get; set; }
        public List<string>? OtherPhotosUrls { get; set; } = new List<string>();
    }
}

