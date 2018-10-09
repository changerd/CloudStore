using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class Manufacturer
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название производителя")]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
        public Manufacturer()
        {
            Products = new List<Product>();
        }
    }
}