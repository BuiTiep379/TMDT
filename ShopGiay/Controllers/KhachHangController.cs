using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.Security.Cryptography;
using System.Text;

namespace ShopGiay.Controllers
{
    public class KhachHangController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: KhachHang
        public ActionResult Index()
        {
            return View();
        }
        
    }
}