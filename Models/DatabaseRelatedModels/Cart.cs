using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Models.ContractInterfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Cart: IHasId
    {
        [Key]
        public int Id { get; set; }
        public DateTime? ExpiresTo { get; set; } = DateTime.Now;
        [ValidateNever]
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        [ForeignKey("User")]
        public string? UserId { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
