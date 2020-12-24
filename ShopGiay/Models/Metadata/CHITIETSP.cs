using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    [MetadataType(typeof(CHITIETSPMetadata))]
    public partial class CHITIETSP
    {
    }
    public partial class CHITIETSPMetadata
    {
        [Display(Name = "Mã chi tiết sản phẩm")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Display(Name = "Mã sản phẩm")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int MaSP { get; set; }
        [Display(Name = "Mã size")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int MaSize { get; set; }
        [Display(Name = "Mã màu")]
        [Required(ErrorMessage ="{0} không được để trống")]
        public int MaMau { get; set; }
        [Display(Name = "Số lượng sản phẩm")]
        public int SoLuong { get; set; }
    }
}