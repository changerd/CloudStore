using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название способа оплаты")]
        public string PaymentMethodName { get; set; }
        public ICollection<Order> Orders { get; set; }
        public PaymentMethod()
        {
            Orders = new List<Order>();
        }
        
    }
}