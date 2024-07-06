using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MSLA.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
 
            config.Routes.MapHttpRoute(
              name: "BOApi",
              routeTemplate: "api/{controller}/{DocType}/{Doc_ID}"//,
                //defaults: new { DocMaster = RouteParameter.Optional, id = RouteParameter.Optional }
          ); 

           // config.Routes.MapHttpRoute(
           //    name: "QueryApi",
           //    routeTemplate: "api/{controller}/{action}/{id}",
           //    defaults: new { id = RouteParameter.Optional }
           //); 

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            ); 
        }
    }
}
