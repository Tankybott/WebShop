using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DatabaseRelatedModels
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime ShippingDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PaymentDate { get; set; }

        public string? OrderStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingLink { get; set; }
        public int? CarrierId { get; set; }
        [ForeignKey("CarrierId")]
        [ValidateNever]
        public Carrier? Carrier { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? PaymentLink { get; set; }

        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? StreetAdress { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        [ValidateNever]
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
