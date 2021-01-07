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
                    Session["UserID"] = nv.MaNV;
                    Session["TenNV"] = nv.TenNV;
                    Session["Quyen"] = nv.QuyenNV;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session["UserID"] = nv.MaNV;
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

       
        public ActionResult SoLuongNhanVien()
        {
            var slnv = db.NHANVIENs.OrderBy(m => m.MaNV).Count();
            ViewBag.SoLuongNV = slnv;
            return PartialView();
        }

        public ActionResult SoLuongKhachHang()
        {
            var slkh = db.KHACHHANGs.OrderBy(m => m.MaKH).Count();
            ViewBag.SoLuongKH = slkh;
            return PartialView();
        }
      

        public ActionResult InfoCaNhan(int? maNV)
        {
            maNV = int.Parse(Session["UserID"].ToString());

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
                ViewBag.TenKH = nv.TenNV;
                return View(nv);
            }
        }


    }
}