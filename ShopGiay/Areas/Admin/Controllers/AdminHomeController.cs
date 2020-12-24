using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        // GET: Admin/AdminHome
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult SidebarMenu()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult HeaderStarts()
        {
            return PartialView();
        }
    }
}