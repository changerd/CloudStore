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
        [Display(Name = "Status Product Name")]
        public string StatusProductName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public StatusProduct()
        {
            Products = new List<Product>();
        }
    }
}