using Core_MVC.Common;
using Core_MVC.Models;
using CS.Portal.App_Start;
using CS.Portal.Common;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core_MVC.Areas.Admin.Controllers.QuanChi
{
    public class QC_MaGiamGiaController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_MaGiamGia/
        [CheckPermission]
        public ActionResult Index(string key, int? loaigiamgia, int? page)
        {
            TempData["loaigiamgia"] = ctx.QC_LoaiGiamGia.ToList();
            TempData.Keep("loaigiamgia");
            key = key != null ? key.Trim() : "";
            loaigiamgia = loaigiamgia ?? 0;
            ViewBag.LOAIGIAMGIA = loaigiamgia;
            var data = ctx.QC_MaGiamGia.Where(x => x.tenma.Contains(key) && (x.idloaigiamgia == loaigiamgia || loaigiamgia == 0)).OrderBy(x => x.tungay).ToList();
            ViewBag.SearchString = key;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [CheckPermission]
        public ActionResult Create()
        {
            try
            {
                TempData["loaigiamgia"] = ctx.QC_LoaiGiamGia.ToList();
                TempData.Keep("loaigiamgia");
                TempData["kieudoituong"] = ctx.QC_KieuDoiTuong.ToList();
                TempData.Keep("kieudoituong");
                return View();
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        [CheckPermission]
        [HttpPost]
        public ActionResult Create(FormCollection fc, QC_MaGiamGia obj, int[] doituong)
        {
            try
            {
                TempData.Keep("loaigiamgia");
                TempData.Keep("kieudoituong");
                if (ModelState.IsValid && doituong.Count() > 0)
                {
                    var mgg = ctx.QC_MaGiamGia.Where(x => x.tenma.Trim().ToLower() == obj.tenma.ToLower().Trim()).FirstOrDefault();
                    if (mgg != null)
                    {
                        ModelState.AddModelError("", "Mã giảm giá đã tồn tại.");
                        return View(obj);
                    }
                    if (obj.idloaigiamgia == LOAI_GIAMGIA.PHANTRAM && (obj.giatri < 0 || obj.giatri > 100))
                    {
                        ModelState.AddModelError("", "Giá trị phần trăm lỗi.");
                        return View(obj);
                    }
                    obj.solansudung = -1;
                    ctx.QC_MaGiamGia.Add(obj);
                    ctx.SaveChanges();
                    if (obj.id > 0)
                    {
                        QC_DoiTuong_MaGiamGia dt_mgg = new QC_DoiTuong_MaGiamGia();
                        if (doituong.Contains(-1))
                        {
                            dt_mgg.iddoituong = -1;
                            dt_mgg.idmagiamgia = obj.id;
                            ctx.QC_DoiTuong_MaGiamGia.Add(dt_mgg);
                        }
                        else
                        {
                            foreach (var item in doituong)
                            {
                                dt_mgg = new QC_DoiTuong_MaGiamGia();
                                dt_mgg.iddoituong = item;
                                dt_mgg.idmagiamgia = obj.id;
                                ctx.QC_DoiTuong_MaGiamGia.Add(dt_mgg);
                            }
                        }
                        ctx.SaveChanges();
                        SetAlert("Tạo mã giảm giá thành công", AlertType.Success);
                    }
                    return RedirectToAction("Index", "QC_MaGiamGia");
                }
                return View();
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        [CheckPermission]
        public ActionResult Edit(int id)
        {
            try
            {
                TempData["loaigiamgia"] = ctx.QC_LoaiGiamGia.ToList();
                TempData.Keep("loaigiamgia");
                TempData["kieudoituong"] = ctx.QC_KieuDoiTuong.ToList();
                TempData.Keep("kieudoituong");
                var obj = ctx.QC_MaGiamGia.Find(id);
                List<int> iddoituongs = ctx.QC_DoiTuong_MaGiamGia.Where(x => x.idmagiamgia == id).Select(x => (int)x.iddoituong).ToList();
                ViewBag.DOITUONG = string.Join(",", iddoituongs.ToArray());
                if (obj != null)
                {
                    return View(obj);
                }
                return View();
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }


        [CheckPermission]
        [HttpPost]
        public ActionResult Edit(FormCollection fc, QC_MaGiamGia obj)
        {
            try
            {
                TempData.Keep("loaigiamgia");
                TempData.Keep("kieudoituong");
                var e = ctx.QC_MaGiamGia.Find(obj.id);
                if (e != null)
                {
                    e.tungay = obj.tungay;
                    e.denngay = obj.denngay;
                    e.kichhoat = obj.kichhoat == null ? false : obj.kichhoat;
                    ctx.Entry(e).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_MaGiamGia");
                    }
                    else
                    {
                        SetAlert("Cập nhật không thành công", AlertType.Error);
                    }
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        [CheckPermission]
        public JsonResult Delete(int id)
        {
            try
            {
                var obj = ctx.QC_MaGiamGia.Find(id);
                if (obj != null)
                {
                    var dt_mgg = ctx.QC_DoiTuong_MaGiamGia.Where(x => x.idmagiamgia == id).ToList();
                    foreach (var item in dt_mgg)
                    {
                        ctx.QC_DoiTuong_MaGiamGia.Remove(item);
                    }
                    ctx.SaveChanges();
                    ctx.QC_MaGiamGia.Remove(obj);
                    ctx.SaveChanges();
                    SetAlert("Xóa thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi xóa bản ghi." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LayDoiTuong(int kieudoituong)
        {
            try
            {
                if (kieudoituong == KIEU_DOITUONG.KHACHHANG)
                {
                    var result = ctx.QC_KhachHang.Where(x => x.kichhoat == true).Select(x => new { x.id, tenhienthi = x.tendangnhap }).ToList();
                    return Json(new { status = true, result = result }, JsonRequestBehavior.AllowGet);
                }
                else if (kieudoituong == KIEU_DOITUONG.NHOMKHACHHANG)
                {
                    var result = ctx.QC_NhomKhachHang.OrderBy(x => x.thutu).Select(x => new { x.id, tenhienthi = x.ten }).ToList();
                    return Json(new { status = true, result = result }, JsonRequestBehavior.AllowGet);
                }
                else if (kieudoituong == KIEU_DOITUONG.SANPHAM)
                {
                    var result = ctx.QC_Thuoc.Select(x => new { x.id, tenhienthi = x.ten }).ToList();
                    return Json(new { status = true, result = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi dữ liệu" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
