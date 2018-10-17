using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IBAstore.Models
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public SelectList Manufacturers { get; set; }
    }
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public List<Line> Lines { get; set; }
        public string ReturnUrl { get; set; }        
    }
    
}