using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Models
{
    public class Category
    {
        [Key] 
        public int Id { get; set; }

        [Required] 
        [MaxLength(100)] 
        public string Name { get; set; }

        [ForeignKey("ParentCategory")]
        public int? ParentCategoryId { get; set; }
        [ValidateNever]
        public Category? ParentCategory { get; set; }
        [ValidateNever]
        public  ICollection<Category> SubCategories { get; set; } = new List<Category>(); 
    }
}
