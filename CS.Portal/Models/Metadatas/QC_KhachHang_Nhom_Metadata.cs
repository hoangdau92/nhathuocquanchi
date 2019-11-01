using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_KhachHang_Nhom_Metadata))]
    public partial class QC_KhachHang_Nhom
    {
        public class QC_KhachHang_Nhom_Metadata
        {
            [Key]
            public int id { get; set; }
            public int idnhom { get; set; }
            public int idkhachhang { get; set; }
        }
    }
}