using IBAstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace IBAstore.Controllers
{
    public class CatalogController : Controller
    {
        StoreContext db = new StoreContext();
        public Cart GetCart()
        {
            string user = User.Identity.GetUserId();
            var cart = db.Carts.FirstOrDefault(c => c.UserId == user);
            return cart;
        }
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
        public ActionResult Items(int id, int? manufacturer, int page = 1)
        {
            var category = db.Categories.Find(id);
            string categoryname = category.CategoryName;
            ViewBag.CategoryName = categoryname;
            ViewBag.mid = manufacturer;
            var product = db.Products
                .Include(c => c.Manufacturer)
                .Include(c => c.StatusProduct)
                .Include(c => c.Categories)
                .Where(c => c.Categories.Any(cc => cc.Id == id));
                //.Where(c => manufacturer == null || c.Manufacturer.Name == manufacturer)
                //.ToList();
            if (manufacturer != null && manufacturer != 0)
            {
                product = product.Where(c => c.ManufacturerId == manufacturer);
            }
            List<Product> products = product.ToList();
            var manufacturers = db.Manufacturers.Where(m => m.Products.Any(p => p.Categories.Any(c => c.Id == id))).ToList();
            manufacturers.Insert(0, new Manufacturer { ManufacturerName = "Все", Id = 0 });
            ProductListViewModel model = new ProductListViewModel
            {
                Products = products.Where(m => manufacturer == null || manufacturer == 0 || m.ManufacturerId == manufacturer).OrderBy(p => p.Id).Skip((page - 1) * pagesize).Take(pagesize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pagesize,
                    TotalItems = manufacturer == 0 || manufacturer == null ? products.Count() : db.Products.Where(c => c.Categories.Any(cc => cc.Id == id)).Where(m => m.ManufacturerId == manufacturer).Count()                    
                },
                Manufacturers = new SelectList(manufacturers, "Id", "ManufacturerName")
                
            };
            return View(model);
        }
        [HttpGet]
        public ActionResult SearchResult(string search)
        {
            ViewBag.SearchResult = search;
            var searchresult = db.Products.Where(p=> p.ProductName.Contains(search)).Include(c => c.Manufacturer).Include(c => c.StatusProduct).Include(c => c.Categories).ToList();
            return View(searchresult);            
        }
        public ActionResult DetailsItem(int id, string err = null)
        {            
            var productt = db.Products.Include(c => c.Manufacturer).Include(c => c.StatusProduct).Include(c => c.Categories).ToList();
            var product = productt.Find(i => i.Id == id);
            ViewBag.Err = err;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }        
    }
}