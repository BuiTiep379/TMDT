using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;


namespace ShopGiay.Controllers
{
    public class LoaiController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Loai
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult DanhSachLoai()
        {
            var loaiSP = db.LOAISPs.OrderBy(x => x.MaLoai);
            return PartialView(loaiSP);
        }    
    }
}