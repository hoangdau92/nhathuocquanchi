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

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QT_RoleFunctionController : BaseController
    {
        //
        // GET: /Admin/QT_RoleFunctions/

        [CheckPermission]
        public ActionResult Index(string search, int? page)
        {
            try
            {
                CSF_RoleFunction_DAO objR_FDAO = new CSF_RoleFunction_DAO();
                List<CSF_RoleFunction_LayTatCa_Result> lRF = objR_FDAO.Search(search);
                ViewBag.search = search;
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                //functions = functions.OrderBy(s => s.ID);
                var test = lRF.ToPagedList(pageNumber, pageSize);
                int a = test.Count();
                return View(test);
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
                CSF_RoleFunction_DAO objFunctionsDAO = new CSF_RoleFunction_DAO();
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                TempData["nhomNguoiDung"] = MyContext.CSF_Roles.OrderBy(x => x.Name).ToList();
                TempData.Keep("nhomNguoiDung");
                TempData["chucNang"] = MyContext.CSF_Functions.OrderBy(x => x.Name).ToList();
                TempData.Keep("chucNang");
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
        public ActionResult Create(FormCollection fc, CSF_RoleFunction obj)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_RoleFunction_DAO objRFDAO = new CSF_RoleFunction_DAO();
                    if (objRFDAO.CheckExist_RF(obj))
                    {
                        SetAlert("Nhóm - Chức năng đã tồn tại!", AlertType.Error);
                    }
                    else
                    {
                        int ReturnID = objRFDAO.Insert(obj);
                        if (ReturnID > 0)
                        {
                            SetAlert("Thêm Nhóm - Chức năng thành công", AlertType.Success);
                            return RedirectToAction("Index", "QT_RoleFunction");
                        }
                        else
                        {
                            SetAlert("Thêm Nhóm - Chức năng không thành công", AlertType.Error);
                        }
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

        [CheckPermission]
        public ActionResult Edit(int id)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");

                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                var rf = MyContext.CSF_RoleFunction.Find(id);
                if (rf != null)
                {
                    var selectListItems = MyContext.CSF_Roles.OrderBy(x => x.Name).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }).ToList();
                    TempData["nhomNguoiDung"] = new SelectList(selectListItems, "Value", "Text", rf.RoleID);
                    TempData.Keep("nhomNguoiDung");


                    selectListItems = MyContext.CSF_Functions.OrderBy(x => x.Name).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }).ToList();
                    TempData["chucNang"] = new SelectList(selectListItems, "Value", "Text", rf.FunctionID);
                    TempData.Keep("chucNang");
                    return View();
                }
                else
                    return RedirectToAction("Index", "QT_RoleFunction");
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
        public ActionResult Edit(FormCollection fc, CSF_RoleFunction obj)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_RoleFunction_DAO objRFDAO = new CSF_RoleFunction_DAO();
                    if (objRFDAO.CheckExist_RF(obj))
                    {
                        SetAlert("Nhóm - Chức năng đã tồn tại!", AlertType.Error);
                    }
                    else
                    {
                        if (objRFDAO.Update(obj))
                        {
                            SetAlert("Cập nhật Nhóm - Chức năng thành công", AlertType.Success);
                            return RedirectToAction("Index", "QT_RoleFunction");
                        }
                        else
                        {
                            SetAlert("Cập nhật Nhóm - Chức năng không thành công", AlertType.Error);
                        }
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

        [CheckPermission]
        public JsonResult Delete(int id)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_RoleFunction_DAO objRF = new CSF_RoleFunction_DAO();
                if (objRF.Delete(id))
                {
                    SetAlert("Xóa Nhóm - Chức năng thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi xóa Nhóm - Chức năng" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }


        //[CheckPermission]
        public JsonResult GetDataPermission()
        {
            try
            {
                CSF_MVCEntities ett = new CSF_MVCEntities();
                var lRole = from a in ett.CSF_Roles select new { a.ID, a.Name };
                var lFunc = from a in ett.CSF_Functions
                            join b in ett.CSF_Modules on a.ModuleID equals b.ID
                            orderby a.ModuleID
                            select new { a.ID, a.Name, a.ModuleID, ModuleName = b.Name };
                var lPage = from a in ett.CSF_Pages
                            join b in ett.CSF_Modules on a.ModuleID equals b.ID
                            orderby a.ModuleID
                            select new { a.ID, a.Name, a.ModuleID, ModuleName = b.Name };
                var lModule = from a in ett.CSF_Modules orderby a.Name select new { a.ID, a.Name };
                var jsonResults = new { lRole, lFunc, lPage, lModule, state = true };
                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        //[CheckPermission]
        public JsonResult GetPermissionByRole(string RoleID, string ModuleID, string isadmin)
        {
            try
            {
                CSF_MVCEntities ett = new CSF_MVCEntities();
                int ROLEID = Convert.ToInt32(RoleID);
                int MODULEID = Convert.ToInt32(ModuleID);
                bool ISADMIN = Convert.ToBoolean(isadmin);
                List<int> lFuncByRole = ett.CSF_RoleFunction.Where(x => x.RoleID == ROLEID).Select(x => (int)x.FunctionID).ToList();
                List<int> lPageByRole = ett.CSF_PageRole.Where(x => x.RoleID == ROLEID).Select(x => (int)x.PageID).ToList();
                #region jsTreePage
                List<jsTree> jsTreeList = new List<jsTree>();
                jsTree objTree = new jsTree();
                jsTreeState state;
                var dataAll = ett.CSF_Pages.Where(x => x.IsAdmin == ISADMIN).OrderBy(x => x.Order).ToList();
                var data = dataAll.Where(x => x.ParentID == 0);
                foreach (var item in data)
                {
                    objTree = new jsTree();
                    state = new jsTreeState();
                    objTree.id = item.ID;
                    objTree.text = item.Name;
                    if (lPageByRole.Contains(item.ID))
                    {
                        state.selected = true;
                    }
                    objTree.children = getChild(item.ID, dataAll, lPageByRole);
                    objTree.state = state;
                    jsTreeList.Add(objTree);
                }
                #endregion
                #region jsTreeFunction
                List<jsTree> jsTreeListF = new List<jsTree>();
                jsTree objTreeF = new jsTree();
                jsTreeState stateF;
                var dataAllF = ett.CSF_Functions.Where(x => x.ModuleID == MODULEID).ToList();
                var dataF = dataAllF.Where(x => x.ParentID == 0);
                foreach (var item in dataF)
                {
                    objTreeF = new jsTree();
                    stateF = new jsTreeState();
                    objTreeF.id = item.ID;
                    objTreeF.text = item.Name;
                    if (lFuncByRole.Contains(item.ID))
                    {
                        stateF.selected = true;
                    }
                    objTreeF.children = getChildF(item.ID, dataAllF, lFuncByRole);
                    objTreeF.state = stateF;
                    jsTreeListF.Add(objTreeF);
                }
                #endregion
                var jsonResults = new { lFuncByRole, lPageByRole, jsTreeList, jsTreeListF, state = true };
                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        #region DEQUYJSTREE
        public List<jsTree> getChild(int ParentID, List<CSF_Pages> dataAll, List<int> lPageByRole)
        {
            try
            {
                List<jsTree> jsTreeList = new List<jsTree>();
                jsTree objTree;
                jsTreeState state;
                var data = dataAll.Where(x => x.ParentID == ParentID);
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        objTree = new jsTree();
                        state = new jsTreeState();
                        objTree.id = item.ID;
                        objTree.text = item.Name;
                        if (lPageByRole.Contains(item.ID))
                        {
                            state.selected = true;
                        }
                        objTree.children = getChild(item.ID, dataAll, lPageByRole);
                        objTree.state = state;
                        jsTreeList.Add(objTree);
                    }
                }
                return jsTreeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<jsTree> getChildF(int ParentID, List<CSF_Functions> dataAll, List<int> lFuncByRole)
        {
            try
            {
                List<jsTree> jsTreeList = new List<jsTree>();
                jsTree objTree;
                jsTreeState state;
                var data = dataAll.Where(x => x.ParentID == ParentID);
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        objTree = new jsTree();
                        state = new jsTreeState();
                        objTree.id = item.ID;
                        objTree.text = item.Name;
                        if (lFuncByRole.Contains(item.ID))
                        {
                            state.selected = true;
                        }
                        objTree.children = getChildF(item.ID, dataAll, lFuncByRole);
                        objTree.state = state;
                        jsTreeList.Add(objTree);
                    }
                }
                return jsTreeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


//        [CheckPermission]
        public JsonResult SaveRoleFunction(string RoleID, string ModuleID, string FuncIDs)
        {
            try
            {
                int ROLEID = Convert.ToInt32(RoleID);
                int MODULEID = Convert.ToInt32(ModuleID);
                CSF_RoleFunction_DAO dao = new CSF_RoleFunction_DAO();
                int result = dao.AddPermission(ROLEID, MODULEID, FuncIDs);
                if (result > 0)
                {
                    return Json(new { state = true, message = "Gán quyền chức năng thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { state = false, message = "Lỗi gán quyền chức năng" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }


        //[CheckPermission]
        public JsonResult SavePageRole(string RoleID, string PageIDs, string isadmin)
        {
            try
            {
                int ROLEID = Convert.ToInt32(RoleID);
                bool ISADMIN = Convert.ToBoolean(isadmin);
                CSF_PageRole_DAO dao = new CSF_PageRole_DAO();
                int result = dao.AddPermission(ROLEID, PageIDs, ISADMIN);
                if (result > 0)
                {
                    return Json(new { state = true, message = "Gán quyền page thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { state = false, message = "Lỗi gán quyền page" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
