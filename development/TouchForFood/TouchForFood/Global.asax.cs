using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TouchForFood.Util.Search;

namespace TouchForFood
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Order", // Route name
                "Order/{id}", // URL with parameters
                new { controller = "Order", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "Table/ManageIndex", // Route name
                "Table/ManageIndex/{id}", // URL with parameters
                new { controller = "Table", action = "ManageIndex", id = UrlParameter.Optional }
            ); 
            routes.MapRoute(
                 "Review/Create", // Route name
                 "Review/Create/{id}", // URL with parameters
                 new { controller = "Review", action = "Create" }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // Load search data into memory
            SearchUtil su = new SearchUtil();
            su.ClearAndFill();
        }
    }
}