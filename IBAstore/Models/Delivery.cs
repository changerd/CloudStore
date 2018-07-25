using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Housing { get; set; }
        public string Flat { get; set; }
        public string Telephone { get; set; }
        public ICollection<Order> Orders { get; set; }
        public Delivery()
        {
            Orders = new List<Order>();
        }
    }
}