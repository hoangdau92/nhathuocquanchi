//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Core_MVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QC_DoiTuong_MaGiamGia
    {
        public int id { get; set; }
        public Nullable<int> idmagiamgia { get; set; }
        public Nullable<int> iddoituong { get; set; }
    
        public virtual QC_MaGiamGia QC_MaGiamGia { get; set; }
    }
}
