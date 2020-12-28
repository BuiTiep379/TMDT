using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.Net;
using System.Data.Entity;
using System.IO;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
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
        ShopGiayEntities db = new ShopGiayEntities();

        // GET: Admin/Login
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
            nv = db.NHANVIENs.SingleOrDefault(n => n.Email == email && n.MatKhau == fMatKhau);
            if (nv != null)
            {
                if (nv.QuyenNV == "1")
                {
                    Session["TaiKhoanNV"] = nv;
                    Session["User"] = nv.TenNV;
                    Session["Quyen"] = nv.QuyenNV;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session["TaiKhoanNV"] = nv;
                    Session["User"] = nv.TenNV;
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
 
        //create a string MD5
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

        public ActionResult DanhSachNV(string search, int? page, int? size)
        {
            //Tạo list pagesize 
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            // giữ kích thước trang được chọn trên Droplist
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.Size = items;// viewbag dropdownlist
            ViewBag.CurrentSize = size;

            page = page ?? 1; // nếu null page =1
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 5);

            var listNV = from nv in db.NHANVIENs select nv;
            listNV = listNV.OrderBy(x => x.MaNV);
            if (!String.IsNullOrEmpty(search))
            {
                listNV = listNV.Where(x => x.TenNV.Contains(search));
                return View(listNV.ToPagedList(pageNumber, pageSize));
            }
            // nếu từ khóa null thì trả về list nhân viên ban đầu
            return View(listNV.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult AddNhanVien()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNhanVien(FormCollection form)
        {
            string tenNV = form["TenNV"].ToString();
            string diaChi = form["DiaChi"].ToString();
            string email = form["Email"].ToString();
            string sdt = form["Sdt"].ToString();
            string gioiTinh = form["GioiTinh"].ToString();
            string ngaySinh = form["NgaySinh"].ToString();
            string cmnd = form["CMND"].ToString();
            string matKhau = form["MatKhau"].ToString();
            string xnMatKhau = form["XacNhanMatKhau"].ToString();
            if (matKhau != xnMatKhau)
            {
                ViewBag.Error = "Xác nhận mật khẩu không trùng khớp";
                return View();
            }    
            NHANVIEN nv = db.NHANVIENs.SingleOrDefault(x => x.Email == email);
            if (nv == null)
            {
                matKhau = GetMD5(matKhau);
                nv = new NHANVIEN
                {
                    TenNV = tenNV,
                    DiaChi = diaChi,
                    Email = email,
                    Sdt = sdt,
                    GioiTinh = gioiTinh,
                    NgaySinh = DateTime.Parse(ngaySinh),
                    CMND = cmnd,
                    MatKhau = matKhau,
                };
                
                
                // thêm dữ liệu vào bảng nhân viên
                db.NHANVIENs.Add(nv);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm nhân viên thành công!";
                return View();
            }
            else
            {
                TempData["ThongBao"] = "Email đã tồn tại!";
                return View();
            }
            
        }

        public ActionResult DeleteNhanVien(int? maNV)
        {
            if (maNV == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nv = db.NHANVIENs.Find(maNV);
            if (nv == null)
            {
                return HttpNotFound();
            }
            return View(nv);
        }
        [HttpPost, ActionName("DeleteNhanVien")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNVConfirm(int maNV)
        {
            NHANVIEN nv = db.NHANVIENs.Find(maNV);
            db.NHANVIENs.Remove(nv);
            db.SaveChanges();
            return RedirectToAction("DanhSachNV");
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
        public ActionResult DeleteKHConfirm(int maKH)
        {
            KHACHHANG kh = db.KHACHHANGs.Find(maKH);
            db.KHACHHANGs.Remove(kh);
            db.SaveChanges();
            return RedirectToAction("DanhSachKH");
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

        public ActionResult DanhSachPromoCode(string search, int? page, int? size)
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
            var listCode = from c in db.PROMOCODEs select c;
            listCode = listCode.OrderBy(x => x.Id);

            if (search != null)
            {
                listCode = listCode.Where(x => x.Name.Contains(search));
            }
            return View(listCode.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult KichHoatCode(int id)
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
                return RedirectToAction("DanhSachPromoCode");
            }
        }
        public ActionResult HuyCode(int id)
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
                return RedirectToAction("DanhSachPromoCode");
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
    }
}