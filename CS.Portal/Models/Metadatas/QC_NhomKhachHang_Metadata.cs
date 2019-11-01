using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_NhomKhachHang_Metadata))]
    public partial class QC_NhomKhachHang
    {
        public class QC_NhomKhachHang_Metadata
        {

            [Display(Name = "Nhóm người dùng")]
            [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
            public string ten { get; set; }

            [Display(Name = "Mô tả")]
            public string mota { get; set; }
        }
    }
}