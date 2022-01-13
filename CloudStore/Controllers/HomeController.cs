using CloudStore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudStore.Controllers
{
    public class HomeController : Controller
    {
        StoreContext db = new StoreContext();
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = string.Empty;
            ViewBag.Message = string.Empty;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = string.Empty;

            return View();
        }       
        public string WhoAmI()
        {
            return User.Identity.Name.ToString() + "\n" + User.Identity.GetUserId() + "\n" + User.Identity.AuthenticationType;
        }
    }
}