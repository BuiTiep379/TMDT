using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    public partial class KHOHANG
    {
    }
    public class KHOHANGMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã kho hàng")]
        public int Id { get; set; }
        [Display(Name = "Mã chi tiết sản phẩm")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public Nullable<int> MaCT { get; set; }
        [Display(Name = "Số lượng bán")]
        public Nullable<int> SoLuongBan { get; set; }
        [Display(Name = "Số lượng còn")]
        public Nullable<int> SoLuongCon { get; set; }
    }
}