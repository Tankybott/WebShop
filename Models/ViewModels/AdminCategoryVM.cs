using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AdminCategoryVM
    {
        public Category Category { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryListItems { get; set; }
        [ValidateNever]
        public IEnumerable<Category> AllCategories { get; set; }
        public string? BindedParentName;
    }
}
