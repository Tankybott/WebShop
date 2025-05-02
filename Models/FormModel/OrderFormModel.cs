using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FormModel
{
    public class OrderFormModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAdress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int cartId { get; set; }
        public int carrierId { get; set; }
    }
}
