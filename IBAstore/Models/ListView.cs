using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }

    }
    
}