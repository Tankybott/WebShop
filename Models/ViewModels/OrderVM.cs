
using Models.DatabaseRelatedModels;

namespace Models.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
