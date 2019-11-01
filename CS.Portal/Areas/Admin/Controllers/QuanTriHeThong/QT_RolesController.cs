using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CS.Portal.Common;
using CS.Portal.Core.EF;
using CS.Portal.Core.DAO;
using CS.Portal.App_Start;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QT_RolesController : BaseController
    {
        [CheckPermission]
        public ActionResult Index(string searchString)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                //var roles = objRolesDAO.GetAll();
                var roles = objRolesDAO.Search(searchString);
                ViewBag.SearchString = searchString;
                return View(roles);
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        [CheckPermission]
        public ActionResult Create()
        {
            return View();
        }

        [CheckPermission]
        [HttpPost]
        public ActionResult Create(CSF_Roles role)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                    int ReturnRoleID = objRolesDAO.Insert(role);
                    if (ReturnRoleID > 0)
                    {
                        SetAlert("Thêm nhóm người dùng thành công", AlertType.Success);
                        return RedirectToAction("Index", "QT_Roles");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm nhóm người dùng không thành công");
                    }
                }
                return View("Index");
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
                CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                if (objRolesDAO.Delete(id))
                {
                    SetAlert("Xóa nhóm người dùng thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Nhóm này đang được sử dụng, không được phép xóa!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission]
        public ActionResult Edit(int id)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                var role = objRolesDAO.GetRoleByID(id);
                return View(role);
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
        public ActionResult Edit(CSF_Roles role)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                    if (objRolesDAO.Update(role))
                    {
                        SetAlert("Cập nhật nhóm người dùng thành công", AlertType.Success);
                        return RedirectToAction("Index", "QT_Roles");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật nhóm người dùng không thành công");
                    }
                }
                return View("Index");
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

    }
}
