using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class Manufacturer
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Manufacturer name")]
        public string ManufacturerName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public Manufacturer()
        {
            Products = new List<Product>();
        }
    }
}