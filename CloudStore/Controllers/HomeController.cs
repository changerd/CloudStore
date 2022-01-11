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
            //var photo1 = db.Products.Find(3010).Photo;
            //var photo2 = db.Products.Find(2017).Photo;
            //var photo3 = db.Products.Find(3011).Photo;
            //ViewBag.FirstPhoto = photo1;
            //ViewBag.SecondPhoto = photo2;
            //ViewBag.ThirdPhoto = photo3;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "CloudStore";
            ViewBag.Message = "Данный проект является тестовым заданием для стажировки.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "IBA.";

            return View();
        }       
        public string WhoAmI()
        {
            return User.Identity.Name.ToString() + "\n" + User.Identity.GetUserId() + "\n" + User.Identity.AuthenticationType;
        }
    }
}