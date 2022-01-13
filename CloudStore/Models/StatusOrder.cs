using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class StatusOrder
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название статуса заказа")]
        public string StatusOrderName { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public StatusOrder()
        {
            Orders = new List<Order>();
        }
    }
}