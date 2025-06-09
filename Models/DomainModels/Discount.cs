using Models.ContractInterfaces;
using System.ComponentModel.DataAnnotations;

namespace Models.DatabaseRelatedModels
{
    public class Discount : IHasId
    {
        [Key]
        public int Id { get; set; }
        [Range(1, 99)]
        public int Percentage { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }

        public bool isActive { get; set; } = false;

        public override bool Equals(object? obj)
        {
            if (obj is Discount other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
