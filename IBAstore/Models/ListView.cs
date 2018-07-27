using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class ProductView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManufacturerName { get; set; }
        public byte[] Photo { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string StatusProductName { get; set; }
    }
}