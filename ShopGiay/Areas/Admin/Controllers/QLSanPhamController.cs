using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.IO;
using System.Net;
using System.Web.Helpers;


namespace ShopGiay.Areas.Admin.Controllers
{
    public class QLSanPhamController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Admin/QLSanPham
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachSanPham(string search, int? page, int? size)
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
            var listSP = from sp in db.SANPHAMs select sp;
            listSP = listSP.OrderBy(x => x.MaSP);

            if (search != null)
            {
                listSP = listSP.Where(x => x.TenSP.Contains(search));
            }
            return View(listSP.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult AddSanPham()
        {
            ViewBag.MaNhanHieu = new SelectList(db.NHANHIEUx, "MaNhanHieu", "TenNhanHieu");
            ViewBag.MaLoai = new SelectList(db.LOAISPs, "MaLoai", "TenLoai");
            ViewBag.TenSanPham = new SelectList(db.SANPHAMs.ToList().OrderBy(x => x.MaSP), "MaSP", "TenSP");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddSanPham(SANPHAM sp, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            int maNhanHieu = int.Parse(Request.Form["MaNhanHieu"]);
            int maLoai = int.Parse(Request.Form["MaLoai"]);
                
            ViewBag.MaLoai = new SelectList(db.LOAISPs, "MaLoai", "TenLoai", sp.MaLoai);
            ViewBag.MaNhanHieu = new SelectList(db.NHANHIEUx, "MaNhanHieu", "TenNhanHieu", sp.MaNhanHieu);
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Chọn hình ảnh";
                return View();
            }
            //Thêm vào cơ sở dữ liệu
            if (ModelState.Count == 7)
            {
                //Lưu tên file
                var fileName = Path.GetFileName(fileUpload.FileName);

                var fileName2 = Path.GetFileName(fileUpload2.FileName);

                var fileName3 = Path.GetFileName(fileUpload3.FileName);
                //Lưu đường dẫn của file
                var path = Path.Combine(Server.MapPath("~/HinhAnh/HinhAnhSP"), fileName);

                var path2 = Path.Combine(Server.MapPath("~/HinhAnh/HinhAnhSP"), fileName2);

                var path3 = Path.Combine(Server.MapPath("~/HinhAnh/HinhAnhSP"), fileName3);
                //Kiểm tra hình ảnh đã tồn tại chưa
                if (System.IO.File.Exists(path))
                {
                    ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                }
                else
                {
                    fileUpload.SaveAs(path);
                }

                //Kiểm tra hình ảnh đã tồn tại chưa
                if (System.IO.File.Exists(path2))
                {
                    ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                }
                else
                {
                    fileUpload2.SaveAs(path2);
                }

                //Kiểm tra hình ảnh đã tồn tại chưa
                if (System.IO.File.Exists(path3))
                {
                    ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                }
                else
                {
                    fileUpload3.SaveAs(path3);
                }

                sp.Anh = fileUpload.FileName;
                sp.Anh2 = fileUpload2.FileName;
                sp.Anh3 = fileUpload3.FileName;
                //sp.NgayCapNhat = DateTime.Now;
                sp.MaNhanHieu = maNhanHieu;
                sp.MaLoai = maLoai;
                db.SANPHAMs.Add(sp);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm mới sản phẩm thành công!";
            }
            else
                TempData["ThongBao"] = "Thêm sản phẩm thất bại";
            return View();
        }
        [HttpGet]
        public ActionResult EditSanPham(int maSP)
        {
            // lấy ra sản phẩm theo mã
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // nếu có đưa dữ liệu vào viewBagNhanHieu
            ViewBag.NhanHieu = new SelectList(db.NHANHIEUx, "MaNhanHieu", "TenNhanHieu", sp.MaNhanHieu);
            ViewBag.Loai = new SelectList(db.LOAISPs, "MaLoai", "TenLoai", sp.MaLoai);
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditSanPham(SANPHAM sp)
        {
            // lấy mã nhãn hiệu
            int maNhanHieu = int.Parse(Request.Form["NhanHieu"]);
            int maLoai = int.Parse(Request.Form["MaLoai"]);
            // thêm vào cơ sở dữ liệu 
            if (ModelState.IsValid)
            {
                sp.MaNhanHieu = maNhanHieu;
                sp.MaLoai = maLoai;
                sp.NgayCapNhat = DateTime.Now;
                db.Entry(sp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Chỉnh sửa sản phẩm thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Chỉnh sửa sản phẩm không thành công!";
            }
            ViewBag.NhanHieu = new SelectList(db.NHANHIEUx, "MaNhanHieu", "TenNhanHieu", sp.MaNhanHieu);
            ViewBag.Loai = new SelectList(db.LOAISPs, "MaLoai", "TenLoai", sp.MaLoai);
            return View(sp);
        }

        public ActionResult ShowSanPham(int maSP)
        {
            //Lấy ra đối tượng theo mã
            SANPHAM sp = db.SANPHAMs.Find(maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        public ActionResult DeleteSanPham(int? maSP)
        {
            //Lấy ra đối tượng sp theo mã
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }

        [HttpPost, ActionName("DeleteSanPham")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int maSP)
        {
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SANPHAMs.Remove(sp);
            db.SaveChanges();
            return RedirectToAction("DanhSachSanPham");
        }
        public ActionResult DetailSanPham(int maSP, int? page)
        {
            TempData["MaSP"] = maSP;
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            //Lấy ra đối tượng sp theo mã
            var listSP = db.CHITIETSPs.Where(n => n.MaSP == maSP).OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize);
            if (listSP == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(listSP);
        }
    }
}