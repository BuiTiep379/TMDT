//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopGiay.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DONHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DONHANG()
        {
            this.CHITIETDONHANGs = new HashSet<CHITIETDONHANG>();
        }
    
        public int MaDH { get; set; }
        public int MaKH { get; set; }
        public Nullable<int> PromoID { get; set; }
        public System.DateTime NgayDatHang { get; set; }
        public Nullable<System.DateTime> NgayGiaoHang { get; set; }
        public string DiaChiGiao { get; set; }
        public decimal TongTien { get; set; }
        public string ThanhToan { get; set; }
        public string TinhTrang { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public Nullable<int> SellerID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETDONHANG> CHITIETDONHANGs { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }
        public virtual KHACHHANG KHACHHANG1 { get; set; }
    }
}
