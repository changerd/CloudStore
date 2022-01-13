using CloudStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace CloudStore.Controllers
{
    public class CatalogController : Controller
    {
        StoreContext db = new StoreContext();
        public int pagesize = 5;

        // GET: Items
        public async Task<ActionResult> Index()
        {            
            List<Category> categories = await db.Categories.ToListAsync();
            return View(categories);
        }

        public async Task<ActionResult> Items(int id, int? manufacturer, int page = 1)
        {
            var category = await db.Categories.FindAsync(id);
            string categoryname = category.CategoryName;

            ViewBag.CategoryName = categoryname;
            ViewBag.mid = manufacturer;

            IQueryable<Product> productsQuery = db.Products.Where(c => c.Categories.Any(cc => cc.Id == id));
                
            if (manufacturer != null && manufacturer != 0)
            {
                productsQuery = productsQuery.Where(c => c.ManufacturerId == manufacturer);
            }
            
            List<Product> products = await productsQuery.ToListAsync();
            
            var manufacturers = await db.Manufacturers.Where(m => m.Products.Any(p => p.Categories.Any(c => c.Id == id))).ToListAsync();
            manufacturers.Insert(0, new Manufacturer { ManufacturerName = "Все", Id = 0 });
            
            ProductListViewModel model = new ProductListViewModel
            {
                Products = products.Where(m => manufacturer == null || manufacturer == 0 || m.ManufacturerId == manufacturer).OrderBy(p => p.Id).Skip((page - 1) * pagesize).Take(pagesize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pagesize,
                    TotalItems = manufacturer == 0 || manufacturer == null ? products.Count() : await db.Products.Where(c => c.Categories.Any(cc => cc.Id == id)).Where(m => m.ManufacturerId == manufacturer).CountAsync()                    
                },
                Manufacturers = new SelectList(manufacturers, "Id", "ManufacturerName")                
            };
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SearchResult(string search)
        {
            ViewBag.SearchResult = search;
            var searchresult = await db.Products.Where(p=> p.ProductName.Contains(search)).ToListAsync();
            return View(searchresult);            
        }

        public async Task<ActionResult> DetailsItem(int id, string err = null)
        {            
            var product = await db.Products.FindAsync(id);
            ViewBag.Err = err;

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }        
    }
}