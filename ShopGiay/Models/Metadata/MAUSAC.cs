using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    [MetadataType(typeof(MAUSACMetadata))]
    public partial class MAUSAC
    {
    }
    public class MAUSACMetadata
    {
        [Display(Name = "Mã màu")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaMau { get; set; }
        [Display(Name = "Tên màu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string MauSac { get; set; }
    }
}