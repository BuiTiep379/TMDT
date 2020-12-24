using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;

namespace ShopGiay.Controllers
{
    public class StoreController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
         public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection form)
        {
            string tenKH = form["txttenkh"].ToString();
            string email = form["txtemail"].ToString();
            string diaChi = form["txtdiachi"].ToString();
            string sdt = form["txtsdt"].ToString();
            string matKhau = form["txtmatkhau"].ToString();
            string xacNhanMK = form["txtxnmk"].ToString();
            if (matKhau != xacNhanMK)
            {
                ViewBag.ThongBao = "Xác nhận mật khẩu không trùng khớp";
                return View();
            }
            
            if (ModelState.IsValid)
            {
                var check = db.KHACHHANGs.FirstOrDefault(s => s.Email == email);
                if (check == null)
                {
                    var maHoa = GetMD5(matKhau);
                    KHACHHANG kh = new KHACHHANG()
                    {
                        TenKH = tenKH,
                        DiaChi = diaChi,
                        Email = email,
                        Sdt = sdt,
                        MatKhau = maHoa,
                    };
                    db.KHACHHANGs.Add(kh);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
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
            string sEmail = f["txtemail"].ToString();
            string sMatKhau = f["txtmatkhau"].ToString();
            
            var f_password = GetMD5(sMatKhau);
            var kh = db.KHACHHANGs.SingleOrDefault(x => x.Email == sEmail && x.MatKhau == f_password);
            if (kh != null)
            {
                ViewBag.ThongBao = "Đăng nhập thành công";
                Session["UserID"] = kh.MaKH;
                Session["Email"] = kh.Email;
                Session["TenKH"] = kh.TenKH;
                return RedirectToAction("Index");
            }
            ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!!";
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Remove("UserID");
            Session.Remove("Email");
            return RedirectToAction("Login");
        }



        //create a string MD5
        public static string GetMD5(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }


        public ActionResult InfoCaNhan(int? maKH)
        {
            maKH = int.Parse(Session["UserID"].ToString());
            
            if (maKH == 0)
            {
                return RedirectToAction("Shop");
            }    
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
                if (kh == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.TenKH = kh.TenKH;
                return View(kh);
            }
        }
        [HttpGet]
        public ActionResult UpdateInfoCaNhan(int? maKH)
        {
            maKH = int.Parse(Session["UserID"].ToString());
            if (maKH == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
                if (kh == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.TenKH = kh.TenKH;
                return View(kh);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateInfoCaNhan(KHACHHANG kh)
        {
            string tenKH = Request.Form["TenKh"].ToString();
            string email = Request.Form["Email"].ToString();
            string diaChi = Request.Form["DiaChi"].ToString();
            string sdt = Request.Form["Sdt"].ToString();
            if (ModelState.Count == 5)
            {
                kh = db.KHACHHANGs.SingleOrDefault(x => x.Email == email);
                kh.TenKH = tenKH;
                kh.Email = email;
                kh.Sdt = sdt;
                kh.DiaChi = diaChi;
                db.Entry(kh).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Thay đổi thông tin thành công!";
                return RedirectToAction("InfoCaNhan");
            }    
            else
            {
                TempData["ThongBao"] = "Thay đổi thông tin không thành công!";
            }
            return View(kh);
        }

        public ActionResult ChangePassword()
        {
            int maKH = int.Parse(Session["UserID"].ToString());
            if (maKH == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
                if (kh == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.TenKH = kh.TenKH;
                return View();
            }
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            string email = form["email"].ToString();
            string matKhau = form["matKhau"].ToString();
            string matKhauMoi = form["matKhauMoi"].ToString();
            string xacNhanMatKhau = form["xacNhanMatKhau"].ToString();

            var kh = db.KHACHHANGs.SingleOrDefault(x => x.Email == email);
            
            if (kh == null)
            {
                ViewBag.Error = "User not existed or wrong password";
                return View();
            }
            if (email == null || matKhau == null || matKhauMoi == null || xacNhanMatKhau == null)
            {
                ViewBag.Error = "Fields are required";
                return View();
            }
            if (matKhau == matKhauMoi)
            {
                ViewBag.Error = "The new password is the same as the old one";
                return View();
            }    
            if (matKhauMoi != xacNhanMatKhau)
            {
                ViewBag.Error = "Password not matched";
                return View();
            }
            var f_matKhau = GetMD5(matKhauMoi);
            kh.MatKhau = f_matKhau;
            ViewBag.TenKH = kh.TenKH;   
            db.Entry(kh).State = EntityState.Modified;
            TempData["ThongBao"] = "Đổi mật khẩu thành công!";
            db.SaveChanges();
            return RedirectToAction("InfoCaNhan");
        }
        // hiện thị list đơn hàng theo maKH
        [HttpGet]
        public ActionResult DonMua(int? maKH)
        {
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
            var dh = db.DONHANGs.Where(x => x.MaKH == maKH).ToList();
            ViewBag.TenKH = kh.TenKH;
            
            if (dh == null)
            {
                TempData["ThongBao"] = "Bạn chưa mua sản phẩm nào";
                return View();
            }    
            
            return View(dh);
        }
       
       public ActionResult DetailDonHang(int? maDH)
        {
            var dh = db.DONHANGs.SingleOrDefault(x => x.MaDH == maDH);
            ViewBag.TenKH = dh.HoTen;
            ViewBag.Sdt = dh.Sdt;
            ViewBag.DiaChi = dh.DiaChiGiao;
            ViewBag.TinhTrang = dh.TinhTrang;
            ViewBag.TongTien = dh.TongTien;
            var ctdh = db.CHITIETDONHANGs.Where(x => x.MaDH == maDH);
            if (ctdh == null)
            {
                Response.StatusCode = 404;
                return null;
            }    
            else
            {
                return View(ctdh.ToList());
            }    
        }
       
        
        public ActionResult Shop()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult SiteNavBar()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.KiemTra = 0;
            }    
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult SiteBlocksCover()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult SiteSection()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult SiteBlocks2()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult SiteBlocks3()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult SiteBlocks8()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult SiteFooter()
        {
            return PartialView();
        }
    }

}

