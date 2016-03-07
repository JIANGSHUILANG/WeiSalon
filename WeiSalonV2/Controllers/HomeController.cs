using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BoboIService;
using BoboModel;

namespace WeiSalonV2.Controllers
{
    public class HomeController : Controller
    {
        readonly string _baseSalonurl = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/SalonWeb/index.html#salon/";
        public ActionResult Index()
        {
            SalonService salon = new SalonService();
            var info = new SalonSimple();
            if (Session["W_B_UID"] != null && Session["W_B_UID"].ToString() != "")
            {
                info = salon.GetSalonSimpleInfo(new Guid(Session["W_B_UID"].ToString()));
            }
            ViewBag._baseSalonurl = _baseSalonurl;
            return View(info);
        }
    }
}
