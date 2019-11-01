using CS.Portal.Common;
using CS.Portal.Core.DAO;
using CS.Portal.Core.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using CS.Portal.App_Start;

namespace Core_MVC.Areas.Admin.Controllers.QuanTriHeThong
{
    public class QT_ModulesController : BaseController
    {
        //
        // GET: /Admin/QT_Modules/


        [CheckPermission]
        public ActionResult Index(string keyword, int? page)
        {
            try
            {
                keyword = keyword != null ? keyword : "";
                CSF_Modules_DAO objDAO = new CSF_Modules_DAO();
                var data = objDAO.Search1(keyword);
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
        public ActionResult Create(FormCollection fc, CSF_Modules obj)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {

                    CSF_Modules_DAO objDAO = new CSF_Modules_DAO();
                    int ReturnID = objDAO.Insert(obj);
                    if (ReturnID > 0)
                    {
                        SetAlert("Thêm mới module thành công", AlertType.Success);
                        return RedirectToAction("Index", "QT_Modules");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm mới module không thành công");
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
                CSF_MVCEntities entity = new CSF_MVCEntities();
                var doc = entity.CSF_Modules.Find(id);
                if (doc != null)
                {
                    return View(doc);
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
        public ActionResult Edit(FormCollection fc, CSF_Modules obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CSF_Modules_DAO objDAO = new CSF_Modules_DAO();
                    CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                    if (objDAO.EditModules(obj))
                    {
                        SetAlert("Cập nhật module thành công", AlertType.Success);
                        return RedirectToAction("Index", "QT_Modules");
                    }
                    else
                    {
                        SetAlert("Cập nhật module không thành công", AlertType.Error);
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
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Modules_DAO objDAO = new CSF_Modules_DAO();
                if (objDAO.CheckDelete(id))
                {
                    if (objDAO.Delete(id))
                    {
                        SetAlert("Xóa module thành công", AlertType.Success);
                        return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, message = "Lỗi xóa bản ghi" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Module đang sử dụng không được phép xóa." }, JsonRequestBehavior.AllowGet);
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
