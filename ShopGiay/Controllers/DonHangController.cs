using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;

namespace ShopGiay.Controllers
{
    public class DonHangController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: DonHang
        public ActionResult Index()
        {
            return View();
        }
        

        

        
    }
}