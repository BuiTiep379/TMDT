using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ShopGiay.Models
{
    [MetadataType(typeof(NHANVIENMetadata))]
    public partial class NHANVIEN
    {

    }
    public partial class NHANVIENMetadata
    {
        [Display(Name = "Mã nhân viên")]
        [Key]
        public int MaNV { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Tên nhân viên")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} không thỏa mãn")]

        public string TenNV { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "{0} không hợp lệ")]

        public string DiaChi { get; set; }
        [Display(Name = "Địa chỉ email")]
        [Index(IsUnique = true)]
        [Required(ErrorMessage = "{0} không được để trống")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Vui lòng nhập đúng đinh dạng là Email")]

        public string Email { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
               ErrorMessage = "Số điện thoại không đúng!")]
        public string Sdt { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Ngày sinh")]
        public Nullable<System.DateTime> NgaySinh { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Số chứng minh thư")]
        public string CMND { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$", ErrorMessage = "Mật khẩu phải tối thiểu sáu ký tự, ít nhất một chữ cái và một số")]//Tối thiểu tám ký tự, ít nhất một chữ cái và một số
        public string MatKhau { get; set; }
        [Display(Name = "Quyền nhân viên")]
        public string QuyenNV { get; set; }
    }
}