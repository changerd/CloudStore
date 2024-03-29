﻿using CloudStore.Infastructure;
using CloudStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CloudStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new DBInitializer());
            StoreContext context = new StoreContext();
            context.Database.Initialize(true);
            AreaRegistration.RegisterAllAreas();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);            
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var culture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.NumberFormat.CurrencyGroupSeparator = ".";
            culture.NumberFormat.PercentDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
