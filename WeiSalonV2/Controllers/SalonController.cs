using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BoboIService;


namespace WeiSalonV2.Controllers
{
    public class SalonController : Controller
    {
        private readonly ISalonService _service;
        private readonly ISalonImage _service1;

        public SalonController()
        {
            _service = new SalonService();
            _service1 = new SalonImageInfo();
        }

        [System.Web.Http.HttpPost]
        public int SetStatus(int status, string uid)
        {
            return _service.UpdatestatusInfo(uid, status);

        }

        [System.Web.Http.HttpPost]
        public int UpdateImage(int id, string imagepath)
        {
            return _service1.UpdateImageInfo(id, imagepath);
        }
    }
}
