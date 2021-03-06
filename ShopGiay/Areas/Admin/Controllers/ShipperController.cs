﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ShopGiay.Models;
using System.Security.Cryptography;
using PagedList;
using System.Data.Entity;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class ShipperController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        [ChildActionOnly]
        public ActionResult SidebarMenu()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult NavBar()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(NHANVIEN nv)
        {
            string email = Request.Form["Email"].ToString();
            string matKhau = Request.Form["MatKhau"].ToString();
            string fMatKhau = GetMD5(matKhau);
            NHANVIEN NVBan = db.NHANVIENs.SingleOrDefault(x => x.QuyenNV == 3);
            nv = db.NHANVIENs.SingleOrDefault(n => n.Email == email && n.MatKhau == fMatKhau);
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Email == email && n.MatKhau == fMatKhau);
            if (kh != null)
            {
                ViewBag.ThongBao = "Tài khoản không phải của shipper";
                return View();
            }
            if (nv != null)
            {
                if (nv.QuyenNV != NVBan.QuyenNV)
                {
                    ViewBag.ThongBao = "Tài khoản không phải của shipper";
                    return View();
                }
                else
                {
                    Session["Shipper"] = nv.MaNV;
                    Session["TenNV"] = nv.TenNV;
                    Session["Quyen"] = nv.QuyenNV;
                    return RedirectToAction("DHDangGiao");
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
        [NonAction]
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
      
        // Trang chủ
        public ActionResult DHDangGiao(string search, int? page, int? size)
        {
            //Tạo list pagesize 
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            // giữ kích thước trang được chọn trên Droplist
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.Size = items;// viewbag dropdownlist
            ViewBag.CurrentSize = size;

            page = page ?? 1; // nếu null page =1
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);

            var listDH = from dh in db.DONHANGs
                         where dh.TinhTrang == "Đang giao"
                         select dh;
            listDH = listDH.OrderBy(x => x.MaDH);
            if (!String.IsNullOrEmpty(search))
            {
                listDH = listDH.Where(x => x.KHACHHANG.TenKH.Contains(search));
                return View(listDH.ToPagedList(pageNumber, pageSize));
            }
            // nếu từ khóa null thì trả về list 
            return View(listDH.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DetailDH(int? maDH)
        {
            var dh = db.DONHANGs.SingleOrDefault(x => x.MaDH == maDH);
            ViewBag.TenKH = dh.KHACHHANG.TenKH;
            ViewBag.Sdt = dh.KHACHHANG.Sdt;
            ViewBag.DiaChi = dh.DiaChiGiao;
            ViewBag.TinhTrang = dh.TinhTrang;
            List<CHITIETDONHANG> ctdh = db.CHITIETDONHANGs.Where(x => x.MaDH == maDH).ToList();
            if (ctdh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else
            {
                decimal tongTien = 0;
                foreach (var item in ctdh)
                {
                    decimal tong = item.CHITIETSP.SANPHAM.DonGia * item.SoLuong;
                    tongTien += tong;
                }
                ViewBag.TongTien = tongTien;
                return View(ctdh);
            }
        }
        public ActionResult XacNhanDH(int maDH, int? page, int? size)
        {
            var dh = db.DONHANGs.Find(maDH);
            if (dh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else
            {
                dh.TinhTrang = "DONE";
                dh.ThanhToan = "Đã thanh toán";
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DonHangThanhCong", new { @page = page, @size = size });
            }
        }
        public ActionResult DonHangThanhCong(string search, int? page, int? size)
        {
            //Tạo list pagesize 
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            // giữ kích thước trang được chọn trên Droplist
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.Size = items;// viewbag dropdownlist
            ViewBag.CurrentSize = size;

            page = page ?? 1; // nếu null page =1
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);

            var listDH = from dh in db.DONHANGs
                         where dh.TinhTrang == "DONE"
                         select dh;
            listDH = listDH.OrderBy(x => x.MaDH);
            if (!String.IsNullOrEmpty(search))
            {
                listDH = listDH.Where(x => x.KHACHHANG.TenKH.Contains(search));
                return View(listDH.ToPagedList(pageNumber, pageSize));
            }
            // nếu từ khóa null thì trả về list 
            return View(listDH.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult InfoCaNhan()
        {
            int maNV = int.Parse(Session["Shipper"].ToString());

            if (maNV == 0)
            {
                return RedirectToAction("Login");
            }
            else
            {
                NHANVIEN nv = db.NHANVIENs.SingleOrDefault(x => x.MaNV == maNV);
                if (nv == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.TenNV = nv.TenNV;
                ViewBag.Email = nv.Email;
                return View();
            }
        }

        [HttpGet]
        public ActionResult UpdateInfoCaNhan(int? maNV)
        {
            maNV = int.Parse(Session["Shipper"].ToString());

            if (maNV == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                NHANVIEN nv = db.NHANVIENs.SingleOrDefault(x => x.MaNV == maNV);
                if (nv == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.GioiTinh = nv.GioiTinh;
                return View(nv);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateInfoCaNhan(NHANVIEN nv)
        {
            string tenNV = Request.Form["TenNV"].ToString();
            string diaChi = Request.Form["DiaChi"].ToString();
            string email = Request.Form["Email"].ToString();
            string sdt = Request.Form["Sdt"].ToString();
            string gioiTinh = Request.Form["GioiTinh"].ToString();
            string cmnd = Request.Form["CMND"].ToString();
            if (ModelState.Count == 7)
            {
                db.UpdateInfoCaNhan(tenNV, diaChi, email, sdt, gioiTinh, cmnd);
                TempData["ThongBao"] = "Thay đổi thông tin thành công!";
                return View(nv);
            }
            else
            {
                TempData["ThongBao"] = "Thay đổi thông tin không thành công!";
            }
            return View(nv);
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            int maNV = int.Parse(Session["Shipper"].ToString());
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(x => x.MaNV == maNV);
            if (nv == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.TenNV = nv.TenNV;
            ViewBag.Email = nv.Email;
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            string email = form["email"].ToString();
            string matKhau = form["matKhau"].ToString();
            string matKhauMoi = form["matKhauMoi"].ToString();
            string xacNhanMatKhau = form["xacNhanMatKhau"].ToString();

            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(x => x.Email == email);
            if (email == null || matKhau == null || matKhauMoi == null || xacNhanMatKhau == null)
            {
                ViewBag.Error = "Nhập đầy đủ thông tin";
                return View();
            }
            if (matKhau == matKhauMoi)
            {
                ViewBag.Error = "Nhập mật khẩu khác";
                return View();
            }
            if (matKhauMoi != xacNhanMatKhau)
            {
                ViewBag.Error = "Xác nhận mật khẩu không chính xác";
                return View();
            }
            var f_matKhau = GetMD5(matKhauMoi);
            nv.MatKhau = f_matKhau;
            db.Entry(nv).State = EntityState.Modified;
            TempData["ThongBao"] = "Đổi mật khẩu thành công!";
            db.SaveChanges();
            return View();
        }
    }
}