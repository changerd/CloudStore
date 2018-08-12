using IBAstore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IBAstore.Controllers
{
    public class ManageController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        StoreContext db = new StoreContext();
        // GET: Menage
        public ActionResult Index()
        {
            string userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        public ActionResult Edit(string id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ApplicationUser model)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(model.Id);
            user.Name = model.Name;
            user.Birth = model.Birth;
            user.PhoneNumber = model.PhoneNumber;
            user.GetNews = model.GetNews;
            IdentityResult result = await UserManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
        public ActionResult DetailOrder(int id)
        {
            var orders = db.Orders.Include(o => o.StatusOrder).Include(o => o.TypeDelivery).Include(o => o.PaymentMethod).ToList();
            var order = orders.Find(i => i.Id == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
    }
}