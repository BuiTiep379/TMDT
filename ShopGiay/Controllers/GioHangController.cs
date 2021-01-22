using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using System.Data.Entity;
using PayPal.Api;
using Newtonsoft.Json.Linq;
using ShopGiay;
using System.Configuration;

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
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Store");
            }    

            int maKH = int.Parse(Session["UserID"].ToString());
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
            ViewBag.MaKH = maKH;
            ViewBag.Email = kh.Email;
            ViewBag.DiaChi = kh.DiaChi;
            ViewBag.Sdt = kh.Sdt;
            Session["TongGiam"] = "0";
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
       
        private int TongSoLuong()
        {
            int tongsoluong = 0;
            List<GIOHANG> listGioHang = Session["GIOHANG"] as List<GIOHANG>;
            if (listGioHang != null)
            {
                tongsoluong = listGioHang.Count();
            }
            return tongsoluong;
        }

       
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
            return tongtien;
        }
        

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
            var check = db.KHOHANGs.SingleOrDefault(x => x.MaCT == ct.Id).SoLuongCon;
            if (check == 0)
            {
                TempData["KhongThanhCong"] = "Sản phẩm đã hết hàng. Vui lòng chọn size và màu khách";
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai});
            }
            //Lấy ra session giỏ hàng
            List<GIOHANG> listGioHang = LayGioHang();
            //Kiểm tra sản phẩm đã tồn tại trong giỏ hàng chưa?
            GIOHANG sanpham = listGioHang.Find(x => x.MaSP == maSP && x.MaMau == maMau && x.MaSize == maSize);
            CHITIETSP ct2 = db.CHITIETSPs.SingleOrDefault(n => n.MaSP == maSP && n.MaMau == maMau && n.MaSize == maSize);
            var slsp = db.KHOHANGs.SingleOrDefault(x => x.MaCT == ct.Id).SoLuongCon;
            TempData["TongSoLuong"] = TongSoLuong();
            if (TongSoLuongSP(maSP) >= slsp && sanpham != null)
            {
                TempData["LoiSL"] = "Sản phẩm hiện tại không đủ số lượng. Vui lòng chọn ít hơn!";
                ModelState.AddModelError("LoiSP", "");
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai });
            }
            if (sanpham == null)
            {
                sanpham = new GIOHANG(maSP, maMau, maSize);
                listGioHang.Add(sanpham);
                TempData["ThanhCong"] = "Thêm sản phẩm vào giỏ hàng thành công";
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai });
            }
            else
            {
                sanpham.SoLuong++;
                TempData["ThanhCong"] = "Thêm sản phẩm vào giỏ hàng thành công";
                return RedirectToAction("XemChiTiet", "SanPham", new { @maSP = sp.MaSP, @maNhanHieu = sp.MaNhanHieu, @maLoai = sp.MaLoai });
            }
        }

        public ActionResult CapNhatGioHang(int maSP, int maMau, int maSize, FormCollection f)
        {
            
            int check = Int32.Parse(Request.Form["txtSoLuong"]);// số lượng giày cần mua
            var ct = db.CHITIETSPs.SingleOrDefault(x => x.MaSP == maSP && x.MaMau == maMau && x.MaSize == maSize);
            var slsp = db.KHOHANGs.SingleOrDefault(x => x.MaCT == ct.Id).SoLuongCon;// số lượng giày trong kho
            if (check > slsp)
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

        public ActionResult ThanhToan()
        {
            int maKH = int.Parse(Session["UserID"].ToString());
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
        [HttpPost, ActionName("ThanhToan")]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanThanhToan()
        {
            int maKH = int.Parse(Session["UserID"].ToString());
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
            DONHANG dh = new DONHANG()
            {
                MaKH = int.Parse(Session["UserID"].ToString()),
                NgayDatHang = DateTime.Now,
                DiaChiGiao = kh.DiaChi,
                TongTien = TongTien(),
                ThanhToan = "Chưa thanh toán",
                TinhTrang = "Chờ xác nhận",
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
                
                CHITIETDONHANG ctdh = new CHITIETDONHANG()
                {
                    MaDH = dh.MaDH,
                    MaCTSP = ctsp.Id,
                    SoLuong = soLuong,
                };
                KHOHANG slsp = db.KHOHANGs.SingleOrDefault(x => x.MaCT == ctsp.Id);// số lượng giày trong kho
                slsp.SoLuongCon -= soLuong;
                slsp.SoLuongBan += soLuong;
                db.CHITIETDONHANGs.Add(ctdh);
                db.Entry(slsp).State = EntityState.Modified;
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
        public ActionResult HuyThanhToan(int maKH)
        {
            return RedirectToAction("GioHang");
        }

       
        public decimal DoiTien(decimal st)
        {

            decimal USD;
            USD = 22602;
            return (st) / USD;
        }


        public ActionResult PaymentWithPaypal(string Cancel = null)
        {

            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/GioHang/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("Failure");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("Failure");
            }
            int maKH = int.Parse(Session["UserID"].ToString());
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(x => x.MaKH == maKH);
            DONHANG dh = new DONHANG()
            {
                MaKH = int.Parse(Session["UserID"].ToString()),
                NgayDatHang = DateTime.Now,
                DiaChiGiao = kh.DiaChi,
                TongTien = TongTien(),
                ThanhToan = "Đã thanh toán",
                TinhTrang = "Chờ xác nhận",
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

                CHITIETDONHANG ctdh = new CHITIETDONHANG()
                {
                    MaDH = dh.MaDH,
                    MaCTSP = ctsp.Id,
                    SoLuong = soLuong,
                };
                KHOHANG slsp = db.KHOHANGs.SingleOrDefault(x => x.MaCT == ctsp.Id);// số lượng giày trong kho
                slsp.SoLuongCon -= soLuong;
                slsp.SoLuongBan += soLuong;
                db.CHITIETDONHANGs.Add(ctdh);
                db.Entry(slsp).State = EntityState.Modified;
                db.SaveChanges();
            }
            Session.Remove("GIOHANG");
            return RedirectToAction("ThanksYou");
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            List<GIOHANG> listGioHang = LayGioHang();
            decimal tongTien = TongTien();
            decimal tongTienDoi = DoiTien(tongTien);
            var itemList = new ItemList() { items = new List<Item>() };
            foreach (var sp in listGioHang)
            {
                decimal donGia = DoiTien(sp.DonGia);
                itemList.items.Add(new Item()
                {
                    name = sp.TenSP,
                    description = "Size: " + sp.TenSize + "  Màu sắc: " + sp.TenMau,
                    currency = "USD",
                    price = donGia.ToString("F"),
                    quantity = sp.SoLuong.ToString(),
                });
            }

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            // Adding Tax, shipping and Subtotal details
            var details = new Details()
            {
                subtotal = tongTienDoi.ToString("F"),
            };

            //Final amount with details
            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDecimal(details.subtotal)).ToString(), // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();
            // Adding description about the transaction
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = itemList
            });


            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return payment.Create(apiContext);
      
        }
    }
}

