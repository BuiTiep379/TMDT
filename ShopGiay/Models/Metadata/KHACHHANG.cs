using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    [MetadataType(typeof(KHACHHANGMetadata))]
    public partial class KHACHHANG
    {
        
    }
    public class KHACHHANGMetadata
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Display(Name = "Mã khách hàng")]
        public int MaKH { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Họ tên")]
        public string TenKH { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string DiaChi { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Email")]
        [Index(IsUnique = true)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Vui lòng nhập đúng đinh dạng là Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
             ErrorMessage = "{0} không đúng!")]
        public string Sdt { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$", ErrorMessage = "Mật khẩu phải tối thiểu sáu ký tự, ít nhất một chữ cái và một số")]
        public string MatKhau { get; set; }
    }
}