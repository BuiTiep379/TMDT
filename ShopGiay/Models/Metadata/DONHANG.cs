using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    [MetadataType(typeof(DONHANGMetadata))]
    public partial class DONHANG
    {

    }
    public class DONHANGMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã đơn hàng")]
        public int MaDH { get; set; }
        [Display(Name = "Mã khách hàng")]
        public Nullable<int> MaKH { get; set; }
        [Display(Name = "Ngày đặt hàng")]
        public System.DateTime NgayDatHang { get; set; }
       
        [Display(Name = "Địa chỉ giao hàng")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string DiaChiGiao { get; set; }
        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }
        [Display(Name = "Thanh toán")]
        public string ThanhToan { get; set; }
        [Display(Name = "Tình trạng")]
        public string TinhTrang { get; set; }
    }
}