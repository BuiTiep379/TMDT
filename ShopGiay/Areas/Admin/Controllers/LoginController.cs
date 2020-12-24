using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ShopGiay.Models;


namespace ShopGiay.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();

        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection f)
        {
            string email = f["txtEmail"].ToString();
            string matKhau = f["txtmatKhau"].ToString();
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(n => n.Email == email && n.MatKhau == matKhau);
            if (nv != null)
            {
                if (nv.QuyenNV == "1")
                {
                    string chao = "Xin chào, Admin:" + nv.TenNV;
                    TempData["ThongBao"] = chao;
                    
                    Session["TaiKhoanNV"] = nv;
                    Session["User"] = nv.TenNV;
                    Session["Pw"] = nv.MatKhau;
                    Session["MaNV"] = nv.MaNV;
                    Session["Quyen"] = nv.QuyenNV;
                    return RedirectToAction("Index");
                }
                else
                {
                    string chao = "Xin chào, nhân viên:" + nv.TenNV;
                    TempData["ThongBao"] = chao;
                    Session["TaiKhoanNV"] = nv;
                    Session["User"] = nv.TenNV;
                    Session["Pw"] = nv.MatKhau;
                    Session["Quyen"] = nv.QuyenNV;
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!";
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        
    }
}