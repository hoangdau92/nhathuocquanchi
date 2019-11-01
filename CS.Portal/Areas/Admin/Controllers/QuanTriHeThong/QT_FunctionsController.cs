using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CS.Portal.Common;
using CS.Portal.Core.EF;
using CS.Portal.Core.DAO;
using PagedList;
using System.Reflection;
using CS.Portal.App_Start;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QT_FunctionsController : BaseController
    {
        [CheckPermission]
        public ActionResult Index(string search, int? page)
        {
            try
            {
                CSF_Functions_DAO objFunctionsDAO = new CSF_Functions_DAO();
                List<CSF_Functions_LayTatCa_Result> functions = objFunctionsDAO.Search(search);
                ViewBag.search = search;
                int pageSize = 15;
                int pageNumber = (page ?? 1);
                return View(functions.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), "error");
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

        #region GetActionInController
        public JsonResult GetActionInController(string controllerName)
        {
            controllerName = controllerName + "Controller";
            List<string> listAction = new List<string>();
            var types = from a in AppDomain.CurrentDomain.GetAssemblies()
                        from t in a.GetTypes()
                        where typeof(IController).IsAssignableFrom(t) &&
                                string.Equals(controllerName, t.Name, StringComparison.OrdinalIgnoreCase)
                        select t;

            var controllerType = types.FirstOrDefault();

            if (controllerType != null)
            {
                listAction = new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().Select(x => x.ActionName).Distinct().ToList();
            }

            var jsonResults = new { data = listAction };
            return Json(jsonResults, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region DeQuy
        public void BuildSubTree(List<SubSelectBox> listTree, int ParentID, List<CSF_Functions> listData, string tag)
        {
            try
            {
                SubSelectBox sc;
                var listChild = listData.Where(x => x.ParentID == ParentID);
                if (listChild != null && listChild.Count() > 0)
                {
                    tag += "--- ";
                    foreach (var item in listChild)
                    {
                        sc = new SubSelectBox();
                        sc.id = item.ID;
                        sc.name = tag + item.Name;
                        listTree.Add(sc);
                        BuildSubTree(listTree, item.ID, listData, tag);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        [CheckPermission]
        public ActionResult Create()
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");

                TempData["listController"] = GetControllerNames();
                TempData.Keep("listController");
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                List<SubSelectBox> listTree = new List<SubSelectBox>();
                SubSelectBox sc;
                var listData = MyContext.CSF_Functions.ToList();
                var listData0 = listData.Where(x => x.ParentID == 0).OrderBy(x => x.Name).ToList();
                string tag = "";
                foreach (var item in listData0)
                {
                    sc = new SubSelectBox();
                    sc.id = item.ID;
                    sc.name = item.Name;
                    listTree.Add(sc);
                    BuildSubTree(listTree, item.ID, listData, tag);
                }
                TempData["functions"] = listTree.ToList();
                TempData.Keep("functions");

                CSF_Modules_DAO objModulesDAO = new CSF_Modules_DAO();
                var modules = objModulesDAO.GetAll();
                TempData["modules"] = modules.ToList();
                TempData.Keep("modules");

                //ParentFunctionDropDownList();
                ModulesDropDownList();

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
        public ActionResult Create(FormCollection fc, CSF_Functions func)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_Functions_DAO objFunctionsDAO = new CSF_Functions_DAO();
                    string controllerAction = fc["listController"] + "-" + fc["listAction"];
                    //Kiểm tra trùng tên ControllerAction
                    if (objFunctionsDAO.CheckControllerAction(0, controllerAction))
                    {
                        SetAlert("Controller-Action đã tồn tại!", AlertType.Error);
                        return View();
                    }

                    func.Controller_Action = controllerAction;
                    int ReturnID = objFunctionsDAO.Insert(func);
                    if (ReturnID > 0)
                    {
                        SetAlert("Thêm chức năng thành công", AlertType.Success);
                        return RedirectToAction("Index", "QT_Functions");
                    }
                    else
                    {
                        SetAlert("Thêm chức năng không thành công", AlertType.Error);
                    }
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


        #region Dropdownlist

        protected void ModulesDropDownList()
        {
            CSF_Modules_DAO objModulesDAO = new CSF_Modules_DAO();
            var listModules = objModulesDAO.GetAll();
            TempData["ddlModules"] = new SelectList(listModules, "ID", "Name");
            TempData.Keep("ddlModules");
        }

        protected void ControllerDropDownList(string selectedController = null)
        {
            try
            {
                var selectListItems = GetControllerNames().Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
                TempData["ddlController"] = new SelectList(selectListItems, "Value", "Text", selectedController);
                TempData.Keep("ddlController");
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                throw;
            };
        }

        protected void ParentFunctionDropDownList()
        {
            try
            {
                CSF_Functions_DAO objFunctionsDAO = new CSF_Functions_DAO();
                List<CSF_Functions> listFunction = objFunctionsDAO.GetAll().OrderBy(o => o.ParentID).ToList();
                TempData["ddlFunctions"] = new SelectList(BuildTreeFunctions(listFunction, (int)listFunction.FirstOrDefault().ParentID), "ID", "Name");
                TempData.Keep("ddlFunctions");
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                throw;
            };
        }

        List<CSF_Functions> listResult = new List<CSF_Functions>(); string t = "--";
        public List<CSF_Functions> BuildTreeFunctions(List<CSF_Functions> lst, int parentNode)
        {
            foreach (var item in lst.Where(i => i.ParentID.Equals(parentNode)))
            {
                var subItem = lst.Where(s => s.ParentID.Equals(item.ID)).Count();

                if (subItem > 0)
                {
                    if (item.ParentID != 0)
                    {
                        item.Name = t + item.Name;
                        t += "--";
                    }
                    listResult.Add(item);
                }
                else
                {
                    if (item.ParentID != 0)
                    {
                        item.Name = t + item.Name;
                    }
                    //t += "--";
                    listResult.Add(item);
                }
                if (subItem > 0)
                {
                    BuildTreeFunctions(lst, item.ID);
                }
            }
            return listResult;
        }

        protected void ActionDropDownList(string controller, string selectedAction = null)
        {
            try
            {
                string controllerName = controller + "Controller";
                List<string> listAction = new List<string>();
                var types = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where typeof(IController).IsAssignableFrom(t) &&
                                    string.Equals(controllerName, t.Name, StringComparison.OrdinalIgnoreCase)
                            select t;

                var controllerType = types.FirstOrDefault();

                if (controllerType != null)
                {
                    listAction = new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().Select(x => x.ActionName).Distinct().ToList();
                }
                var selectListItems = listAction.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
                TempData["ddlAction"] = new SelectList(selectListItems, "Value", "Text", selectedAction);
                TempData.Keep("ddlAction");
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                throw;
            };
        }

        private void keepData()
        {
            TempData.Keep("ddlController");
            TempData.Keep("ddlModules");
            TempData.Keep("ddlFunctions");
            TempData.Keep("ddlAction");
        }

        #endregion

        [CheckPermission]
        public JsonResult Delete(int id)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Functions_DAO objFunctionsDAO = new CSF_Functions_DAO();
                if (objFunctionsDAO.CheckParentFunction(id))
                {
                    return Json(new { status = false, message = "Không thể xóa chức năng cha" }, JsonRequestBehavior.AllowGet);
                }
                if (objFunctionsDAO.Delete(id))
                {
                    SetAlert("Xóa chức năng thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi xóa chức năng" }, JsonRequestBehavior.AllowGet);
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
                CSF_Functions_DAO objFunctionsDAO = new CSF_Functions_DAO();
                var func = objFunctionsDAO.GetFunctionByID(id);
                ModulesDropDownList();
                ParentFunctionDropDownList();
                string ca = func.Controller_Action;
                List<string> lca = ca.Split('-').ToList();
                ControllerDropDownList(lca[0]);
                ActionDropDownList(lca[0], lca[1]);
                return View(func);
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        public JsonResult EditSubmit(string ID, string Name, string Description, string ParentID, string ModuleID, string Controller_Action)
        {
            try
            {
                CSF_Functions_DAO objFunctionsDAO = new CSF_Functions_DAO();
                CSF_Functions objFunc = new CSF_Functions();
                if (ParentID != "")
                {
                    objFunc.ParentID = Convert.ToInt32(ParentID);
                    if (Convert.ToInt32(ID) == Convert.ToInt32(ParentID))
                    {
                        return Json(new { status = false, message = "Không thể chọn chức năng cha chính nó" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    objFunc.ParentID = 0;
                if (objFunctionsDAO.CheckControllerAction(Convert.ToInt32(ID), Controller_Action))
                {
                    return Json(new { status = false, message = "Controller-Action đã tồn tại!" }, JsonRequestBehavior.AllowGet);
                }
                objFunc.ID = Convert.ToInt32(ID);
                objFunc.Name = Name;
                objFunc.Description = Description;
                objFunc.ModuleID = Convert.ToInt32(ModuleID);
                objFunc.Controller_Action = Controller_Action;
                if (objFunctionsDAO.Update(objFunc))
                {
                    SetAlert("Cập nhật chức năng thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi cập nhật chức năng" }, JsonRequestBehavior.AllowGet);
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
