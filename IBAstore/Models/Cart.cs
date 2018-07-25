using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public Cart()
        {
            Orders = new List<Order>();
            Products = new List<Product>();
        }
    }
}