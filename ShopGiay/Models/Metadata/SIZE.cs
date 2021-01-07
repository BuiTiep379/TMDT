using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    public partial class SIZE
    {

    }
    public class SIZEMetadata
    {
        [Display(Name = "Mã size")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaSize { get; set; }
        [Display(Name = "Tên loại sản phẩm")]
        [Required(ErrorMessage = "Tên loại sản phẩm không được để trống")]
        [Index(IsUnique = true)]
        public int Size { get; set; }
    }
}