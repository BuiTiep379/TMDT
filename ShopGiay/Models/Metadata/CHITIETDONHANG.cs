using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    [MetadataType(typeof(CHITIETDONHANGMetadata))]
    public partial class CHITIETDONHANG
    {
    }
    public class CHITIETDONHANGMetadata
    {
        public int MaDH { get; set; }
        [Display(Name = "Mã chi tiết sản phẩm")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int MaCTSP { get; set; }
        [Display(Name = "Số lượng sản phẩm")]
        [Range(1, maximum: 100, ErrorMessage = ("Số lượng tối thiểu là 1 tối đa 100 sản phẩm"))]
        public int SoLuong { get; set; }

    }
}