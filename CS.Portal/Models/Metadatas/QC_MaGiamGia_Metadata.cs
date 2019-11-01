using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_MaGiamGia_Metadata))]
    public partial class QC_MaGiamGia
    {
        public class QC_MaGiamGia_Metadata
        {

            [Display(Name = "Mã giảm giá")]
            [Required(ErrorMessage = "Vui lòng nhập tên mã")]
            public string tenma { get; set; }


            [Display(Name = "Từ ngày")]
            [Required(ErrorMessage = "Vui lòng nhập thời gian bắt đầu")]
            [DataType(DataType.DateTime)]
            public DateTimeOffset tungay { get; set; }


            [Display(Name = "Đến ngày")]
            [Required(ErrorMessage = "Vui lòng nhập thời gian kết thúc")]
            [DataType(DataType.DateTime)]
            public DateTimeOffset denngay { get; set; }


            [Display(Name = "Loại giảm giá")]
            public string idloaigiamgia { get; set; }


            [Display(Name = "Đối tượng giảm giá")]
            [Required(ErrorMessage = "Chọn đối tượng được giảm giá")]
            public string idkieudoituong { get; set; }


            [Display(Name = "Kích hoạt")]
            public Boolean kichhoat { get; set; }


            [Display(Name = "Giá trị giảm")]
            [Required(ErrorMessage = "Nhập giá trị giảm giá")]
            [RegularExpression("([0-9]+)", ErrorMessage = "Chỉ nhập số")]
            public string giatri { get; set; }
        }
    }
}