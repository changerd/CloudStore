using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CloudStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(null,
            //    "",
            //    new
            //    {
            //        controller = "Catalog",
            //        action = "Items",
            //        category = (string)null,
            //        page = 1
            //    }
            //);

            //routes.MapRoute(
            //    name: null,
            //    url: "{id}/Page{page}",
            //    defaults: new { controller = "Catalog", action = "Items", manufacturer = (string)null },
            //    constraints: new { page = @"\d+" }
            //);

            //routes.MapRoute(null,
            //    "{id}/{manufacturer}",
            //    new { controller = "Catalog", action = "Items" }
            //);

            //routes.MapRoute(null,
            //    "{id}/{manufacturer}/Page{page}",
            //    new { controller = "Catalog", action = "Items" },
            //    new { page = @"\d+" }
            //);

            //routes.MapRoute(null, "{controller}/{action}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
