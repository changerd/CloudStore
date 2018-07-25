using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public int CartId { get; set; }
        public string UserId { get; set; }
        public int StatusOrderId { get; set; }
        public int DeliveryId { get; set; }
        public int PaymentMethodId { get; set; }
        public Cart Cart { get; set; }
        public ApplicationUser User { get; set; }
        public StatusOrder StatusOrder { get; set; }
        public Delivery Delivery { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}