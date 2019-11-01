using Core_MVC.Models;
using CS.Portal.App_Start;
using CS.Portal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using System.Configuration;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QC_KhachHangController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_KhachHang/

        [CheckPermission]
        public ActionResult Index(string keyword, int? page)
        {
            try
            {
                keyword = keyword != null ? keyword.Trim() : "";
                var data = ctx.QC_KhachHang.Where(x => x.tendaydu.Contains(keyword) || x.tendangnhap.Contains(keyword) && x.kichhoat == true).ToList();
                ViewBag.SearchString = keyword;

                string url = ConfigurationManager.AppSettings["UrlAvatar"].ToString();
                string SiteUrl = ConfigurationManager.AppSettings["SiteUrl"].ToString();
                ViewBag.DUONGDANANH = SiteUrl + url;

                var anhdaidien = (from k in ctx.QC_KhachHang
                                  select k.anhdaidien).FirstOrDefault();
                ViewBag.ANHDAIDIEN = anhdaidien;

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(data.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), "error");
                Logs.WriteLog(ex);
                return View();
            }
        }

        [CheckPermission]
        public ActionResult Create()
        {
            try
            {
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
        public ActionResult Create(FormCollection fc, QC_KhachHang obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var o = ctx.QC_KhachHang.Where(x => x.tendangnhap.ToLower() == obj.tendangnhap.ToLower()).FirstOrDefault();
                    if (o != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tại tên đăng nhập này !");
                        return View();
                    }
                    ctx.QC_KhachHang.Add(obj);
                    ctx.SaveChanges();
                    if (obj.id > 0)
                    {
                        SetAlert("Thêm mới thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_KhachHang");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm mới không thành công");
                    }
                    return View("Index");
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
                var obj = ctx.QC_KhachHang.Find(id);
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
        public ActionResult Edit(FormCollection fc, QC_KhachHang obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = ctx.QC_KhachHang.Where(x => x.id == obj.id && x.tendangnhap.ToLower() == obj.tendangnhap.ToLower()).FirstOrDefault();
                    if (check != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tại tên đăng nhập này !");
                        return View();
                    }
                    ctx.Entry(obj).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_KhachHang");
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
                var obj = ctx.QC_KhachHang.Find(id);
                var khachhang_donhang = ctx.QC_DonHang.Where(x => x.idkhachhang == id).FirstOrDefault();
                var khachhang_gopy = ctx.QC_Gopy_KhieuNai.Where(x => x.idkhachhang == id).FirstOrDefault();

                if (khachhang_donhang != null || khachhang_gopy != null)
                {
                    SetAlert("Không thể xóa người dùng đã có đơn hàng hoặc đã có góp ý, khiếu nại", AlertType.Error);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
              
                
                var khachhang_nhoms = ctx.QC_KhachHang_Nhom.Where(x => x.idkhachhang == id).ToList();
                if (khachhang_nhoms != null)
                {
                    if (obj != null)
                    {
                        ctx.QC_KhachHang.Remove(obj);

                        foreach (var khachhang_nhom in khachhang_nhoms)
                        {
                            ctx.QC_KhachHang_Nhom.Remove(khachhang_nhom);
                        }
                        ctx.SaveChanges();

                        SetAlert("Xóa thành công", AlertType.Success);
                        return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, message = "Lỗi xóa bản ghi." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { status = false, message = "Lỗi xóa bản ghi." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
