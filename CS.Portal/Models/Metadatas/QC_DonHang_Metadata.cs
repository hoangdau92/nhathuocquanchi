using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_DonHang_Metadata))]
    public partial class QC_DonHang
    {
        public class QC_DonHang_Metadata 
        {

            [Display(Name = "Mã đơn hàng")]
            public string madonhang { get; set; }

            [Display(Name = "Ghi chú")]
            public string motaxuly { get; set; }

            [Display(Name = "Trạng thái")]
            public string trangthai { get; set; }
        }
    }
}