using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class QLChiTietSanPhamController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Admin/QLChiTietSanPham
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachChiTietSP(string search, int? page, int? size)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });

            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.Size = items;
            ViewBag.CurrentSize = size;

            page = (page ?? 1);
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 5);
            var listSP = from ct in db.CHITIETSPs select ct;
            listSP = listSP.OrderBy(x => x.ID);

            if (search != null)
            {
                listSP = listSP.Where(x => x.SANPHAM.TenSP.Contains(search));
            }
            return View(listSP.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult AddChiTietSP()
        {
            ViewBag.MaMau = new SelectList(db.MAUSACs.OrderBy(x => x.MaMau).ToList(), "MaMau", "MauSac");
            ViewBag.MaSize = new SelectList(db.SIZEs.OrderBy(x => x.MaSize).ToList(), "MaSize", "Size");
            ViewBag.MaSP = new SelectList(db.SANPHAMs.OrderBy(x => x.MaSP).ToList(), "MaSP", "TenSP");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddChiTietSP(CHITIETSP ct)
        {
            int maMau = int.Parse(Request.Form["MaMau"]);
            int maSize = int.Parse(Request.Form["MaSize"]);
            int maSP = int.Parse(Request.Form["MaSP"]);
            ViewBag.MaMau = new SelectList(db.MAUSACs.OrderBy(x => x.MaMau).ToList(), "MaMau", "MauSac");
            ViewBag.MaSize = new SelectList(db.SIZEs.OrderBy(x => x.MaSize).ToList(), "MaSize", "Size");
            ViewBag.MaSP = new SelectList(db.SANPHAMs.OrderBy(x => x.MaSP).ToList(), "MaSP", "TenSP");
            if (ModelState.IsValid)
            {
                ct.MaSP = maSP;
                ct.MaMau = maMau;
                ct.MaSize = maSize;
                db.CHITIETSPs.Add(ct);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm chi tiết sản phẩm thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Thêm chi tiết sản phẩm không thành công!";
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditChiTietSP(int id)
        {
            // lấy ra sản phẩm theo mã
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.ID == id);
            if (ct == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // nếu có đưa dữ liệu vào viewBagNhanHieu
            ViewBag.MaMau = new SelectList(db.MAUSACs.OrderBy(x => x.MaMau).ToList(), "MaMau", "MauSac", ct.MaMau);
            ViewBag.MaSize = new SelectList(db.SIZEs.OrderBy(x => x.MaSize).ToList(), "MaSize", "Size", ct.MaSize);
            ViewBag.MaSP = new SelectList(db.SANPHAMs.OrderBy(x => x.MaSP).ToList(), "MaSP", "TenSP", ct.MaSP);
            return View(ct);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditChiTietSP(CHITIETSP ct)
        {
            // lấy mã nhãn hiệu
            int maMau = int.Parse(Request.Form["MaMau"]);
            int maSize = int.Parse(Request.Form["MaSize"]);
            int maSP = int.Parse(Request.Form["MaSP"]);
            // thêm vào cơ sở dữ liệu 
            if (ModelState.IsValid)
            {
                ct.MaSP = maSP;
                ct.MaMau = maMau;
                ct.MaSize = maSize;
                db.Entry(ct).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Chỉnh sửa sản phẩm thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Chỉnh sửa sản phẩm không thành công!";
            }
            ViewBag.MaMau = new SelectList(db.MAUSACs.OrderBy(x => x.MaMau).ToList(), "MaMau", "MauSac", ct.MaMau);
            ViewBag.MaSize = new SelectList(db.SIZEs.OrderBy(x => x.MaSize).ToList(), "MaSize", "Size", ct.MaSize);
            ViewBag.MaSP = new SelectList(db.SANPHAMs.OrderBy(x => x.MaSP).ToList(), "MaSP", "TenSP", ct.MaSP);
            return View(ct);
        }
        public ActionResult DeleteCTSP(int? id)
        {
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.ID == id);
            if (ct == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ct);
        }
        [HttpPost, ActionName("DeleteCTSP")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int? id)
        {
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.ID == id);
            if (ct == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.CHITIETSPs.Remove(ct);
            db.SaveChanges();
            return RedirectToAction("DanhSachChiTietSP");
        }
    }
}