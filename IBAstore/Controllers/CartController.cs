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
    public class CartController : Controller
    {
        StoreContext db = new StoreContext();

        public Cart GetCart()
        {
            string user = User.Identity.GetUserId();
            var cart = db.Carts.FirstOrDefault(c => c.UserId == user);
            return cart;
        }
        public RedirectToRouteResult AddToCart(int Id, string returnUrl)
        {            
            Product product = db.Products.FirstOrDefault(p => p.Id == Id);
            if (product != null)
            {
                GetCart().Products.Add(product);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { returnUrl });
        }
        public RedirectToRouteResult RemoveFromCart(int Id, string returnUrl)
        {
            Product product = db.Products.First(p => p.Id == Id);
            if (product != null)
            {
                GetCart().Products.Remove(product);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { returnUrl });
        }
        // GET: Cart
        public ActionResult Index(string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = GetCart(),
                ReturnUrl = returnUrl
            });
        }
        public PartialViewResult Summary()
        {
            return PartialView(GetCart());
        }
        public ActionResult Checkout()
        {
            SelectList paymentmethod = new SelectList(db.PaymentMethods, "Id", "Name");
            SelectList typedelivery = new SelectList(db.TypeDeliveries, "Id", "Name");            
            //ViewBag.CartId = cartid;
            //ViewBag.Description = orderdescription;
            decimal value = GetCart().ComputeTotalValue();
            ViewBag.Value = value;
            ViewBag.PaymentMethod = paymentmethod;
            ViewBag.TypeDelivery = typedelivery;
            return View(/*new Order
            {
                Date = DateTime.Now,
                CartId = cartid,
                Description = orderdescription,
                Value = value,
                StatusOrderId = 2
            }*/);
        }
        [HttpPost]
        public async Task<ActionResult> Checkout(Order order)
        {
            string orderdescription = null;
            int cartid = GetCart().Id;
            decimal value = GetCart().ComputeTotalValue();
            foreach (var cart in GetCart().Products)
            {
                orderdescription += cart.Name + " " + "Цена " + cart.Cost + " руб" + "\n";
            }
            order.Date = DateTime.Now;
            order.CartId = cartid;
            order.Description = orderdescription;
            order.Value = value;
            order.StatusOrderId = 2;
            if (GetCart().Products.Count() == 0)
            {
                ModelState.AddModelError("", "Извините, ваша корзина пуста!");
            }
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                GetCart().Products.Clear();
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