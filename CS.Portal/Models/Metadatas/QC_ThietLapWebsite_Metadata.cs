using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(CSF_ThietLapWebsite_Metadata))]
    public partial class CSF_ThietLapWebsite
    {
        public class CSF_ThietLapWebsite_Metadata
        {

            [Display(Name = "Tên website")]
            [Required(ErrorMessage = "Vui lòng nhập tên")]
            public string tenwebsite { get; set; }

            [Display(Name = "Logo")]
            public string logo { get; set; }

            [Display(Name = "Slogan")]
            public string slogan { get; set; }

            [Display(Name = "Hotline")]
            public string hotline { get; set; }

            [Display(Name = "Hotline hỗ trợ")]
            public string hotline_hotro { get; set; }

            [Display(Name = "Hotline dịch vụ")]
            public string hotline_dichvu { get; set; }

            [Display(Name = "Địa chỉ")]
            public string diachi { get; set; }

            [Display(Name = "Email")]
            [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email chưa đúng")]
            public string email { get; set; }
    
            [Display(Name = "Tên công ty")]
            public string tencongty { get; set; }

            [Display(Name = "Facebook")]
            public string facebook { get; set; }

            [Display(Name = "Thông tin bổ sung")]
            public string thongtinbosung { get; set; }

            [Display(Name = "Hiển thị")]
            public string hienthi_logo { get; set; }

        }
    }
}