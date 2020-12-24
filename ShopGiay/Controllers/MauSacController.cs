using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;


namespace ShopGiay.Controllers
{
    public class MauSacController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: MauSac
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult DanhSachMau()
        {
            var listMau = db.MAUSACs.OrderBy(x => x.MaMau);
            return PartialView(listMau);
        }
    }
}