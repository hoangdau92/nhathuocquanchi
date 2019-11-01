using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_Thuoc_Metadata))]
    public partial class QC_Thuoc
    {
        public class QC_Thuoc_Metadata
        {

            [Display(Name = "Tên")]
            [Required(ErrorMessage = "Vui lòng nhập tên")]
            public string ten { get; set; }

            [Display(Name = "Hình đại diện")]
            public string anhdaidien { get; set; }

            [Display(Name = "Loại thuốc")]
            [Required(ErrorMessage = "Chọn loại thuốc")]
            public string idloaithuoc { get; set; }

            [Display(Name = "Thời hạn sử dụng")]
            public string hansudung { get; set; }

            [Display(Name = "Giá tiền (VNĐ)")]
            [Required(ErrorMessage = "Nhập giá tiền")]
            [RegularExpression("([0-9]+)", ErrorMessage = "Chỉ nhập số")]
            public string gia { get; set; }

            [Display(Name = "Đơn vị")]
            [Required(ErrorMessage = "Chọn đơn vị")]
            public string iddonvi { get; set; }

            [Display(Name = "Còn hàng")]
            public string tinhtrang { get; set; }

            [Display(Name = "Mô tả")]
            public string mota { get; set; }

            [Display(Name = "Biến động giá")]
           // [Required(ErrorMessage = "Chọn loại biến động giá")]
            public string idbiendonggia { get; set; }

            [Display(Name = "Độc quyền")]
            public string docquyen { get; set; }

            [Display(Name = "Ưu đãi")]
            public string uudai { get; set; }

            [Display(Name = "Sản phẩm mới")]
            public string sanphammoi { get; set; }

        }
    }
}