using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_YKienKhachHang_Metadata))]
    public partial class QC_YKienKhachHang
    {
        public class QC_YKienKhachHang_Metadata
        {

            [Display(Name = "Tên khách hàng")]
            [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
            public string tenkhachhang { get; set; }

            [Display(Name = "Nội dung")]
            public string noidung { get; set; }

            [Display(Name = "Hiển thị")]
            public bool hienthi { get; set; }

            [Display(Name = "Thứ tự")]
            public int thutu { get; set; }

            [Display(Name = "Ảnh đại diện")]

            [Required(ErrorMessage = "Vui lòng upload hình ảnh")]
            public string anhdaidien { get; set; }

            [Display(Name = "Thông tin thêm")]
            public string thongtinthem { get; set; }
        }
    }
}