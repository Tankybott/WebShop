using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseRelatedModels
{
    public class PhotoUrlSet
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        [Required]
        public string ThumbnailPhotoUrl { get; set; }

        [Required]
        public string BigPhotoUrl { get; set; }

        public bool IsMainPhoto { get; set; }
    }
}
