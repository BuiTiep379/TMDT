using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    [MetadataType(typeof(SANPHAMMetadata))]
    public partial class SANPHAM
    {
    }
    public class SANPHAMMetadata
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "Mã sản phẩm")]
        public int MaSP { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSP { get; set; }
        [Display(Name = "Loại sản phẩm")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public Nullable<int> MaLoai { get; set; }
        [Display(Name = "Nhãn hiệu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public Nullable<int> MaNhanHieu { get; set; }
        [Display(Name = "Ảnh")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Anh { get; set; }
      
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Giá bán")]
        public Nullable<decimal> DonGia { get; set; }
        [Display(Name = "Hình ảnh 2")]
        public string Anh2 { get; set; }

        [Display(Name = "Hình ảnh 3")]
        public string Anh3 { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Ngày cập nhật")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime NgayCapNhat { get; set; }
    }

}