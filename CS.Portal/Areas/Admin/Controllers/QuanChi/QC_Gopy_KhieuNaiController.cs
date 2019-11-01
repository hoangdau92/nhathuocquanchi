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

namespace Core_MVC.Areas.Admin.Controllers
{

    public class QC_Gopy_KhieuNaiController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_Gopy_KhieuNai/

        [CheckPermission]
        public ActionResult Index(int? page,int? id_khachhang)
        {
            id_khachhang = id_khachhang ?? 0;
            var data = ctx.QC_Gopy_KhieuNai.Where(x=>x.idkhachhang == id_khachhang || id_khachhang==0).ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var khang_hang = ctx.QC_KhachHang.Where(x => x.kichhoat == true).ToList();
            TempData["khachhang"] = khang_hang;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [CheckPermission]
        public ActionResult PhanHoiYKien(int id)
        {
            try
            {
                var obj = ctx.QC_Gopy_KhieuNai.Find(id);
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
        public ActionResult PhanHoiYKien(QC_Gopy_KhieuNai obj, string noidungphanhoi)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = ctx.QC_Gopy_KhieuNai.Where(x => x.id == obj.id).FirstOrDefault(); 
                    data.ngayphanhoi = DateTime.Now;
                    data.noidungphanhoi = noidungphanhoi;
                    ctx.Entry(data).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Phản hồi thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_Gopy_KhieuNai");
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
        public JsonResult Delete(int id)
        {
            try
            {
                var obj = ctx.QC_Gopy_KhieuNai.Find(id);
                if (obj != null)
                {
                    ctx.QC_Gopy_KhieuNai.Remove(obj);
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
