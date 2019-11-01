using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Portal.Core.EF;
using CS.Portal.Core.DAO;
using CS.Portal.Common;
using PagedList;
using CS.Portal.App_Start;

namespace Core_MVC.Areas.Admin.Controllers.QuanTriHeThong
{
    public class QT_PagesController : BaseController
    {
        //
        // GET: /Admin/QT_Pages/

        [CheckPermission]
        public ActionResult Index(string searchString, string isadmin, int? page)
        {
            try
            {
                CSF_Pages_DAO objDAO = new CSF_Pages_DAO();
                var data = objDAO.Search(searchString, isadmin);
                ViewBag.SearchString = searchString;
                ViewBag.ISADMIN = isadmin;
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
                CSF_Pages_DAO objDAO = new CSF_Pages_DAO();
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                TempData["module"] = MyContext.CSF_Modules.ToList();
                TempData.Keep("module");
                return View();
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        public JsonResult GetPageParent(string isadmin)
        {
            try
            {
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                bool IsAdmin = isadmin == "true" ? true : false;
                List<SubSelectBox> listTree = new List<SubSelectBox>();
                SubSelectBox sc;
                var listData = MyContext.CSF_Pages.Where(x => x.IsAdmin == IsAdmin).ToList();
                var listData0 = listData.Where(x => x.ParentID == 0).OrderBy(x => x.Order).ToList();
                string tag = "";
                foreach (var item in listData0)
                {
                    sc = new SubSelectBox();
                    sc.id = item.ID;
                    sc.name = item.Name;
                    listTree.Add(sc);
                    BuildSubTree(listTree, item.ID, listData, tag);
                }
                var jsonResults = new { listTree, state = true };
                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public void BuildSubTree(List<SubSelectBox> listTree, int ParentID, List<CSF_Pages> listData, string tag)
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

        [CheckPermission]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection fc, CSF_Pages obj)
        {
            try
            {
                TempData.Keep("module");
                TempData.Keep("page");
                if (ModelState.IsValid)
                {
                    CSF_Pages_DAO objDAO = new CSF_Pages_DAO();
                    if (objDAO.CheckExist_PageKey(obj))
                    {
                        SetAlert("Key đã tồn tại!", AlertType.Error);
                        return View();
                    }
                    CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                    int intUserID = objUserDao.GetUserIDByUserName(HttpContext.User.Identity.Name);
                    obj.UserCreate = intUserID;
                    int ReturnID = objDAO.Insert(obj);
                    if (ReturnID > 0)
                    {
                        SetAlert("Thêm page thành công", AlertType.Success);
                        return RedirectToAction("Index", "QT_Pages");
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
        
        [CheckPermission]
        public JsonResult Delete(int id)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Pages_DAO objDAO = new CSF_Pages_DAO();
                if (objDAO.Delete(id))
                {
                    SetAlert("Xóa page thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi xóa page" }, JsonRequestBehavior.AllowGet);
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
                CSF_Pages_DAO objDAO = new CSF_Pages_DAO();
                var page = objDAO.GetPageByID(id);
                SetTempData2SelectList((int)page.ModuleID, (int)page.ParentID, (bool)page.IsAdmin);
                return View(page);
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        protected void SetTempData2SelectList(int moduleid, int pageid, bool IsAdmin)
        {
            CSF_MVCEntities MyContext = new CSF_MVCEntities();
            var listModules = MyContext.CSF_Modules;
            TempData["module"] = new SelectList(listModules, "ID", "Name", moduleid);
            TempData.Keep("module");
            List<SubSelectBox> listTree = new List<SubSelectBox>();
            SubSelectBox sc;
            var listData = MyContext.CSF_Pages.Where(x => x.IsAdmin == IsAdmin).ToList();
            var listData0 = listData.Where(x => x.ParentID == 0).OrderBy(x => x.Order).ToList();
            string tag = "";
            foreach (var item in listData0)
            {
                sc = new SubSelectBox();
                sc.id = item.ID;
                sc.name = item.Name;
                listTree.Add(sc);
                BuildSubTree(listTree, item.ID, listData, tag);
            }
            TempData["page"] = listTree;
            TempData.Keep("page");
        }

        [CheckPermission]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection fc, CSF_Pages obj)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_Pages_DAO objDAO = new CSF_Pages_DAO();
                    if (objDAO.CheckExist_PageKey(obj))
                    {
                        SetAlert("Key đã tồn tại!", AlertType.Error);
                    }
                    else
                    {
                        if (objDAO.Update(obj))
                        {
                            SetAlert("Cập nhật page thành công", AlertType.Success);
                            return RedirectToAction("Index", "QT_Pages");
                        }
                        else
                        {
                            SetAlert("Cập nhật page không thành công", AlertType.Error);
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

        #region TrinhBayPage

        [CheckPermission]
        public ActionResult PageSetup(string id)
        {
            try
            {
                int idpages;
                if (Int32.TryParse(id, out idpages))
                {
                    CSF_MVCEntities ett = new CSF_MVCEntities();
                    var page = ett.CSF_Pages.Find(idpages);
                    ViewBag.PAGE = page.Name;
                    ViewBag.ID = idpages;
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

        public JsonResult GetDataPageSetup(string pageid)
        {
            try
            {
                CSF_MVCEntities ett = new CSF_MVCEntities();
                var modules = from a in ett.CSF_Modules select new { a.ID, a.Name };
                int PAGEID = Convert.ToInt32(pageid);
                CSF_Pages_DAO dao = new CSF_Pages_DAO();
                List<CSF_Pages_GetPartial_Result> lData = dao.GetPartialByPageID(PAGEID, -1);
                var lDataBox = (from a in lData where a.IsBox == true select new { a.ID, a.Title }).ToList();

                List<PagePartialBox> listPPB = new List<PagePartialBox>();
                PagePartialBox ppb;
                foreach (var item in lData.Where(x => x.IsBox == true))
                {
                    ppb = new PagePartialBox();
                    ppb.box = item;
                    ppb.boxChild = lData.Where(x => x.BoxParent == item.ID).ToList();
                    listPPB.Add(ppb);
                }
                //get list all page to copy
                var listPage = (from a in ett.CSF_Pages where a.IsActive == true && a.IsAdmin == false orderby a.Name select new { a.ID, a.Name }).ToList();

                var jsonResults = new { modules, lData, lDataBox, listPPB, listPage, state = true };
                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getPartials(string moduleid)
        {
            try
            {
                CSF_MVCEntities ett = new CSF_MVCEntities();
                int MODULEID = Convert.ToInt32(moduleid);
                var partials = from a in ett.CSF_Partials where a.ModuleID == MODULEID select new { a.ID, a.Name };
                var jsonResults = new { partials, state = true };
                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult SaveSetupPage(CSF_PagePartial obj)
        {
            try
            {
                CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                int intUserID = objUserDao.GetUserIDByUserName(HttpContext.User.Identity.Name);
                obj.UserCreate = intUserID;
                CSF_Pages_DAO dao = new CSF_Pages_DAO();
                int id = dao.PagePartialSave(obj);
                if (id > 0)
                {
                    return Json(new { state = true, message = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { state = false, message = "Lỗi thêm mới" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getDetailPagePartial(string id)
        {
            try
            {
                CSF_MVCEntities ett = new CSF_MVCEntities();
                int ID = Convert.ToInt32(id);
                var obj = (from a in ett.CSF_PagePartial
                           where a.ID == ID
                           join b in ett.CSF_Partials on a.PartialID equals b.ID
                           select new { a.ID, a.PartialID, a.Position, a.IsActive, a.PageID, a.Row, a.Title, a.Order, b.ModuleID, a.IsBox, a.BoxParent }).FirstOrDefault();
                int moduleID = obj != null ? (int)obj.ModuleID : 0;
                //var lPartials = ett.CSF_Partials.Where(x => x.ModuleID == moduleID).ToList();
                var lPartials = (from a in ett.CSF_Partials select new { a.ID, a.Name }).ToList();
                var jsonResults = new { state = true, obj, lPartials };
                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeletePagePartial(string id)
        {
            try
            {
                CSF_MVCEntities ett = new CSF_MVCEntities();
                int ID = Convert.ToInt32(id);
                var pagePartial = ett.CSF_PagePartial.Find(ID);
                if (pagePartial != null)
                {
                    ett.CSF_PagePartial.Remove(pagePartial);
                    ett.SaveChanges();
                    return Json(new { state = true, message = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { state = false, message = "Lỗi xóa dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CopyPageSetup(string pagenguon, string pagedich)
        {
            try
            {
                int IDPageNguon = Convert.ToInt32(pagenguon);
                int IDPageDich = Convert.ToInt32(pagedich);
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                MyContext.CSF_Pages_CopyPage(IDPageNguon, IDPageDich);//delete ban ghi cu
                var listPageNguon = MyContext.CSF_PagePartial.Where(x => x.PageID == IDPageNguon).ToList();
                CSF_PagePartial objPP;
                foreach (var item in listPageNguon.Where(x => x.IsBox == true))
                {
                    int idItem = item.ID;
                    objPP = new CSF_PagePartial();
                    objPP = item;
                    objPP.PageID = IDPageDich;
                    MyContext.CSF_PagePartial.Add(objPP);
                    MyContext.SaveChanges();
                    var newID = objPP.ID;
                    foreach (var itemC in listPageNguon.Where(x => x.BoxParent == idItem))
                    {
                        objPP = new CSF_PagePartial();
                        objPP = itemC;
                        objPP.PageID = IDPageDich;
                        objPP.BoxParent = newID;
                        MyContext.CSF_PagePartial.Add(objPP);
                        MyContext.SaveChanges();
                    }
                }

                return Json(new { state = true, message = "Copy pageSetup thành công" }, JsonRequestBehavior.AllowGet);
                //return Json(new { state = false, message = "Lỗi copy pageSetup" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
