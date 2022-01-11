using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }        
        public ApplicationUser User { get; set; }
        public ICollection<Order> Orders { get; set; }
        public virtual ICollection<Line> Lines { get; set; }
        public Cart()
        {
            Orders = new List<Order>();
            Lines = new List<Line>();
        }        
    }    
}