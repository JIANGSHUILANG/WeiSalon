using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WeiSalonV2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "saloninfo",
              url: "manage/saloninfo/{uid}",
              defaults: new { controller = "manage", action = "saloninfo", uid = UrlParameter.Optional }
          );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{pageIndex}",
                defaults: new { controller = "Home", action = "Index", pageIndex = UrlParameter.Optional }
            );


        }
    }
}