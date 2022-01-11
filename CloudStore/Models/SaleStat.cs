﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class SaleStat
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}