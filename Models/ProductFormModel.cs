using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ProductFormModel : ProductBase
    {
        [ValidateNever]
        public IFormFile MainPhoto { get; set; }
        [ValidateNever]
        public List<IFormFile>? OtherPhotos { get; set; } = new List<IFormFile>();

        [ValidateNever]
        public List<string> UrlsToDelete { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public bool? IsDisocuntChanged { get; set; }
    }
}
