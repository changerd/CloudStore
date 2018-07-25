using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class StatusProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
        public StatusProduct()
        {
            Products = new List<Product>();
        }
    }
}