using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.IO;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class MyShopController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
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
            NHANVIEN NVBan = db.NHANVIENs.SingleOrDefault(x => x.QuyenNV == 2);
            nv = db.NHANVIENs.SingleOrDefault(n => n.Email == email && n.MatKhau == fMatKhau);
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Email == email && n.MatKhau == fMatKhau);
            if (kh != null)
            {
                ViewBag.ThongBao = "Tài khoản không phải của nhân viên bán!";
                return View();
            }
            if (nv != null)
            {
                if (nv.QuyenNV != NVBan.QuyenNV)
                {
                    ViewBag.ThongBao = "Tài khoản không phải là nhân viên bán!";
                    return View();
                }
                else
                {
                    Session["MaNV"] = nv.MaNV;
                    Session["TenNV"] = nv.TenNV;
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
        // GET: Admin/NguoiBan
        #region Trang chủ người bán
        public ActionResult Index(FormCollection form)
        {
            int maNV = int.Parse(Session["MaNV"].ToString());
            // Biểu đồ cột
            var ordersForBar = db.DONHANGs.Where(x => x.TinhTrang == "DONE").ToList();
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
                    decimal tongTien = order.CHITIETDONHANG.CHITIETSP.SANPHAM.DonGia* order.CHITIETDONHANG.SoLuong;
                    dataForBar.Add(tongTien);
                }
                else
                {
                    int index = categoriesForBar.IndexOf(order.NgayDatHang.Month.ToString());
                    decimal tongTien = order.CHITIETDONHANG.CHITIETSP.SANPHAM.DonGia * order.CHITIETDONHANG.SoLuong;
                    dataForBar[index] += tongTien;
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
            var dhForPie = db.DONHANGs.Where(x => x.NgayDatHang.Month == monthPie && x.NgayDatHang.Year == yearPie && x.TinhTrang == "DONE");
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
                if (!categoriesForPie.Contains(item.CHITIETSP.SANPHAM.NHANHIEU.TenNhanHieu))
                {
                    categoriesForPie.Add(item.CHITIETSP.SANPHAM.NHANHIEU.TenNhanHieu);
                    decimal tongTien = item.CHITIETSP.SANPHAM.DonGia * item.SoLuong;
                    dataForPie.Add(item.SoLuong * item.CHITIETSP.SANPHAM.DonGia);
                }
                else
                {
                    int index = categoriesForPie.IndexOf(item.CHITIETSP.SANPHAM.NHANHIEU.TenNhanHieu);
                    dataForPie[index] += item.CHITIETSP.SANPHAM.DonGia * item.SoLuong;
                }
            }
            ViewBag.pieLabels = categoriesForPie.ToArray();
            ViewBag.pieData = dataForPie.ToArray();
            ViewBag.monthPie = monthPie;
            ViewBag.yearPie = yearPie;
            return View();
        }
        #endregion

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
            var listSP = from sp in db.SANPHAMs 
                         where (sp.Status == true)
                         select sp;
            listSP = listSP.OrderBy(x => x.MaSP);
            if (!String.IsNullOrEmpty(search))
            {
                listSP = listSP.Where(x => x.TenSP.Contains(search));
                return View(listSP.ToPagedList(pageNumber, pageSize));
            }
            return View(listSP.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult AddSanPham()
        {
            ViewBag.MaNhanHieu = new SelectList(db.NHANHIEUx.Where(x => x.Status == true), "MaNhanHieu", "TenNhanHieu");
            ViewBag.MaLoai = new SelectList(db.LOAISPs.Where(x =>  x.Status == true), "MaLoai", "TenLoai");
            ViewBag.TenSanPham = new SelectList(db.SANPHAMs.ToList().OrderBy(x => x.MaSP), "MaSP", "TenSP");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddSanPham(SANPHAM sp, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            int maNhanHieu = int.Parse(Request.Form["MaNhanHieu"]);
            int maLoai = int.Parse(Request.Form["MaLoai"]);
            ViewBag.MaLoai = new SelectList(db.LOAISPs.Where(x => x.Status == true), "MaLoai", "TenLoai", sp.MaLoai);
            ViewBag.MaNhanHieu = new SelectList(db.NHANHIEUx.Where(x => x.Status == true), "MaNhanHieu", "TenNhanHieu", sp.MaNhanHieu);
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
                sp.MaNhanHieu = maNhanHieu;
                sp.MaLoai = maLoai;
                sp.Status = true;
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
            ViewBag.MaLoai = new SelectList(db.LOAISPs.Where(x => x.Status == true), "MaLoai", "TenLoai", sp.MaLoai);
            ViewBag.MaNhanHieu = new SelectList(db.NHANHIEUx.Where(x => x.Status == true), "MaNhanHieu", "TenNhanHieu", sp.MaNhanHieu);
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
            ViewBag.MaNhanHieu = new SelectList(db.NHANHIEUx.Where(x => x.Status == true), "MaNhanHieu", "TenNhanHieu", sp.MaNhanHieu);
            ViewBag.MaLoai = new SelectList(db.LOAISPs.Where(x => x.Status == true), "MaLoai", "TenLoai", sp.MaLoai);
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
            sp.Status = false;
            db.Entry(sp).State = EntityState.Modified;
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
          
            var listLoai = from loai in db.LOAISPs 
                           where loai.Status == true
                           select loai;
            listLoai = listLoai.OrderBy(x => x.MaLoai);
            if (!String.IsNullOrEmpty(search))
            {
                listLoai = listLoai.Where(x => x.TenLoai.Contains(search));
                return View(listLoai.ToPagedList(pageNumber, pageSize));
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
            if (ModelState.IsValid)
            {
                var tenLoai = loai.TenLoai;
                var check = db.LOAISPs.Where(x => x.TenLoai == tenLoai);
                if (check.Count() == 0)
                {
                    loai.Status = true;
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

        public ActionResult DeleteLoaiSP(int? maLoai)
        {

            LOAISP loai = db.LOAISPs.SingleOrDefault(x => x.MaLoai == maLoai);
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loai);
        }

        [HttpPost, ActionName("DeleteLoaiSP")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLoaiConfirm(int maLoai)
        {
            LOAISP loai = db.LOAISPs.SingleOrDefault(x => x.MaLoai == maLoai);
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            loai.Status = false;
            db.Entry(loai).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DanhSachLoaiSP");
        }
        #endregion
        #region QUẢN LÝ NHÃN HIỆU
        public ActionResult DanhSachNhanHieu(string search, int? page, int? size)
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
            var listNH = from nh in db.NHANHIEUx 
                         where nh.Status == true
                         select nh;
            listNH = listNH.OrderBy(x => x.MaNhanHieu);
            if (!String.IsNullOrEmpty(search))
            {
                listNH = listNH.Where(x => x.TenNhanHieu.Contains(search));
                return View(listNH.ToPagedList(pageNumber, pageSize));
            }
            return View(listNH.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult AddNhanHieu()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNhanHieu(NHANHIEU nh)
        {
            if (ModelState.IsValid)
            {
                var tenNH = nh.TenNhanHieu;
                var check = db.NHANHIEUx.Where(x => x.TenNhanHieu == tenNH);
                if (check.Count() == 0)
                {
                    nh.Status = true;
                    db.NHANHIEUx.Add(nh);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Thêm nhãn hiệu thành công!";
                    return View(nh);
                }
                else
                {
                    TempData["ThongBao"] = "Tên nhãn hiệu đã tồn tại!";
                    return View(nh);
                }
            }
            else
            {
                TempData["ThongBao"] = "Thêm nhãn hiệu không thành công!";
            }
            return View();
        }
        public ActionResult EditNH(int? maNH)
        {
            NHANHIEU nh = db.NHANHIEUx.SingleOrDefault(x => x.MaNhanHieu == maNH);
            if (nh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nh);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditNH(NHANHIEU nh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nh).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Chỉnh sửa nhãn hiệu thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Chỉnh sửa nhãn hiệu không thành công!";
            }
            return View(nh);
        }
        public ActionResult DeleteNH(int? maNH)
        {

            NHANHIEU nh = db.NHANHIEUx.SingleOrDefault(x => x.MaNhanHieu == maNH);
            if (nh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nh);
        }

        [HttpPost, ActionName("DeleteNH")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNHConfirm(int maNH)
        {
            NHANHIEU nh = db.NHANHIEUx.SingleOrDefault(x => x.MaNhanHieu == maNH);
            if (nh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            nh.Status = false;
            db.Entry(nh).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DanhSachNhanHieu");
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
            var listCT = from ct in db.CHITIETSPs 
                         where ct.SANPHAM.Status == true
                         where ct.Status == true
                         select ct;
            listCT = listCT.OrderBy(x => x.Id);
            if (!String.IsNullOrEmpty(search))
            {
                listCT = listCT.Where(x => x.SANPHAM.TenSP.Contains(search));
                return View(listCT.ToPagedList(pageNumber, pageSize));
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
            ViewBag.MaMau = new SelectList(db.MAUSACs.OrderBy(x => x.MaMau).ToList(), "MaMau", "MauSac");
            ViewBag.MaSize = new SelectList(db.SIZEs.OrderBy(x => x.MaSize).ToList(), "MaSize", "Size");
            ViewBag.MaSP = new SelectList(db.SANPHAMs.OrderBy(x => x.MaSP).ToList(), "MaSP", "TenSP");
           
            if (ModelState.IsValid)
            {
                ct.MaSP = maSP;
                ct.MaMau = maMau;
                ct.MaSize = maSize;
                ct.Status = true;
                CHITIETSP ctsp = db.CHITIETSPs.SingleOrDefault(x => x.MaMau == maMau && x.MaSP == maSP && x.MaSize == maSize);
                if (ctsp != null)
                {
                    TempData["ThongBao"] = "Chi tiết sản phẩm đã tồn tại!";
                    return View();
                }
                else
                {
                    db.CHITIETSPs.Add(ct);

                    KHOHANG kho = new KHOHANG()
                    {
                        MaCT = ct.Id,
                        SoLuongBan = 0,
                        SoLuongCon = 0,
                    };
                    db.KHOHANGs.Add(kho);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Thêm chi tiết sản phẩm thành công!";
                    return View();
                }
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
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.Id == id);
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
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.Id == id);
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
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.Id == id);
            if (ct == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ct.Status = false;
            db.Entry(ct).State = EntityState.Modified;
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

            var listDH = from dh in db.DONHANGs select dh ;
            listDH = listDH.OrderBy(x => x.MaDH);
            if (!String.IsNullOrEmpty(search))
            {
                listDH = listDH.Where(x => x.KHACHHANG.TenKH.Contains(search));
                return View(listDH.ToPagedList(pageNumber, pageSize));
            }
            // nếu từ khóa null thì trả về list 
            return View(listDH.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DonHangChuaXN(string search, int? page, int? size)
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
                         where dh.TinhTrang == "Chờ xác nhận"
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
                dh.TinhTrang = "Đang giao";
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DonHangChuaXN", new { @page = page, @size = size });
            }
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
        #endregion
        #region VIEW THÔNG TIN CHUNG

        public ActionResult SoLuongDonHang()
        {
            List<DONHANG> listDH = new List<DONHANG>();
            foreach (var item in db.DONHANGs)
            {
                listDH.Add(item);
            }
            ViewBag.SoLuongDH = listDH.Count;
            return PartialView();
        }
        #region số lượng đơn hàng đã giao thành công
        public ActionResult SoLuongDHThanhCong()
        {
            List<DONHANG> listDH = new List<DONHANG>();
            foreach (var item in db.DONHANGs)
            {
                if (item.TinhTrang == "DONE") 
                    listDH.Add(item);
            }
            ViewBag.SoLuongDH = listDH.Count;
            return PartialView();
        }
        #endregion 
        public ActionResult TongDoanhThu()
        {
            var listDH = from dh in db.DONHANGs 
                         where dh.TinhTrang == "DONE"
                         select dh;
            decimal tongDT = 0;
            if (listDH == null)
            {
                ViewBag.TongDTDuKien = tongDT;
                return PartialView();
            }
            else
            {
                foreach (var item in listDH)
                {
                    decimal tong = item.CHITIETDONHANG.CHITIETSP.SANPHAM.DonGia * item.CHITIETDONHANG.SoLuong;
                    tongDT += tong;
                }
                ViewBag.TongDoanhThu = tongDT;
                return PartialView();
            }
        }
        #endregion
        #region Tong doanh thu dự kiến
        public ActionResult TongDTDuKien()
        {
            var listDH = from dh in db.DONHANGs
                         select dh;
            decimal tongDT = 0;
            if (listDH == null)
            {
                ViewBag.TongDTDuKien = tongDT;
                return PartialView();
            }    
            else
            {
                foreach (var item in listDH)
                {
                   decimal tong = item.CHITIETDONHANG.CHITIETSP.SANPHAM.DonGia * item.CHITIETDONHANG.SoLuong;
                    tongDT += tong;
                }    
                ViewBag.TongDTDuKien = tongDT;
                return PartialView();
            }    
           
        }
        #endregion
        #region Quản lý kho hàng

        public ActionResult DanhSachSPTrongKho(string search, int? page, int? size)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "40", Value = "40" });


            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.Size = items;
            ViewBag.CurrentSize = size;

            page = (page ?? 1);
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 20);
            var khoHang = from kho in db.KHOHANGs
                         select kho;
            khoHang = khoHang.OrderBy(x => x.Id);
            if (!String.IsNullOrEmpty(search))
            {
                khoHang = khoHang.Where(x => x.CHITIETSP.SANPHAM.TenSP.Contains(search));
                return View(khoHang.ToPagedList(pageNumber, pageSize));
            }
            return View(khoHang.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThayDoiSoLuong(int id)
        {
            KHOHANG kho = db.KHOHANGs.Find(id);
            if (kho == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.ID = id;
            return View(kho);
        }
        public ActionResult ThayDoiSoLuong(KHOHANG kho)
        {
            int soLuongCon = int.Parse(Request.Form["SoLuongCon"].ToString());
            int id = int.Parse(Request.Form["ID"].ToString());
            kho = db.KHOHANGs.Find(id);
            kho.SoLuongCon = soLuongCon;
            db.Entry(kho).State = EntityState.Modified;
            db.SaveChanges();
            TempData["ThongBao"] = "Thêm số lượng sản phẩm thành công!";
            return View();
        }
        #endregion
        #region Thông tin cá nhân
        public ActionResult InfoCaNhan()
        {
            int maNV = int.Parse(Session["MaNV"].ToString());

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
            maNV = int.Parse(Session["MaNV"].ToString());

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
            int maNV = int.Parse(Session["MaNV"].ToString());
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
        #endregion
    }
}