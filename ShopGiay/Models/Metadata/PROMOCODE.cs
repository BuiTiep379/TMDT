using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models
{
    [MetadataType(typeof(PROMOCODEMetadata))]
    public partial class PROMOCODE
    {
    }
    public class PROMOCODEMetadata
    {
        [Display(Name = "Số thứ tự code")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Tên code")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "{0} không được để trống")]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        [Display(Name = "Giá trị code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} không được để trống")]
        public decimal Value { get; set; }
        [Display(Name= "Mã code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} không được để trống")]
        public string Code { get; set; }
        [Display(Name = "Trạng thái code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} không được để trống")]
        public Nullable<bool> Status { get; set; }
    }
}