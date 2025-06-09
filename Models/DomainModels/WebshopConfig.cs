using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseRelatedModels
{
    public class WebshopConfig
    {
        [Key]
        public int Id { get; set; } = 1;
        [Required]
        public string SiteName { get; set; }
        [Required]
        public string Currency { get; set; }
        public decimal? FreeShippingFrom { get; set; }
    }
}
