using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_LoaiThuoc_Metadata))]
    public partial class QC_LoaiThuoc
    {
        public class QC_LoaiThuoc_Metadata
        {

            [Display(Name = "Tiêu đề")]
            [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
            public string ten { get; set; }

            [Display(Name = "Mô tả")]
            public string mota { get; set; }
        }
    }
}