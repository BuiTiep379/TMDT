using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.Net;
using System.Data.Entity;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class QLLoaiSPController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Admin/LoaiSanPham

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DanhSachLoaiSP(string search, int? page, int? size)
        {
            // Tạo list page size 
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            // giữ size page hiện tại 
            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }
            // ViewBag cho dropDownList
            ViewBag.Size = items;
            // Viewbag cho size hiện tại
            ViewBag.CurrentSize = size;
            page = page ?? 1;
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 5);

            // Lấy danh sách loại sản phẩm 
            var ListLoai = from loai in db.LOAISPs select loai;
            // sắp xếp danh sách theo mã sản phẩm
            ListLoai = ListLoai.OrderBy(x => x.MaLoai);
            if (!String.IsNullOrEmpty(search))
            {
                ListLoai = ListLoai.Where(x => x.TenLoai.Contains(search));
            }
            return View(ListLoai.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult AddLoaiSP()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLoaiSP(LOAISP loai)
        {
            if (ModelState.IsValid)
            {
                var tenLoai = loai.TenLoai;
                var check = db.LOAISPs.Where(x => x.TenLoai == tenLoai);
                if (check.Count() == 0)
                {
                    db.LOAISPs.Add(loai);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Thêm loại sản phẩm thành công!";
                    return View(loai);
                }
                else
                {
                    TempData["ThongBao"] = "Tên loại sản phẩm đã tồn tại!";
                    return View(loai);
                }
            }
            else
            {
                TempData["ThongBao"] = "Thêm loại sản phẩm không thành công!";
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditLoaiSP(int? maLoai)
        {
            LOAISP loai = db.LOAISPs.SingleOrDefault(x => x.MaLoai == maLoai);
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loai);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditLoaiSP(LOAISP loai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loai).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Chỉnh sửa loại sản phẩm thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Chỉnh sửa loại sản phẩm không thành công!";
            }
            return View(loai);
        }
    }
}