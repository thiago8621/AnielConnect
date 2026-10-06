using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aniel.Connect
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            /*
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
             */
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            
            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { 
                    culture = CultureHelper.GetDefaultCulture(), 
                    controller = "Account", 
                    action = "Index",
                    guid = "",
                    id = UrlParameter.Optional
                },
                // caso use Areas defina o namespace
                namespaces: new[] { "Aniel.Connect.Controllers" }
            );

            
            routes.Add("GuidRoute", 
                new GuidRoute(
                    "g/{guid}/{culture}/{controller}/{action}/{id}", 
                new { 
                    culture = CultureHelper.GetDefaultCulture(), 
                    controller = "", 
                    action = "",
                    guid = "", 
                    id = UrlParameter.Optional 
                },
                new[] { "Aniel.Connect.Controllers" }));
            
        }


    }
}
