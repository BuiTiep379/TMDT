using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopGiay.Models.Metadata
{
    [MetadataType(typeof(HINHANHSPMetadata))]
    public partial class HINHANHSP
    {
    }
    public partial class HINHANHSPMetadata
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Display(Name = "File Name")]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string FileName { get; set; }
    }
}