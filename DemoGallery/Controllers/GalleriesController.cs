using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGalleryProcessor.Model;

namespace DemoGallery.Controllers
{
    public class GalleriesController : Controller
    {
        public ActionResult Index()
        {
            return View(MvcApplication.GalleriesModel);
        }

        public ActionResult GetGallery(String name)
        {
            var gallery = from g in MvcApplication.GalleriesModel.Galleries
                          where String.Equals(g.UrlFriendlyName, name, StringComparison.OrdinalIgnoreCase)
                          select g;

            Gallery requiredGallery = gallery.FirstOrDefault();

            if (requiredGallery != null)
            {
                return View("GalleryDisplay", requiredGallery);
            }
            else
            {
                return View("Index", MvcApplication.GalleriesModel);

            }
        }
    }
}
