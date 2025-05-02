using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class WebshopSettingsVm
    {
        [Required]
        public string SiteName { get; set; }
        [Required]
        public string Currency { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Currencies { get; set; } 
    }
}
