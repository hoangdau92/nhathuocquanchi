using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_DoiTacTieuBieu_Metadata))]
    public partial class QC_DoiTacTieuBieu
    {
        public class QC_DoiTacTieuBieu_Metadata
        {

            [Display(Name = "Tên")]
            [Required(ErrorMessage = "Vui lòng nhập tên đối tác")]
            public string ten { get; set; }

            [Display(Name = "Mô tả")]
            public string mota { get; set; }

            [Display(Name = "Ảnh đại diện")]
            [Required(ErrorMessage = "Vui lòng nhập ảnh đại diện")]
            public string anhdaidien { get; set; }

            [Display(Name = "Thứ tự")]
            public Nullable<int> thutu { get; set; }

        }
    }
}