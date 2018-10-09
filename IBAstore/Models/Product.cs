using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название товара")]
        public string Name { get; set; }        
        public byte[] Photo { get; set; }
        [Required]
        [Display(Name = "Описание товара")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Цена товара")]
        public decimal Cost { get; set; }
        [Required]
        [Display(Name = "Производитель товара")]
        public int ManufacturerId { get; set; }
        [Required]
        [Display(Name = "Статус товара")]
        public int StatusProductId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public StatusProduct StatusProduct { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<SaleStat> SaleStats { get; set; } 
        public virtual ICollection<ProductRequest> ProductRequests { get; set; }
        public Product()
        {
            Categories = new List<Category>();
            Carts = new List<Cart>();
            SaleStats = new List<SaleStat>();
            ProductRequests = new List<ProductRequest>();
        }

    }
}