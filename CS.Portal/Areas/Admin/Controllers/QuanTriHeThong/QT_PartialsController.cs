using CS.Portal.Common;
using CS.Portal.Core.DAO;
using CS.Portal.Core.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using PagedList;
using CS.Portal.App_Start;
namespace Core_MVC.Areas.Admin.Controllers.QuanTriHeThong
{
    public class QT_PartialsController : BaseController
    {
        //
        // GET: /Admin/QT_Partials/
        [CheckPermission]
        public ActionResult Index(string searchString, int? page)
        {
            try
            {
                CSF_Partials_DAO objDAO = new CSF_Partials_DAO();
                var data = objDAO.Search(searchString);
                ViewBag.SearchString = searchString;
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
        public ActionResult Edit(int id)
        {
            try
            {
                CSF_Partials_DAO objDAO = new CSF_Partials_DAO();
                var Partial = objDAO.GetPartialsByID(id);
                SetTempData2SelectList((int)Partial.ModuleID, Partial.Controller);
                return View(Partial);
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }
        protected void SetTempData2SelectList(int moduleid, string Controller)
        {
            CSF_MVCEntities MyContext = new CSF_MVCEntities();
            var listModules = MyContext.CSF_Modules;
            TempData["module"] = new SelectList(listModules, "ID", "Name", moduleid);
            TempData.Keep("module");
            var selectListItems = GetControllerNames().Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
            TempData["ddlController"] = new SelectList(selectListItems, "Value", "Text", Controller);
            TempData.Keep("ddlController");
        }
        [CheckPermission]
        public ActionResult Create()
        {
            try
            {

                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                TempData["module"] = MyContext.CSF_Modules.ToList();
                TempData.Keep("module");
                TempData["listController"] = GetControllerNames().ToList();
                TempData.Keep("listController");
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
        public ActionResult Create(FormCollection fc, CSF_Partials obj)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    CSF_Partials_DAO objDAO = new CSF_Partials_DAO();
                    if (objDAO.CheckExist_PartialsKey(obj))
                    {
                        SetAlert("Key đã tồn tại!", AlertType.Error);
                        return View();
                    }
                    int ReturnID = objDAO.Insert(obj);
                    //CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                    //int intUserID = objUserDao.GetUserIDByUserName(HttpContext.User.Identity.Name);
                    //obj.UserCreate = intUserID;
                    //int ReturnID = objDAO.Insert(obj);
                    if (ReturnID > 0)
                    {
                        SetAlert("Thêm page thành công", AlertType.Success);
                        return RedirectToAction("Index", "QT_Partials");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm page không thành công");
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
        #region GetControllerName
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        private List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(type => controllerNames.Add(type.Name));
            List<string> listResult = new List<string>();
            if (controllerNames.Count > 0)
            {
                for (int i = 0; i < controllerNames.Count; i++)
                {
                    listResult.Add(controllerNames[i].Replace("Controller", ""));
                }
            }
            return listResult;
        }
        #endregion
        [CheckPermission]
        public JsonResult Delete(int id)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Partials_DAO objDAO = new CSF_Partials_DAO();
                if (objDAO.Delete(id))
                {
                    SetAlert("Xóa điều khiển thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi xóa điều khiển" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [CheckPermission]
        public ActionResult Edit(FormCollection fc, CSF_Partials obj)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_Partials_DAO objDAO = new CSF_Partials_DAO();
                    if (objDAO.CheckExist_PartialsKey(obj))
                    {
                        SetAlert("Key đã tồn tại!", AlertType.Error);
                    }
                    else
                    {
                        if (objDAO.Update(obj))
                        {
                            SetAlert("Cập nhật điều khiển thành công", AlertType.Success);
                            return RedirectToAction("Index", "QT_Partials");
                        }
                        else
                        {
                            SetAlert("Cập nhật điều không thành công", AlertType.Error);
                        }
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
    }
}
