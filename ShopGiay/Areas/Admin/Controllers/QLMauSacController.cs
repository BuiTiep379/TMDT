using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.Data.Entity;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class QLMauSacController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Admin/QLMauSac
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachMauSac(string search, int? page, int? size)
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
            var ListMau = from mau in db.MAUSACs select mau;
            // sắp xếp danh sách theo mã sản phẩm
            ListMau = ListMau.OrderBy(x => x.MaMau);
            if (!String.IsNullOrEmpty(search))
            {
                ListMau = ListMau.Where(x => x.MauSac.Contains(search));
            }
            return View(ListMau.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult AddMau()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMau(MAUSAC mau)
        {
            if (ModelState.IsValid)
            {
                var mauSac = mau.MauSac;
                var check = db.MAUSACs.Where(x => x.MauSac == mauSac);
                if (check.Count() == 0)
                {
                    db.MAUSACs.Add(mau);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Thêm màu thành công!";
                    return View(mau);
                }
                else
                {
                    TempData["ThongBao"] = "Tên màu đã tồn tại!";
                    return View(mau);
                }
            }
            else
            {
                TempData["ThongBao"] = "Thêm màu không thành công!";
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditMau(int? maMau)
        {
            MAUSAC mau = db.MAUSACs.SingleOrDefault(x => x.MaMau == maMau);
            if (mau == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(mau);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMau(MAUSAC mau)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mau).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Chỉnh sửa màu thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Chỉnh sửa màu không thành công!";
            }
            return View(mau);
        }
    }
}