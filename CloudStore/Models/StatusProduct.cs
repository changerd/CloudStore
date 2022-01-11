using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class StatusProduct
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название статуса продукта")]
        public string StatusProductName { get; set; }
        public ICollection<Product> Products { get; set; }
        public StatusProduct()
        {
            Products = new List<Product>();
        }
    }
}