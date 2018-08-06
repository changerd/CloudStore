using IBAstore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IBAstore.Controllers
{
    public class HomeController : Controller
    {
        StoreContext db = new StoreContext();
        public ActionResult Index()
        {
            List<Category> categories = new List<Category>();
            ViewBag.Categories = categories;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }       
        public string WhoAmI()
        {
            return User.Identity.Name.ToString() + "\n" + User.Identity.GetUserId() + "\n" + User.Identity.AuthenticationType;
        }
    }
}