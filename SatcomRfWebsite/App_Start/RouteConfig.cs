using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SatcomRfWebsite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}",
               defaults: new { controller = "Home", action = "Index" }
           );

            /*routes.MapRoute(
                name: "Search",
                url: "{controller}/{action}/{prodT}/{modelN}",
                defaults: new { controller = "ateOutput", action = "Index", prodT = UrlParameter.Optional, modelN = UrlParameter.Optional }
            );
            */
            routes.MapRoute(
               name: "testResults",
               url: "{controller}/{action}/{prodT}/{modelN}/{filter}",
               defaults: new { controller = "testsData", action = "TestResults", prodT = UrlParameter.Optional, modelN = UrlParameter.Optional, filter = UrlParameter.Optional }
           );
        }
    }
}