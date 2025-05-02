using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string FullName { get; set;  }
        public string ApplicationUserId { get; set; }
        public string OrderStatus { get; set; }
    }
}
