using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_DonViThuoc_Metadata))]
    public partial class QC_DonViThuoc
    {
        public class QC_DonViThuoc_Metadata
        {

            [Display(Name = "Tiêu đề")]
            [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
            public string ten { get; set; }

            [Display(Name = "Mô tả")]
            public string mota { get; set; } 
        }
    }
}