using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models.DatabaseRelatedModels;

namespace Models.FormModel
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
        public string? MainPhotoUrl { get; set; }
        public List<string> OtherPhotosUrls { get; set; } = new List<string>();
    }
}
