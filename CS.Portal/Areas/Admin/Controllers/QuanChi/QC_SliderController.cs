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
    public class QC_SliderController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_Silder/

        [CheckPermission]
        public ActionResult Index(string keyword, int? page)
        {
            try
            {
                keyword = keyword != null ? keyword.Trim() : "";
                var data = ctx.QC_Slider.ToList();
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
        public ActionResult Create(FormCollection fc, QC_Slider obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var o = ctx.QC_Slider.Where(x => x.duongdananh.ToLower() == obj.duongdananh.ToLower()).FirstOrDefault();
                    if (o != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tên slide này !");
                        return View();
                    }
                    ctx.QC_Slider.Add(obj);
                    ctx.SaveChanges();
                    if (obj.id > 0)
                    {
                        SetAlert("Thêm mới thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_Slider");
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
                var obj = ctx.QC_Slider.Find(id);
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
        public ActionResult Edit(FormCollection fc, QC_Slider obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = ctx.QC_Slider.Where(x => x.id == obj.id && x.duongdananh.ToLower() == obj.duongdananh.ToLower()).FirstOrDefault();
                    if (check != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tại slide này !");
                        return View();
                    }
                    ctx.Entry(obj).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_Slider");
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
                var obj = ctx.QC_Slider.Find(id);
                if (obj != null)
                {
                    ctx.QC_Slider.Remove(obj);
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
