using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ShopGiay.Models
{
    public class GIOHANG
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        [Display(Name ="Mã size")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int MaSize { get; set; }
        [Display(Name = "Tên size")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int TenSize { get; set; }

        public int MaMau { get; set; }

        public string TenMau { get; set; }

        [Range(1, maximum:100, ErrorMessage = ("Số lượng tối thiểu là 1 tối đa 10 sản phẩm"))]
        public int SoLuong { get; set; }
        public decimal ThanhTien { get { return (SoLuong * DonGia); } }
        ShopGiayEntities db = new ShopGiayEntities();
        public GIOHANG(int maSP, int maMau, int maSize)
        {
            MaSP = maSP;
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSP == maSP);
            TenSP = sp.TenSP;
            HinhAnh = sp.Anh;
            DonGia = decimal.Parse(sp.DonGia.ToString());
            SoLuong = 1;
            CHITIETSP ct = db.CHITIETSPs.SingleOrDefault(x => x.MaSP == maSP && x.MaMau == maMau && x.MaSize == maSize);
            MaMau = ct.MaMau;
            MaSize = ct.MaSize;
            MAUSAC mau = db.MAUSACs.SingleOrDefault(x => x.MaMau == maMau);
            TenMau = mau.MauSac;
            SIZE size = db.SIZEs.SingleOrDefault(x => x.MaSize == maSize);
            TenSize = size.Size;
        }
    }
}