using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.Net;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class QLKhachHangController : Controller
    {
        // khai báo biến để kết nối với cở sở dữ liệu
        ShopGiayEntities db = new ShopGiayEntities();

        // GET: Admin/QLKhachHang
        public ActionResult Index()
        {
            return View();
        }
        // Hiện thị danh sách khách hàng
        public ActionResult DanhSachKH(string search, int? page, int? size)
        {
            // Tạo list pagesize
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });

            // giữ pagesize trên dropDownList
            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            // viewbag dropdownlist
            ViewBag.Size = items;

            //ViewBag size hiện tại
            ViewBag.CurrentSize = size;
            if (page == null)
            {
                page = 1;
            }
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 5);
            // lấy danh sách khách hàng trong database
            var listKH = from kh in db.KHACHHANGs select kh;
            // sắp sếp lại danh sách khách hàng theo mã khách hàng
            listKH = listKH.OrderBy(x => x.MaKH);
            // Nếu search không null
            if (!String.IsNullOrEmpty(search))
            {
                // tìm kiếm khách hàng dựa theo search
                listKH = listKH.Where(x => x.TenKH.Contains(search));
                return View(listKH.ToPagedList(pageNumber, pageSize));  
            }    
            return View(listKH.ToPagedList(pageNumber, pageSize));
        }
  
        // Chi tiết khách hàng
        public ActionResult DetailKhachHang(int maKH)
        {
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(model => model.MaKH == maKH);
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }

        // Xóa khách hàng
        public ActionResult DeleteKhachHang(int? maKH)
        {
            if (maKH == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG kh = db.KHACHHANGs.Find(maKH);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }
        [HttpPost, ActionName("DeleteKhachHang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int maKH)
        {
            KHACHHANG kh = db.KHACHHANGs.Find(maKH);
            db.KHACHHANGs.Remove(kh);
            db.SaveChanges();
            return RedirectToAction("DanhSachKH");
        }
    }
}