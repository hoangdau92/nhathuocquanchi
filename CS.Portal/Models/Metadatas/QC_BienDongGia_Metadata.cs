using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_BienDongGia_Metadata))] 
    public partial class QC_BienDongGia
    {
        public class QC_BienDongGia_Metadata
        {

            [Display(Name = "Ký hiệu")]
            [Required(ErrorMessage = "Vui lòng nhập ký hiệu")]
            public string kyhieu { get; set; }

            [Display(Name = "Mô tả")]
            public string mota { get; set; }
        }
    }
}