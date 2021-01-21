using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    public partial class DIACHIKH
    {
    }
    public class DIACHIKHMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã")]
        public int Id { get; set; }
        [Display(Name = "Mã khách hàng")]
        public int UserID { get; set; }
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Status { get; set; }
    }
}