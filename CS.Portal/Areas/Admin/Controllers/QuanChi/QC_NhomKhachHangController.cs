using Core_MVC.Models;
using CS.Portal.App_Start;
using CS.Portal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Web.Mvc;
using System.Data.Entity;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QC_NhomKhachHangController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_NhomKhachHang/
        [CheckPermission]
        public ActionResult Index(string keyword, int? page)
        {
            try
            {
                keyword = keyword != null ? keyword.Trim() : "";
                var data = ctx.QC_NhomKhachHang.Where(x => x.ten.Contains(keyword)).OrderBy(x=>x.thutu).ToList();
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
        public ActionResult Create(FormCollection fc, QC_NhomKhachHang obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var o = ctx.QC_NhomKhachHang.Where(x => x.ten.ToLower() == obj.ten.ToLower()).FirstOrDefault();
                    if (o != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tại nhóm khách hàng này !");
                        return View();
                    }
                    ctx.QC_NhomKhachHang.Add(obj);
                    ctx.SaveChanges();
                    if (obj.id > 0)
                    {
                        SetAlert("Thêm mới thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_NhomKhachHang");
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
                var obj = ctx.QC_NhomKhachHang.Find(id);
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
        public ActionResult Edit(FormCollection fc, QC_NhomKhachHang obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ctx.Entry(obj).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_NhomKhachHang");
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
                var obj = ctx.QC_NhomKhachHang.Find(id);
                if (obj != null)
                {
                    ctx.QC_NhomKhachHang.Remove(obj);
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
