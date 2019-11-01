using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_KhachHang_Metadata))]
    public partial class QC_KhachHang
    {
        public class QC_KhachHang_Metadata
        {

            [Display(Name = "Họ và tên")]
            [Required(ErrorMessage = "Vui lòng nhập tên")]
            public string tendaydu { get; set; }

            [Display(Name = "Tên đăng nhập")]
            [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
            public string tendangnhap { get; set; }

            [Display(Name = "Mật khẩu")]
            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [StringLength(100, ErrorMessage = "Mật khẩu phải có ý nhất 6 ký tự", MinimumLength = 6)]
            public string matkhau { get; set; }

    
            [RegularExpression("([0-9]+)", ErrorMessage = "Chỉ nhập số")]
            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            [Display(Name = "Số điện thoại")]
            public string dienthoai { get; set; }

            [Display(Name = "Địa chỉ")]
            public string diachi { get; set; }

            [Display(Name = "Ảnh đại diện")]
            //[Required(ErrorMessage = "Vui lòng chọn ảnh đại diện")]
            public string anhdaidien { get; set; }

            [Display(Name = "Ngày đăng ký")]
            public DateTime ngaydangky { get; set; }

            [Display(Name = "Điểm tích lũy")]
            public int diemtichluy { get; set; }

            [Display(Name = "Kích hoạt")]
            public bool kichhoat { get; set; }

            [Display(Name = "Mã kích hoạt")]
            public string makichhoat { get; set; }

            [Display(Name = "Email")]
            [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email chưa đúng")]
            [Required(ErrorMessage = "Vui lòng nhập email để kích hoạt tài khoản")]
            public string email { get; set; }

        }
    }
}