using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Payment Method Name")]
        public string PaymentMethodName { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public PaymentMethod()
        {
            Orders = new List<Order>();
        }
        
    }
}