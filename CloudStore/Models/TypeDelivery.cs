using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class TypeDelivery
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Type delivery name")]
        public string TypeDeliveryName { get; set; }
        public ICollection<Order> Orders { get; set; }
        public TypeDelivery()
        {
            Orders = new List<Order>();
        }
    }
}