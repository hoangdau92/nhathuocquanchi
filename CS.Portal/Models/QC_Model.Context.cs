﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class quanchiEntities : DbContext
    {
        public quanchiEntities()
            : base("name=quanchiEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<QC_BienDongGia> QC_BienDongGia { get; set; }
        public virtual DbSet<QC_DoiTacTieuBieu> QC_DoiTacTieuBieu { get; set; }
        public virtual DbSet<QC_DoiTuong_MaGiamGia> QC_DoiTuong_MaGiamGia { get; set; }
        public virtual DbSet<QC_DonViThuoc> QC_DonViThuoc { get; set; }
        public virtual DbSet<QC_Gopy_KhieuNai> QC_Gopy_KhieuNai { get; set; }
        public virtual DbSet<QC_KhachHang> QC_KhachHang { get; set; }
        public virtual DbSet<QC_LoaiThuoc> QC_LoaiThuoc { get; set; }
        public virtual DbSet<QC_NhomKhachHang> QC_NhomKhachHang { get; set; }
        public virtual DbSet<QC_PhuongThucThanhToan> QC_PhuongThucThanhToan { get; set; }
        public virtual DbSet<QC_Slider> QC_Slider { get; set; }
        public virtual DbSet<QC_Thuoc> QC_Thuoc { get; set; }
        public virtual DbSet<QC_Thuoc_DonHang> QC_Thuoc_DonHang { get; set; }
        public virtual DbSet<CSF_ThietLapWebsite> CSF_ThietLapWebsite { get; set; }
        public virtual DbSet<QC_KhachHang_Nhom> QC_KhachHang_Nhom { get; set; }
        public virtual DbSet<QC_YKienKhachHang> QC_YKienKhachHang { get; set; }
        public virtual DbSet<QC_KieuDoiTuong> QC_KieuDoiTuong { get; set; }
        public virtual DbSet<QC_LoaiGiamGia> QC_LoaiGiamGia { get; set; }
        public virtual DbSet<QC_MaGiamGia> QC_MaGiamGia { get; set; }
        public virtual DbSet<QC_DonHang> QC_DonHang { get; set; }
    }
}
