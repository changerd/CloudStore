using IBAstore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;

namespace IBAstore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        StoreContext db = new StoreContext();

        public Cart GetCart()
        {
            string user = User.Identity.GetUserId();
            var cart = db.Carts.FirstOrDefault(c => c.UserId == user);       
            return cart;
        }
        public List<Line> GetLines()
        {
            int cartid = GetCart().Id;
            var list = db.Lines.Include(p => p.Product).Where(c => c.CartId == cartid).ToList();
            return list;
        }
        public RedirectToRouteResult AddToCart(int Id, string returnUrl, int quantity)
        {
            string strerror = null;
            Product product = db.Products.FirstOrDefault(p => p.Id == Id);
            if (product != null && product.Stock >= quantity)
            {
                var sline = db.Lines.Where(p => p.ProductId == product.Id).FirstOrDefault();
                if (sline == null)
                {
                    Line line = new Line
                    {
                        ProductId = product.Id,
                        Quantity = quantity
                    };
                    GetCart().Lines.Add(line);
                }
                else
                {
                    sline.Quantity += quantity;
                }
                product.Stock -= quantity;
                if (product.Stock < 1)
                {
                    var stp = db.StatusProducts.FirstOrDefault(s => s.StatusProductName == "Нет в наличии");
                    product.StatusProductId = stp.Id;
                }
                db.Entry(product).State = EntityState.Modified;
                //GetCart().Products.Add(product);                
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("Quantity", "Для добавления недостаточно количества на складе. Доступно: " + product.Stock);
                strerror = "Для добавления недостаточно количества на складе. Доступно: " + product.Stock;
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", new { returnUrl });
            }
            return RedirectToAction("DetailsItem", "Catalog", new { Id = Id, Err = strerror });
        }
        public RedirectToRouteResult RemoveFromCart(int Id, string returnUrl)
        {
            Line line = db.Lines.First(p => p.Id == Id);
            if (line != null)
            {
                Product product = db.Products.FirstOrDefault(p => p.Id == line.ProductId);
                product.Stock += line.Quantity;
                db.Lines.Remove(line);                
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { returnUrl });
        }
        public RedirectResult AddProductRequest(int Id, string returnUrl)
        {
            string userid = User.Identity.GetUserId();
            string prname = db.Products.Find(Id).ProductName;
            ProductRequest pr = new ProductRequest { ProductId = Id, UserId = userid };            
            db.ProductRequests.Add(pr);
            db.SaveChanges();
            TempData["message"] = string.Format("Заявка на {0} успешно принята!", prname);
            return Redirect(returnUrl);
        }
        // GET: Cart
        public ActionResult Index(string returnUrl)
        {            
            return View(new CartIndexViewModel
            {                
                Cart = GetCart(),
                Lines = GetLines(),
                ReturnUrl = returnUrl
            });
        }
        public PartialViewResult Summary()
        {
            return PartialView(GetCart());
        }
        public ActionResult Checkout()
        {
            SelectList paymentmethod = new SelectList(db.PaymentMethods, "Id", "PaymentMethodName");
            SelectList typedelivery = new SelectList(db.TypeDeliveries, "Id", "TypeDeliveryName");           
            decimal value = GetLines().Sum(s => s.Product.Cost * s.Quantity);
            ViewBag.Value = value;
            ViewBag.PaymentMethod = paymentmethod;
            ViewBag.TypeDelivery = typedelivery;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Checkout(Order order)
        {
            string orderdescription = null;
            int cartid = GetCart().Id;
            decimal value = GetLines().Sum(s => s.Product.Cost * s.Quantity);
            string userid = User.Identity.GetUserId();
            foreach (var cart in GetLines())
            {
                orderdescription += cart.Product.ProductName + " Цена " + cart.Product.Cost + " руб Количество  " + cart.Quantity + " штук." + "\n";
                for (int i = 0; i < cart.Quantity; i++)
                {
                    SaleStat stat = new SaleStat
                    {
                        ProductId = cart.Product.Id,
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
            var storder = db.StatusOrders.FirstOrDefault(s => s.StatusOrderName == "Принят");
            order.StatusOrderId = storder.Id;
            if (GetCart().Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Извините, ваша корзина пуста!");
            }
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.Lines.RemoveRange(GetLines());
                await db.SaveChangesAsync();
                return View("Completed");
            }
            else
            {
                return View(GetCart());
            }
        }

    }
}