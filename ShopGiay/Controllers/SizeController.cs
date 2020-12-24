using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;

namespace ShopGiay.Controllers
{
    public class SizeController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Size
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult DanhSachSize()
        {
            var listSize = db.SIZEs.OrderBy(x => x.MaSize);
            return PartialView(listSize);
        }
    }
}