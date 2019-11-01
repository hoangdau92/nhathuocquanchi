using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core_MVC.Models
{
    [MetadataTypeAttribute(typeof(QC_Gopy_KhieuNai_Metadata))]
    public partial class QC_Gopy_KhieuNai
    {
        public class QC_Gopy_KhieuNai_Metadata
        {
            public int idkhachhang { get; set; }
            public string noidunggopy { get; set; }
            public DateTime ngaygopy { get; set; }
            public int noidungkhieunai { get; set; }
            public string noidungphanhoi { get; set; }
            public DateTime ngayphanhoi { get; set; }
        }
    }
}