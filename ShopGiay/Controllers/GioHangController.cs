using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using System.Data.Entity;


namespace ShopGiay.Controllers
{
    public class GioHangController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: GioHang
        public List<GIOHANG> LayGioHang()
        {
            List<GIOHANG> listGioHang = Session["GIOHANG"] as List<GIOHANG>;
            // Nếu giỏ hàng chưa tồn tại 
            if (listGioHang == null)
            {
                // khởi tạo giỏ hang
                listGioHang = new List<GIOHANG>();
                Session["GIOHANG"] = listGioHang;
            }
            return listGioHang;
        }

        public ActionResult GioHang()
        {
            int maKH = int.Parse(Session["UserID"].ToString());
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
            ViewBag.MaKH = maKH;
            ViewBag.Email = kh.Email;
            ViewBag.DiaChi = kh.DiaChi;
            ViewBag.Sdt = kh.Sdt;
            if (Session["Code"] != null)
            {
                Session.Remove("Code");
                Session.Remove("TongGiam");
                Session.Remove("GIOHANG2");
                List<GIOHANG> listGioHang = LayGioHang();
                foreach (var item in listGioHang)
                {
                    int maSP = item.MaSP;
                    SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSP == maSP);
                    item.DonGia = decimal.Parse(sp.DonGia.ToString());
                }
                TempData["TongTien"] = TongTien();
                return View(listGioHang);
            }
            if (Session["GIOHANG"] == null)
            {
                TempData["KhongCo"] = "Cart is empty";
                return View();
            }
            else
            {
                List<GIOHANG> listGioHang = LayGioHang();
                TempData["TongTien"] = TongTien();
                Session["TongTien"] = TongTien();
                return View(listGioHang);
            }    
        }
        // Tính tổng số lượng sản phẩm trong kho
        private int TongSoLuong()
        {
            int tongsoluong = 0;
            List<GIOHANG> listGioHang = Session["GIOHANG"] as List<GIOHANG>;
            if (listGioHang != null)
            {
                tongsoluong = listGioHang.Sum(n => n.SoLuong);
            }
            return tongsoluong;
        }

        //private List<GIOHANG> ApplyCode(string code, string message)
        //{
        //    List<GIOHANG> listGioHang = LayGioHang();
        //    message = "";
        //    if (code != null)
        //    {
        //        PROMOCODE check = db.PROMOCODEs.SingleOrDefault(x => x.Code == code);
        //        if (check == null)
        //        {
        //            message = "0";
        //            return listGioHang; // "Mã giảm giá không chính xác";
        //        }
        //        else
        //        {
        //            if (check.Status == false)
        //            {
        //                message = "-1";//"Mã giảm giá không còn áp dụng!";
        //                return listGioHang; // 
        //            }
        //            else
        //            {
        //                Session["Code"] = code;
        //                List<NHANHIEU> listNH = db.NHANHIEUx.OrderBy(x => x.MaNhanHieu).ToList();
        //                foreach (var item in listNH)
        //                {
        //                    if (check.Code.Contains(item.TenNhanHieu))
        //                    {
        //                        int maNH = item.MaNhanHieu;
        //                        foreach (var sp in listGioHang)
        //                        {
        //                            int nhanHieu = (int)db.SANPHAMs.SingleOrDefault(x => x.MaSP == sp.MaSP).MaNhanHieu;
        //                            if (nhanHieu == maNH)
        //                            {
        //                                decimal giam = sp.DonGia * check.Value;
        //                                sp.DonGia -= giam;
        //                            }
        //                        }
        //                    }
        //                    message = "1";
        //                    return listGioHang;
        //                }
        //            }
                    
        //        }
        //    }
        //    return listGioHang;
        //}
        private int TongSoLuongSP(int maSP)
        {
            int tongsoluong = 0;
            List<GIOHANG> listGioHang = Session["GIOHANG"] as List<GIOHANG>;
            if (listGioHang != null)
            {
                tongsoluong = listGioHang.Where(n => n.MaSP == maSP).Sum(n => n.SoLuong);
            }
            return tongsoluong;
        }
        private decimal TongTien()
        {
            decimal tongtien = 0;
            List<GIOHANG> listGioHang = Session["GIOHANG"] as List<GIOHANG>;
            if (listGioHang != null)
            {
                tongtien = listGioHang.Sum(n => n.ThanhTien);
            }
            if (Session["TongGiam"] != null)
            {
                decimal tongGiam = decimal.Parse(Session["TongGiam"].ToString());
                tongtien -= tongGiam;
            }   
            return tongtien;
        }
        //private decimal TongTien2()
        //{
        //    decimal tongtien = 0;
        //    List<GIOHANG> listGioHang = Session["GIOHANG2"] as List<GIOHANG>;
        //    if (listGioHang != null)
        //    {
        //        tongtien = listGioHang.Sum(n => n.ThanhTien);
        //    }
        //    Session["TongTien2"] = tongtien;
        //    return tongtien;
        //}

        public ActionResult ThemSanPham(int maSP, string url)
        {
          
            //lấy mã màu mã  size từ dropdownlist
            int maMau = int.Parse(Request.Form["MaMau"]);
            int maSize = int.Parse(Request.Form["MaSize"]);
            // kiểm tra sự tồn tại của sản phẩm
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // kiểm tra sự tồn tại màu sắc và size của đôi giày trong kho
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.MaSP == maSP && x.MaSize == maSize && x.MaMau == maMau);
            if (ct == null)
            {
                TempData["KhongThanhCong"] = "Products are out of stock. You can try different colors or sizes!";
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai });
            }
            //Lấy ra session giỏ hàng
            List<GIOHANG> listGioHang = LayGioHang();
            //Kiểm tra sản phẩm đã tồn tại trong giỏ hàng chưa?
            GIOHANG sanpham = listGioHang.Find(x => x.MaSP == maSP && x.MaMau == maMau && x.MaSize == maSize);
            var slsp = db.CHITIETSPs.SingleOrDefault(n => n.MaSP == maSP && n.MaMau == maMau && n.MaSize == maSize).SoLuong;
            TempData["TongSoLuong"] = TongSoLuong();
            if (TongSoLuongSP(maSP) >= slsp && sanpham != null)
            {
                TempData["LoiSL"] = "The product does not have the required quantity. Please choose less!";
                ModelState.AddModelError("LoiSP", "");
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai });
            }
            if (sanpham == null)
            {
                sanpham = new GIOHANG(maSP, maMau, maSize);
                listGioHang.Add(sanpham);
                TempData["ThanhCong"] = "Add to cart successfully";
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai });
            }
            else
            {
                sanpham.SoLuong++;
                TempData["ThanhCong"] = "Add to cart successfully";
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai });
            }
        }

        public ActionResult CapNhatGioHang(int maSP, int maMau, int maSize, FormCollection f)
        {
            
            int check = Int32.Parse(Request.Form["txtSoLuong"]);// số lượng giày cần mua
            var soLuongGiay = db.CHITIETSPs.SingleOrDefault(x => x.MaSP == maSP && x.MaMau == maMau && x.MaSize == maSize).SoLuong;// số lượng giày trong kho
            if (check > soLuongGiay)
            {
                TempData["LoiSL"] = "Sản phẩm hiện không đủ số lượng. Vui lòng chọn ít hơn!";
                ModelState.AddModelError("LoiSP", " ");
                return RedirectToAction("GioHang", "GioHang");
            }
            if (check < 1)
            {
                TempData["LoiSL"] = "Không thể mua với số lượng nhỏ hơn 1. Vui lòng chọn nhiều hơn!";
                ModelState.AddModelError("LoiSL", " ");
                return RedirectToAction("GioHang", "GioHang");
            }
            else
            {
                SANPHAM sp = db.SANPHAMs.SingleOrDefault(n => n.MaSP == maSP);
                if (sp == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                List<GIOHANG> listGioHang = LayGioHang();
                GIOHANG sanPham = listGioHang.SingleOrDefault(n => n.MaSP == maSP && n.MaMau == maMau && n.MaSize == maSize);
                if (sanPham != null)
                {
                    sanPham.SoLuong = int.Parse(f["txtSoLuong"].ToString());
                }
                return RedirectToAction("GioHang", "GioHang");
            }
        }
        public ActionResult XoaGioHang(int maSP, int maMau, int maSize)
        {
            // kiểm tra sản phẩm 
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(n => n.MaSP == maSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            List<GIOHANG> listGioHang = LayGioHang();
            GIOHANG sanpham = listGioHang.SingleOrDefault(n => n.MaSP == maSP && n.MaMau == maMau && n.MaSize == maSize);
            if (sanpham != null)
            {
                listGioHang.RemoveAll(n => n.MaSP == maSP && n.MaMau == maMau && n.MaSize == maSize);

            }
            if (listGioHang.Count == 0)
            {
                return RedirectToAction("Shop", "Store");
            }
            return RedirectToAction("GioHang");
        }
        [ChildActionOnly]
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }

            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        public ActionResult ThanhToan(int maKH)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Store");
            }
            else
            {
                if (Session["GioHang"] == null)
                {
                    TempData["ThongBao"] = "Giỏ hàng của bạn trống";
                    return RedirectToAction("Shop", "Store");
                }
                else
                {
                    if (Session["GIOHANG2"] != null)
                    {
                        List<GIOHANG> listGioHang2 = Session["GIOHANG2"] as List<GIOHANG>;
                        foreach (var item in listGioHang2)
                        {
                            int maSP = item.MaSP;
                            SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSP == maSP);
                            item.DonGia = decimal.Parse(sp.DonGia.ToString());
                        }
                        decimal tongTien = listGioHang2.Sum(n => n.ThanhTien);
                        decimal tongGiam = decimal.Parse(Session["TongGiam"].ToString());
                        decimal tongTien2 = tongTien - tongGiam;
                        ViewBag.TongTien = tongTien;
                        ViewBag.TongGiam = tongGiam;
                        TempData["TongTien"] = tongTien2;
                        Session["TongTien2"] = tongTien2;
                        maKH = int.Parse(Session["UserID"].ToString());
                        KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
                        ViewBag.TenKH = kh.TenKH;
                        ViewBag.DiaChi = kh.DiaChi;
                        ViewBag.Sdt = kh.Sdt;
                        return View(listGioHang2);
                    }
                    else
                    {
                        List<GIOHANG> listGioHang = Session["GIOHANG"] as List<GIOHANG>;
                        TempData["TongTien"] = TongTien();
                        if (Session["TongGiam"] == null)
                        {
                            ViewBag.TongGiam = "0";
                        }    
                        ViewBag.TongTien = listGioHang.Sum(n => n.ThanhTien);
                        maKH = int.Parse(Session["UserID"].ToString());
                        KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
                        ViewBag.TenKH = kh.TenKH;
                        ViewBag.DiaChi = kh.DiaChi;
                        ViewBag.Sdt = kh.Sdt;
                        return View(listGioHang);
                    }
                }
            }

        }
        [HttpPost, ActionName("ThanhToan")]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanThanhToan(int maKH)
        {
            string code = Session["Code"].ToString();
            PROMOCODE check = db.PROMOCODEs.SingleOrDefault(x => x.Code == code);
            int promoID = check.Id;
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
            DONHANG dh = new DONHANG()
            {
                MaKH = int.Parse(Session["UserID"].ToString()),
                PromoID = promoID,
                NgayDatHang = DateTime.Now,
                NgayGiaoHang = null,
                DiaChiGiao = kh.DiaChi,
                TongTien = TongTien(),
                ThanhToan = "Cash",
                TinhTrang = "Chưa xác nhận",
                HoTen = kh.TenKH,
                Email = kh.Email,
                Sdt = kh.Sdt
            };

            db.DONHANGs.Add(dh);
            db.SaveChanges();
            List<GIOHANG> listGioHang = LayGioHang();
            foreach (var item in listGioHang)
            {
                int maSP = item.MaSP;
                int maSize = item.MaSize;
                int maMau = item.MaMau;
                int soLuong = item.SoLuong;
                CHITIETSP ctsp = db.CHITIETSPs.SingleOrDefault(x => x.MaSP == maSP && x.MaMau == maMau && x.MaSize == maSize);
                ctsp.SoLuong -= soLuong;
                db.Entry(ctsp).State = EntityState.Modified;
                db.SaveChanges();
            }
            foreach (var item in listGioHang)
            {
                int maSP = item.MaSP;
                int maSize = item.MaSize;
                int maMau = item.MaMau;
                int soLuong = item.SoLuong;
                decimal donGia = item.ThanhTien;
                CHITIETDONHANG ctdh = new CHITIETDONHANG()
                {
                    MaDH = dh.MaDH,
                    MaSP = maSP,
                    MaMau = maMau,
                    MaSize = maSize,
                    SoLuong = soLuong,
                    DonGia = donGia
                };
                db.CHITIETDONHANGs.Add(ctdh);
                db.SaveChanges();
            }
            Session.Remove("GIOHANG");
            return RedirectToAction("ThanksYou");
        }
        public ActionResult ThanksYou()
        {
            int maKH = int.Parse(Session["UserID"].ToString());
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
            ViewBag.TenKhachHang = kh.TenKH;
            return View();
        }
        [HttpGet]
        public ActionResult ThayDoiDiaChi(int maKH)
        {
            var kh = db.KHACHHANGs.Find(maKH);
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View();  
        }
        [HttpPost]
        public ActionResult ThayDoiDiaChi(KHACHHANG kh)
        {
            string diaChi = Request.Form["DiaChi"].ToString();
            var maKH = int.Parse(Session["UserID"].ToString());
            kh = db.KHACHHANGs.Find(maKH);
            if (kh.DiaChi == diaChi)
            {
                return View();
            }    
            else
            {
                kh.DiaChi = diaChi;
                db.Entry(kh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ThanhToan", new { maKH = maKH});
            }    
        }
        [HttpGet]
        public ActionResult ApplyPromoCode(int maKH)
        {
            var kh = db.KHACHHANGs.Find(maKH);
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View();
        }

        [HttpPost]
        public ActionResult ApplyPromoCode(FormCollection form)
        {
            string code = form["txtcode"].ToString();
            List<GIOHANG> listGioHangMoi = Session["GIOHANG"] as List<GIOHANG>;
            if (code != null)
            {
                PROMOCODE check = db.PROMOCODEs.SingleOrDefault(x => x.Code == code);
                if (check == null)
                {
                    ViewBag.Error = "Mã giảm giá không chính xác";
                    return View();
                }
                else
                {
                    if (check.Status == false)
                    {
                        ViewBag.Error = "Mã giảm giá áp dụng thành công!";
                        return View(); // 
                    }
                    else
                    {
                        Session["Code"] = code;
                        List<NHANHIEU> listNH = db.NHANHIEUx.OrderBy(x => x.MaNhanHieu).ToList();
                        foreach (var item in listNH)
                        {
                            if (check.Code.Contains(item.TenNhanHieu))
                            {
                                decimal tongGiam = 0;
                                int maNH = item.MaNhanHieu;
                                foreach (var sp in listGioHangMoi)
                                {
                                    
                                    int nhanHieu = (int)db.SANPHAMs.SingleOrDefault(x => x.MaSP == sp.MaSP).MaNhanHieu;
                                    if (nhanHieu == maNH)
                                    {
                                        decimal giam = sp.DonGia * check.Value;
                                        tongGiam += giam;
                                        sp.DonGia -= giam;
                                    }
                                }
                                Session["TongGiam"] = tongGiam;
                                List<GIOHANG> listGioHang2 = listGioHangMoi;
                                Session["GIOHANG2"] = listGioHangMoi;
                                return RedirectToAction("ThanhToan", new { @maKH = Session["UserID"] });
                            }
                            
                        }
                    }

                }
            }
            return View();
        }
        public ActionResult HuyThanhToan(int maKH)
        {
            Session.Remove("GIOHANG2");
            Session.Remove("Code");
            return RedirectToAction("GioHang");
        }
    }
    
}