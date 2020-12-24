using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShopGiay.Models
{
    [MetadataType(typeof(NHANHIEUMetadata))]
    public partial class NHANHIEU
    {
    }
    public partial class NHANHIEUMetadata
    {
        [Display(Name = "Mã nhãn hiệu")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNhanHieu { get; set; }
        [Display(Name = "Tên nhãn hiệu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [Index(IsUnique = true)]
        public string TenNhanHieu { get; set; }
    }
}