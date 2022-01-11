using CloudStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CloudStore.Controllers
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
            user.Email=model.Email;
            user.FullName = model.FullName;
            user.Birth = model.Birth;
            user.PhoneNumber = model.PhoneNumber;
            user.GetNews = model.GetNews;
            string reg = ".+\\@.+\\..+";
            if (!string.IsNullOrEmpty(user.Email) && !Regex.IsMatch(user.Email, reg))
            {
                ModelState.AddModelError("Email", "Не корректная электронная почта.");
            }
            if (string.IsNullOrEmpty(user.Email) && user.GetNews == true)
            {
                ModelState.AddModelError("GetNews", "Чтобы получать новости, нужно ввести электронную почту.");
            }
            if (ModelState.IsValid)
            {
                IdentityResult result = await UserManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View(model);
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