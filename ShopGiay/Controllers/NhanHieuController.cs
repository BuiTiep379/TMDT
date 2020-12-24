using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;


namespace ShopGiay.Controllers
{
    public class NhanHieuController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();

        // GET: NhanHieu
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult DanhSachNhanHieu()
        {
            var nhanHieu = db.NHANHIEUx.OrderBy(x => x.MaNhanHieu);
            return PartialView(nhanHieu);
        }
    }
}