using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }        
        public byte[] Photo { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Cost")]
        public decimal Cost { get; set; }
        [Required]
        [Display(Name = "Stock")]
        public int Stock { get; set; }
        [Required]
        [Display(Name = "Manufacturer")]
        public int ManufacturerId { get; set; }
        [Required]
        [Display(Name = "Status Product")]
        public int StatusProductId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual StatusProduct StatusProduct { get; set; }        
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Line> Lines { get; set; }
        public virtual ICollection<SaleStat> SaleStats { get; set; } 
        public virtual ICollection<ProductRequest> ProductRequests { get; set; }
        public Product()
        {
            Categories = new List<Category>();
            Lines = new List<Line>();
            SaleStats = new List<SaleStat>();
            ProductRequests = new List<ProductRequest>();
        }

    }
}