using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;

namespace ShopGiay.Controllers
{
    public class SanPhamController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: SanPham
        public ActionResult Index()
        {
            return View();
        }
        // danh sách sản phẩm ở header
        [ChildActionOnly]
        public ActionResult SanPhamNoiBat()
        {
            var listSP = db.SANPHAMs.Take(5);
            return PartialView(listSP);
        }
        //Danh sách sản phẩm ở body store
        [ChildActionOnly]
        public ActionResult DanhSachSanPhamNoiBat()
        {
            var ListSP = db.SANPHAMs.OrderBy(x => x.MaSP).Take(9);
            return PartialView(ListSP);
        }
        public ActionResult DanhSachSanPham(string search, int? page, int? size)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "12", Value = "12" });

            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.Size = items;
            ViewBag.CurrentSize = size;

            page = (page ?? 1);
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 3);
            var listSP = from sp in db.SANPHAMs select sp;
            listSP = listSP.OrderBy(x => x.MaSP);
            if (search != null)
            {
                listSP = listSP.Where(x => x.TenSP.Contains(search));
                return PartialView(listSP.ToPagedList(pageNumber, pageSize));
            }
            return PartialView(listSP.ToPagedList(pageNumber, pageSize));
        }
        //Xem chi tiết giày
        public ViewResult XemChiTiet(int maSP = 0, int maNhanHieu = 0, int maLoai = 0)
        {
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                //Trả về trang báo lỗi
                Response.StatusCode = 404;
                return null;
            }
            //----
            var mau = from a in db.CHITIETSPs
                      join b in db.MAUSACs
                      on a.MaMau equals b.MaMau
                      where (a.MaSP == maSP)
                      select new
                      {
                          a.MaMau,
                          b.MauSac
                      };

            var size = from a in db.CHITIETSPs
                       join b in db.SIZEs
                       on a.MaSize equals b.MaSize
                       where (a.MaSP == maSP)
                       select new
                       {
                           a.MaSize,
                           b.Size
                       };
            //-
            ViewBag.NhanHieu = db.NHANHIEUx.Single(x => x.MaNhanHieu == sp.MaNhanHieu).TenNhanHieu;
            ViewBag.Loai = db.LOAISPs.Single(x => x.MaLoai == sp.MaLoai).TenLoai;
            ViewBag.MaMau = new SelectList(mau.GroupBy(x => x.MaMau).Select(x => x.FirstOrDefault()), "MaMau", "MauSac");
            ViewBag.MaSize = new SelectList(size.GroupBy(x => x.MaSize).Select(g => g.FirstOrDefault()), "MaSize", "Size");
            ViewBag.SPLienQuan = db.SANPHAMs.Where(n => n.MaSP != maSP && n.MaNhanHieu == maNhanHieu && n.MaLoai == maLoai).Take(4).ToList();
            return View(sp);
        }

    }
}