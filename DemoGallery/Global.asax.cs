using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebGalleryProcessor;
using WebGalleryProcessor.Model;

namespace DemoGallery
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static GalleriesModel GalleriesModel { get; set; }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "GalleriesIndex",
                "Galleries",
                new
                {
                    controller = "Galleries",
                    action = "Index",
                }
            );

            routes.MapRoute(
                "GalleryDisplay",
                "Galleries/{name}",
                new
                {
                    controller = "Galleries",
                    action = "GetGallery",
                    name = UrlParameter.Optional,
                }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

#if DEBUG
            String baseHttpUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            if (HttpContext.Current.Request.Url.AbsolutePath != "/")
            {
                baseHttpUrl = baseHttpUrl.Replace(HttpContext.Current.Request.Url.AbsolutePath, String.Empty);
            }
            if (baseHttpUrl.EndsWith("/"))
            {
                baseHttpUrl = baseHttpUrl.Remove(baseHttpUrl.Length - 1);
            }
#else
            String baseHttpUrl = "http://MyUrl.com";
#endif

            GalleryModelBuilder modelBuilder = new GalleryModelBuilder(baseHttpUrl,
                                    "Web Gallery Processor - Demo",
                                    "/Content/Galleries/",
                                    HttpContext.Current.Server.MapPath("~/Content/"),
                                    HttpContext.Current.Server.MapPath("~/Content/Galleries/"));

            GalleriesModel = modelBuilder.BuildGalleriesModel();
        }
    }
}