using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class StatusOrder
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название статуса заказа")]
        public string Name { get; set; }
        public ICollection<Order> Orders { get; set; }
        public StatusOrder()
        {
            Orders = new List<Order>();
        }
    }
}