using Core_MVC.Models;
using CS.Portal.App_Start;
using CS.Portal.Common;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QC_ThuocController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_LoaiThuoc/
        [CheckPermission]
        public ActionResult Index(string key, int? loaithuoc, int? loaidonvithuoc, int? loaibiendonggia, int? page)
        {
            try
            {
                ViewBag.URLIMAGE = System.Configuration.ConfigurationManager.AppSettings["UrlImage"];
                TempData["loaithuoc"] = ctx.QC_LoaiThuoc.ToList();
                TempData.Keep("loaithuoc");
                key = key != null ? key.Trim() : "";
                loaithuoc = (loaithuoc ?? -1);
                ViewBag.LOAITHUOC = loaithuoc;
                var data = ctx.QC_Thuoc.Where(x => x.ten.Contains(key) && (x.idloaithuoc == loaithuoc || loaithuoc == -1)).OrderBy(x => x.ten).ToList();
                ViewBag.SearchString = key;
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
                TempData["loaithuoc"] = ctx.QC_LoaiThuoc.OrderBy(x => x.ten).ToList();
                TempData.Keep("loaithuoc");
                TempData["loaidonvitinh"] = ctx.QC_DonViThuoc.OrderBy(x => x.ten).ToList();
                TempData.Keep("loaidonvitinh");
                TempData["loaibiendonggia"] = ctx.QC_BienDongGia.OrderBy(x => x.kyhieu).ToList();
                TempData.Keep("loaibiendonggia");
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
        public ActionResult Create(FormCollection fc, QC_Thuoc obj)
        {
            try
            {
                TempData.Keep("loaithuoc");
                TempData.Keep("loaidonvitinh");
                TempData.Keep("loaibiendonggia");
                if (ModelState.IsValid)
                {
                    var o = ctx.QC_Thuoc.Where(x => x.ten.ToLower() == obj.ten.ToLower()).FirstOrDefault();
                    if (o != null)
                    {
                        ModelState.AddModelError("", "Đã tồn tên thuốc này !");
                        return View();
                    }
                    obj.docquyen = obj.docquyen == null ? false : true;
                    obj.uudai = obj.uudai == null ? false : true;
                    obj.sanphammoi = obj.sanphammoi == null ? false : true;
                    if (obj.anhdaidien == null || obj.anhdaidien == "")
                    {
                        obj.anhdaidien = "/Images/webimg/noimage.png";
                    }
                    ctx.QC_Thuoc.Add(obj);
                    ctx.SaveChanges();
                    if (obj.id > 0)
                    {
                        SetAlert("Thêm mới thuốc thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_Thuoc");
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
                ViewBag.URLIMAGE = System.Configuration.ConfigurationManager.AppSettings["UrlImage"];
                TempData["loaithuoc"] = ctx.QC_LoaiThuoc.OrderBy(x => x.ten).ToList();
                TempData.Keep("loaithuoc");
                TempData["loaidonvitinh"] = ctx.QC_DonViThuoc.OrderBy(x => x.ten).ToList();
                TempData.Keep("loaidonvitinh");
                TempData["loaibiendonggia"] = ctx.QC_BienDongGia.OrderBy(x => x.mota).ToList();
                TempData.Keep("loaibiendonggia");
                var obj = ctx.QC_Thuoc.Find(id);
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
        public ActionResult Edit(FormCollection fc, QC_Thuoc obj)
        {
            try
            {
                TempData.Keep("loaithuoc");
                TempData.Keep("loaidonvitinh");
                TempData.Keep("loaibiendonggia");
                if (ModelState.IsValid)
                {
                    var o = ctx.QC_Thuoc.Find(obj.id);
                    o.ten = obj.ten;
                    if (obj.anhdaidien == null || obj.anhdaidien == "")
                    {
                        obj.anhdaidien = "/Images/webimg/noimage.png";
                    }
                    o.anhdaidien = obj.anhdaidien;
                    o.idloaithuoc = obj.idloaithuoc;
                    o.hansudung = obj.hansudung;
                    o.gia = obj.gia;
                    o.iddonvi = obj.iddonvi;
                    o.tinhtrang = obj.tinhtrang;
                    o.mota = obj.mota;
                    o.idbiendonggia = obj.idbiendonggia;
                    o.docquyen = obj.docquyen == null ? false : true;
                    o.uudai = obj.uudai == null ? false : true;
                    o.sanphammoi = obj.sanphammoi == null ? false : true;
                    ctx.Entry(o).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        return RedirectToAction("Index", "QC_Thuoc");
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
                var obj = ctx.QC_Thuoc.Find(id);
                if (obj != null)
                {
                    //ctx.Entry(obj).State = EntityState.Modified;
                    ctx.QC_Thuoc.Remove(obj);
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
