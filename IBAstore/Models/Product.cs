using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Photo { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public int ManufacturerId { get; set; }
        public int StatusProductId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public StatusProduct StatusProduct { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public Product()
        {
            Categories = new List<Category>();
            Carts = new List<Cart>();
        }

    }
}