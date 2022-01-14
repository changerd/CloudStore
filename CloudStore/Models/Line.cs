using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class Line
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }        
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Cart Cart { get; set; }
    }
}