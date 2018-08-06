using IBAstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace IBAstore.Controllers
{
    public class CatalogController : Controller
    {
        StoreContext db = new StoreContext();
        public int pagesize = 5;
        // GET: Items
        public ActionResult Index()
        {
            //var product = from p in db.Products where p.Categories.
            //var productt = db.Products.Include(c => c.Manufacturer).Include(c => c.StatusProduct).Include(c => c.Categories).ToList();
            //var products = productt.Where(c => c.Categories. == id).ToList();
            //var products = db.Products.Include(c => c.Categories.Where(p => p.Id == id)).ToList();
            //return View(products);
            List<Category> categories = db.Categories.ToList();
            return View(categories);
        }
        public ActionResult Items(int id, int page = 1)
        {
            var category = db.Categories.Find(id);
            string categoryname = category.Name;
            ViewBag.CategoryName = categoryname;            
            var products = db.Products.Include(c => c.Manufacturer).Include(c => c.StatusProduct).Include(c => c.Categories).Where(c => c.Categories.Any(cc => cc.Id == id)).ToList(); 
            ProductListViewModel model = new ProductListViewModel
            {
                Products = products.OrderBy(p => p.Id).Skip((page - 1) * pagesize).Take(pagesize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pagesize,
                    TotalItems = products.Count()
                }
            };
            return View(model);
        }
    }
}