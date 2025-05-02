
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.DatabaseRelatedModels;

namespace Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader? OrderHeader { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
        public IEnumerable<CartItem>? OrderDetailsAsCartItems { get; set; }
        public IEnumerable<SelectListItem> CarrierListItems { get; set; }
        public string Currency { get; set; }
        public bool? IsPricePerKg { get; set; }
    }
}
