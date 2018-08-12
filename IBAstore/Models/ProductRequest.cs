using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class ProductRequest
    {
        public int Id { get; set; }        
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}