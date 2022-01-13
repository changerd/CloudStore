﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class ProductRequest
    {
        public int Id { get; set; }        
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}