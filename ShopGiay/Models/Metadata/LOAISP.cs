using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShopGiay.Models
{
    [MetadataType(typeof(LOAISPMetadata))]
    public partial class LOAISP
    {
    }
    public class LOAISPMetadata
    {
        [Display(Name ="Mã loại sản phẩm")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLoai { get; set; }
        [Display(Name = "Tên loại sản phẩm")]
        [Required(ErrorMessage ="Tên loại sản phẩm không được để trống")]
        [Index(IsUnique = true)]
        public string TenLoai { get; set; }
    }
}