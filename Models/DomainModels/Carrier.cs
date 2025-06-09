using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseRelatedModels
{
    public class Carrier
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsPricePerKg {  get; set; } 
        public decimal MinimalShippingPrice { get; set; }
        public decimal Price {  get; set; } 
    }
}
