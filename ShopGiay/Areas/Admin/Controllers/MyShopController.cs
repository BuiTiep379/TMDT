using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.IO;
using System.Data.Entity;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class MyShopController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        
        // GET: Admin/NguoiBan
        public ActionResult Index(FormCollection form)
        {
            // Biểu đồ cột
            var ordersForBar = db.DONHANGs.ToList();
            var dataForBar = new List<decimal>();
            var categoriesForBar = new List<string>();
            for (int i = 1; i < 13; i++)
            {
                categoriesForBar.Add(i.ToString());
                dataForBar.Add(0);
            }

            foreach (var order in ordersForBar)
            {
                if (!categoriesForBar.Contains(order.NgayDatHang.Month.ToString()))
                {
                    categoriesForBar.Add(order.NgayDatHang.Month.ToString());
                    dataForBar.Add(order.TongTien);
                }
                else
                {
                    int index = categoriesForBar.IndexOf(order.NgayDatHang.Month.ToString());
                    dataForBar[index] += order.TongTien;
                }

            }
            ViewBag.barLabels = categoriesForBar.ToArray();
            ViewBag.barData = dataForBar.ToArray();
            // Biểu đồ tròn
            var now = DateTime.Now;
            int monthPie, yearPie;
            string mouthYear = form["month"];
            if (mouthYear != null)
            {
                monthPie = Convert.ToInt32(mouthYear.Substring(5, 2));
                yearPie = Convert.ToInt32(mouthYear.Substring(0, 4));
            }
            else
            {
                monthPie = Convert.ToInt32(now.Month);
                yearPie = Convert.ToInt32(now.Year);
            }
            var dataForPie = new List<decimal>();
            var dhForPie = db.DONHANGs.Where(x => x.NgayDatHang.Month == monthPie && x.NgayDatHang.Year == yearPie);
            var categoriesForPie = new List<string>();
            List<int?> tempidOrder = new List<int?>();
            List<int> tempidProduct = new List<int>();
            foreach (var dh in dhForPie)
            {
                if (!tempidOrder.Contains(dh.MaDH))
                    tempidOrder.Add(dh.MaDH);
            }
            var ctdhForPie = db.CHITIETDONHANGs.Where(x => tempidOrder.Contains(x.MaDH)).ToList();

            foreach (var item in ctdhForPie)
            {
                if (!categoriesForPie.Contains(item.SANPHAM.NHANHIEU.TenNhanHieu))
                {
                    categoriesForPie.Add(item.SANPHAM.NHANHIEU.TenNhanHieu);
                    dataForPie.Add(item.DonGia);
                }
                else
                {
                    int index = categoriesForPie.IndexOf(item.SANPHAM.NHANHIEU.TenNhanHieu);
                    dataForPie[index] += item.SANPHAM.DonGia * item.SoLuong;
                }
            }
            ViewBag.pieLabels = categoriesForPie.ToArray();
            ViewBag.pieData = dataForPie.ToArray();
            ViewBag.monthPie = monthPie;
            ViewBag.yearPie = yearPie;
            return View();
        }
        #region CHIA LAYOUT ADMIN
        public ActionResult SidebarMenu()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult NavBar()
        {
            return PartialView();
        }
        #endregion


        #region DANH SÁCH SẢN PHẨM CỦA NGƯỜI BÁN 
        public ActionResult DanhSachSanPham(string search, int? page, int? size)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });

            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.Size = items;
            ViewBag.CurrentSize = size;

            page = (page ?? 1);
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);
            int id = Convert.ToInt32(Session["UserID"]);
            List<SANPHAM> listSP = new List<SANPHAM>();
            foreach (var item in db.SANPHAMs)
            {
                if (item.UserID == id)
                {
                    listSP.Add(item);
                }
            }

            if (search != null)
            {
                listSP = (List<SANPHAM>)listSP.Where(x => x.TenSP.Contains(search));
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
            int maKH = int.Parse(Session["UserID"].ToString());
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
                sp.UserID = maKH;
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
        public ActionResult DeleteSPConfirm(int maSP)
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

        #endregion


        #region QUẢN LÝ MÀU SẮC SẢN PHẨM
        public ActionResult DanhSachMauSac(string search, int? page, int? size)
        {
            // Tạo list page size 
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });

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

            var listMau = from mau in db.MAUSACs select mau;
            listMau = listMau.OrderBy(x => x.MaMau);
            if (!String.IsNullOrEmpty(search))
            {
                listMau = listMau.Where(x => x.MauSac.Contains(search));
                return View(listMau.ToPagedList(pageNumber, pageSize));
            }


            if (search != null)
            {
                listMau = listMau.Where(x => x.MauSac.Contains(search));
            }
            return View(listMau.ToPagedList(pageNumber, pageSize));
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
            int maKH = int.Parse(Session["UserID"].ToString());
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

        #endregion


        #region QUẢN LÝ LOẠI SẢN PHẨM

        public ActionResult DanhSachLoaiSP(string search, int? page, int? size)
        {
            // Tạo list page size 
            List<SelectListItem> items = new List<SelectListItem>();
             items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });

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
            int pageSize = (size ?? 10);
            int id = Convert.ToInt32(Session["UserID"]);
            List<LOAISP> listLoai = new List<LOAISP>();
            foreach (var item in db.LOAISPs)
            {
                if (item.UserID == id)
                {
                    listLoai.Add(item);
                }
            }

            if (search != null)
            {
                listLoai = (List<LOAISP>)listLoai.Where(x => x.TenLoai.Contains(search));
            }
            return View(listLoai.ToPagedList(pageNumber, pageSize));
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
            int maKH = int.Parse(Session["UserID"].ToString());
            if (ModelState.IsValid)
            {
                var tenLoai = loai.TenLoai;
                var check = db.LOAISPs.Where(x => x.TenLoai == tenLoai);
                if (check.Count() == 0)
                {
                    loai.UserID = maKH;
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

        #endregion

        #region QUẢN LÝ DANH SÁCH MÃ GIẢM GIÁ

        public ActionResult DanhSachPromoCode(string search, int? page, int? size)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });


            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.Size = items;
            ViewBag.CurrentSize = size;

            page = (page ?? 1);
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);
            int id = Convert.ToInt32(Session["UserID"]);
            List<PROMOCODE> listCode = new List<PROMOCODE>();
            foreach (var item in db.PROMOCODEs)
            {
                if (item.UserID == id)
                {
                    listCode.Add(item);
                }
            }

            if (search != null)
            {
                listCode = (List<PROMOCODE>)listCode.Where(x => x.Code.Contains(search));
            }
            return View(listCode.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult KichHoatCode(int id, int? page, int? size)
        {
            var code = db.PROMOCODEs.Find(id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;

            }
            else
            {
                code.Status = true;
                db.Entry(code).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachPromoCode", new { @page = page, @size = size });
            }
        }
        public ActionResult HuyCode(int id, int? page, int? size)
        {
            var code = db.PROMOCODEs.Find(id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;

            }
            else
            {
                code.Status = false;
                db.Entry(code).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachPromoCode", new { @page = page, @size = size });
            }
        }
        [HttpGet]
        public ActionResult AddPromoCode()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPromoCode(FormCollection form)
        {
            int maKH = int.Parse(Session["UserID"].ToString());
            string code = form["Code"].ToString();
            string name = form["Name"].ToString();
            decimal value = decimal.Parse(form["Value"].ToString());
            bool status = bool.Parse(form["Status"]);
            var kt = db.PROMOCODEs.Where(x => x.Code == code);
            if (kt.Count() == 0)
            {
                var c = new PROMOCODE()
                {
                    Name = name,
                    Code = code,
                    Value = value,
                    Status = status,
                    UserID = maKH
                };
                db.PROMOCODEs.Add(c);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm code thành công!";
                return View(c);
            }
            else
            {
                TempData["ThongBao"] = "Code đã tồn tại!";
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditPromoCode(int? id)
        {
            PROMOCODE code = db.PROMOCODEs.SingleOrDefault(x => x.Id == id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(code);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPromoCode(PROMOCODE code)
        {
            if (ModelState.IsValid)
            {
                db.Entry(code).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Chỉnh sửa code thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Chỉnh sửa code không thành công!";
            }
            return View(code);
        }

        public ActionResult DeletePromoCode(int? id)
        {
            PROMOCODE code = db.PROMOCODEs.SingleOrDefault(x => x.Id == id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(code);
        }

        [HttpPost, ActionName("DeletePromoCode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            PROMOCODE code = db.PROMOCODEs.SingleOrDefault(x => x.Id == id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.PROMOCODEs.Remove(code);
            db.SaveChanges();
            return RedirectToAction("DanhSachPromoCode");
        }

        #endregion

        #region DANH SÁCH CHI TIẾT SẢN PHẨM

        public ActionResult DanhSachChiTietSP(string search, int? page, int? size)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });


            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.Size = items;
            ViewBag.CurrentSize = size;

            page = (page ?? 1);
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);
            int id = Convert.ToInt32(Session["UserID"]);
            List<CHITIETSP> listCT = new List<CHITIETSP>();
            foreach (var item in db.CHITIETSPs)
            {
                if (item.SANPHAM.UserID == id)
                {
                    listCT.Add(item);
                }
            }

            if (search != null)
            {
                listCT = (List<CHITIETSP>)listCT.Where(x => x.SANPHAM.TenSP.Contains(search));
            }
            return View(listCT.ToPagedList(pageNumber, pageSize));
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
            int id = Convert.ToInt32(Session["UserID"]);
            ViewBag.MaMau = new SelectList(db.MAUSACs.OrderBy(x => x.MaMau).ToList(), "MaMau", "MauSac");
            ViewBag.MaSize = new SelectList(db.SIZEs.OrderBy(x => x.MaSize).ToList(), "MaSize", "Size");
            ViewBag.MaSP = new SelectList(db.SANPHAMs.OrderBy(x => x.MaSP).ToList(), "MaSP", "TenSP");
            if (ModelState.IsValid)
            {
                ct.MaSP = maSP;
                ct.MaMau = maMau;
                ct.MaSize = maSize;
                ct.SANPHAM.UserID = id;
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
        public ActionResult DeleteCTConfirm(int? id)
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
        #endregion

        #region QUẢN LÝ DANH SÁCH DƠN HÀNG 

        public ActionResult DanhSachDH(string search, int? page, int? size)
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

            int maKH = Convert.ToInt32(Session["UserID"]);
            List<DONHANG> listDH = new List<DONHANG>();
            foreach (var item in db.DONHANGs)
            {
                if (item.SellerID == maKH)
                    listDH.Add(item);
            }
            if (search != null)
            {
                listDH = (List<DONHANG>)listDH.Where(x => x.HoTen.Contains(search));
            }
            // nếu từ khóa null thì trả về list 
            return View(listDH.ToPagedList(pageNumber, pageSize));
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
                dh.TinhTrang = "Đã xác nhận";
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachDH", new { @page = page, @size = size });
            }
        }
        public ActionResult DetailDH(int? maDH)
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
        #endregion
        #region VIEW THÔNG TIN CHUNG

        public ActionResult SoLuongDonHang()
        {
            int maKH = Convert.ToInt32(Session["UserID"]);
            List<DONHANG> listDH = new List<DONHANG>();
            foreach (var item in db.DONHANGs)
            {
                if (item.SellerID == maKH)
                    listDH.Add(item);
            }
            ViewBag.SoLuongDH = listDH.Count;
            return PartialView();
        }
        public ActionResult TongDoanhThu()
        {
            int maKH = Convert.ToInt32(Session["UserID"]);
            List<DONHANG> listDH = new List<DONHANG>();
            foreach (var item in db.DONHANGs)
            {
                if (item.SellerID == maKH)
                    listDH.Add(item);
            }
            var tongdt = listDH.OrderBy(m => m.MaDH).Sum(m => m.TongTien);
            ViewBag.TongDoanhThu = tongdt;
            return PartialView();
        }
        #endregion
    }
}