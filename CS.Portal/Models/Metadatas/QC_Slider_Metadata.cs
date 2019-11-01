using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_Slider_Metadata))]
    public partial class QC_Slider
    {
        public class QC_Slider_Metadata
        {

            [Display(Name = "Tiêu đề chính")]
            public string tieudechinh { get; set; }

            [Display(Name = "Tiêu đề phụ")]
            public string tieudephu { get; set; }

            [Display(Name = "Đường dẫn ảnh")]
            [Required(ErrorMessage = "Vui lòng nhập đường dẫn ảnh")]
            public string duongdananh { get; set; }

            [Display(Name = "Đường link")]
            public string duonglink { get; set; }

            [Display(Name = "Thứ tự")]
            public Nullable<int> thutu { get; set; }
        }
    }
}