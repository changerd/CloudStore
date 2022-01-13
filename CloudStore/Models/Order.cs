using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Housing { get; set; }
        public string Flat { get; set; }
        public string Telephone { get; set; }
        public decimal Value { get; set; }
        public int CartId { get; set; }        
        public int StatusOrderId { get; set; }        
        public int PaymentMethodId { get; set; }
        public int TypeDeliveryId { get; set; }
        public virtual Cart Cart { get; set; }        
        public virtual StatusOrder StatusOrder { get; set; }       
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual TypeDelivery TypeDelivery { get; set; }
    }
}