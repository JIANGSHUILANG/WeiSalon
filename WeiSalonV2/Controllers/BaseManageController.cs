using BoboModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeiSalonV2.Models;

namespace WeiSalonV2.Controllers
{
    public class BaseManageController : Controller
    {
        //
        // GET: /BaseManage/
        readonly string _adminCell = ConfigurationManager.AppSettings["AdminCell"];
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        public string ValAdmin()
        {

            if (_adminCell.Contains(manage.User.Cell))
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

    }
}
