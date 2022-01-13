using CloudStore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CloudStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        StoreContext db = new StoreContext();

        const string SPNotAviable = "Нет в наличии";
        const string SOAccepted = "Принят";

        public async Task<Cart> GetCart()
        {
            string user = User.Identity.GetUserId();
            var cart = await db.Carts.FirstOrDefaultAsync(c => String.Equals(c.UserId, user));       
            return cart;
        }

        public async Task<List<Line>> GetLines()
        {
            Cart cart = await GetCart();
            int cartid = cart.Id;
            var list = await db.Lines.Include(p => p.Product).Where(c => c.CartId == cartid).ToListAsync();
            return list;
        }

        public async Task<RedirectToRouteResult> AddToCart(int Id, string returnUrl, int quantity)
        {
            string strerror = null;            
            Product product = await db.Products.FirstOrDefaultAsync(p => p.Id == Id);
            Cart cart = await GetCart();
            
            if (product != null && product.Stock >= quantity)
            {
                var sline = await db.Lines.Where(p => p.ProductId == product.Id).FirstOrDefaultAsync();
                
                if (sline == null)
                {
                    Line line = new Line
                    {
                        ProductId = product.Id,
                        Quantity = quantity
                    };
                   
                    cart.Lines.Add(line);
                }
                else
                {
                    sline.Quantity += quantity;
                }
                
                product.Stock -= quantity;
                
                if (product.Stock < 1)
                {
                    var stp = db.StatusProducts.FirstOrDefault(s => String.Equals(s.StatusProductName, SPNotAviable));
                    product.StatusProductId = stp.Id;
                }

                db.Entry(product).State = EntityState.Modified;                
                               
                await db.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("Quantity", "Для добавления недостаточно количества на складе. Доступно: " + product.Stock);
                strerror = String.Format("Для добавления недостаточно количества на складе. Доступно: {0}", product.Stock);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", new { returnUrl });
            }

            return RedirectToAction("DetailsItem", "Catalog", new { Id = Id, Err = strerror });
        }

        public async Task<RedirectToRouteResult> RemoveFromCart(int Id, string returnUrl)
        {
            Line line = await db.Lines.FindAsync(Id);
            
            if (line != null)
            {
                Product product = await db.Products.FindAsync(line.ProductId);
                product.Stock += line.Quantity;
                db.Lines.Remove(line);                
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public async Task<RedirectResult> AddProductRequest(int Id, string returnUrl)
        {
            string userid = User.Identity.GetUserId();

            Product product = await db.Products.FindAsync(Id);
            string prname = product.ProductName;

            ProductRequest pr = new ProductRequest { ProductId = Id, UserId = userid };            
            db.ProductRequests.Add(pr);
            
            await db.SaveChangesAsync();

            TempData["message"] = string.Format("Заявка на {0} успешно принята!", prname);
            return Redirect(returnUrl);
        }

        // GET: Cart
        public async Task<ActionResult> Index(string returnUrl)
        {            
            return View(new CartIndexViewModel
            {                
                Cart = await GetCart(),
                Lines = await GetLines(),
                ReturnUrl = returnUrl
            });
        }
        public async Task<PartialViewResult> Summary()
        {
            Cart cart = await GetCart();
            int summary = 0;
            if (cart != null)
            {
                summary = cart.Lines.Sum(s => s.Quantity);
            }

            return PartialView(summary);
        }

        public async Task<ActionResult> Checkout()
        {
            SelectList paymentmethod = new SelectList(await db.PaymentMethods.ToListAsync(), "Id", "PaymentMethodName");
            SelectList typedelivery = new SelectList(await db.TypeDeliveries.ToListAsync(), "Id", "TypeDeliveryName");
            
            var lines = await GetLines();
            decimal value = lines.Sum(s => s.Product.Cost * s.Quantity);
           
            ViewBag.Value = value;
            ViewBag.PaymentMethod = paymentmethod;
            ViewBag.TypeDelivery = typedelivery;
            
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Checkout(Order order)
        {
            string orderdescription = string.Empty;

            Cart cart = await GetCart();

            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Извините, ваша корзина пуста!");
            }

            int cartid = cart.Id;

            var lines = await GetLines();
            decimal value = lines.Sum(s => s.Product.Cost * s.Quantity);
            string userid = User.Identity.GetUserId();

            foreach (var line in lines)
            {
                orderdescription += String.Format("{0} Цена {1} руб Количество {2} штук. {3} ", line.Product.ProductName, line.Product.Cost, line.Quantity, String.Empty);
                for (int i = 0; i < line.Quantity; i++)
                {
                    SaleStat stat = new SaleStat
                    {
                        ProductId = line.Product.Id,
                        UserId = userid,
                        Date = DateTime.Now
                    };
                    db.SaleStats.Add(stat);
                }                
            }

            order.Date = DateTime.Now;
            order.CartId = cartid;
            order.Description = orderdescription;
            order.Value = value;
            var storder = db.StatusOrders.FirstOrDefault(s => String.Equals(s.StatusOrderName, SOAccepted));
            order.StatusOrderId = storder.Id;
            
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.Lines.RemoveRange(lines);
                await db.SaveChangesAsync();
                return View("Completed");
            }
            else
            {
                return View(cart);
            }
        }

    }
}