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

namespace Core_MVC.Areas.Admin.Controllers.QuanChi
{
    public class QC_LoaiThuocController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_LoaiThuoc/
        [CheckPermission]
        public ActionResult Index(string keyword, int? page)
        {
            try
            {
                keyword = keyword != null ? keyword.Trim() : "";
                var data = ctx.QC_LoaiThuoc.Where(x => x.ten.Contains(keyword)).ToList();
                ViewBag.SearchString = keyword;
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
        public ActionResult Create(FormCollection fc, QC_LoaiThuoc obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var o = ctx.QC_LoaiThuoc.Where(x => x.ten.ToLower() == obj.ten.ToLower()).FirstOrDefault();
                    if (o != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tại loại thuốc này !");
                        return View();
                    }
                    ctx.QC_LoaiThuoc.Add(obj);
                    ctx.SaveChanges();
                    if (obj.id > 0)
                    {
                        SetAlert("Thêm mới thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_LoaiThuoc");
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
                var obj = ctx.QC_LoaiThuoc.Find(id);
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
        public ActionResult Edit(FormCollection fc, QC_LoaiThuoc obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = ctx.QC_LoaiThuoc.Where(x => x.id == obj.id && x.ten.ToLower() == obj.ten.ToLower()).FirstOrDefault();
                    if (check != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tại loại sp này !");
                        return View();
                    }
                    ctx.Entry(obj).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_LoaiThuoc");
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
                var obj = ctx.QC_LoaiThuoc.Find(id);
                if (obj != null)
                {
                    ctx.QC_LoaiThuoc.Remove(obj);
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

    }
}
