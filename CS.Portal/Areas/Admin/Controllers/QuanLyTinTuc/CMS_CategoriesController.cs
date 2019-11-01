using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Portal.Core.EF;
using System.IO;
using CS.Portal.Core.DAO;
using CS.Portal.Common;
using System.Reflection;
using PagedList;
using CS.Portal.App_Start;
namespace Core_MVC.Areas.Admin.Controllers
{
    public class CMS_CategoriesController : BaseController
    {
        //
        // GET: /Admin/CMS_Categories/

        [CheckPermission]
        public ActionResult Index(string searchString, int? page)
        {
            try
            {
                CMS_Categories_DAO objDAO = new CMS_Categories_DAO();
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
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                CMS_Categories_DAO objDAO = new CMS_Categories_DAO();
                var Partial = objDAO.GetCategoriesByID(id);
                List<SubSelectBox> listTree = new List<SubSelectBox>();
                SubSelectBox sc;
                var listData = MyContext.CMS_Categories.ToList();
                var listData0 = listData.Where(x => x.PARENTCATE == 0).OrderBy(x => x.ORDERS).ToList();
                string tag = "";
                foreach (var item in listData0)
                {
                    sc = new SubSelectBox();
                    sc.id = item.ID;
                    sc.name = item.NAME;
                    listTree.Add(sc);
                    BuildSubTree(listTree, item.ID, listData, tag);
                }
                TempData["PARENTCATE"] = listTree;
                TempData.Keep("PARENTCATE");
                return View(Partial);
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
            try
            {
                CMS_Categories_DAO objDAO = new CMS_Categories_DAO();
                var funcs = objDAO.Search(string.Empty);
                TempData["PARENTCATE"] = funcs.ToList();
                TempData.Keep("PARENTCATE");
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
        public ActionResult Create(FormCollection fc, CMS_Categories obj, HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Data/UploadedFiles/FileLoaiTinBai"), _FileName);
                        file.SaveAs(_path);
                        obj.PICTURE = _FileName;
                    }
                }
                if (ModelState.IsValid)
                {
                    CMS_Categories_DAO objDAO = new CMS_Categories_DAO();
                    if (objDAO.CheckExist_CategoriesKey(obj))
                    {
                        SetAlert("Key đã tồn tại!", AlertType.Error);
                        return View();
                    }

                    int ReturnID = objDAO.Insert(obj);
                    if (ReturnID > 0)
                    {
                        SetAlert("Thêm loại tin bài thành công", AlertType.Success);
                        return RedirectToAction("Index", "CMS_Categories");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm loại tin bài không thành công");
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
                CMS_Categories_DAO objDAO = new CMS_Categories_DAO();
                if (objDAO.CheckDelete(id))
                {
                    if (objDAO.Delete(id))
                    {
                        SetAlert("Xóa loại tin bài thành công", AlertType.Success);
                        return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, message = "Lỗi xóa loại tin bài" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Loại tin bài đang được sử dụng không được phép xóa." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission]
        [HttpPost]
        public ActionResult Edit(FormCollection fc, CMS_Categories obj, HttpPostedFileBase file)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    CMS_Categories_DAO objDAO = new CMS_Categories_DAO();
                    if (objDAO.CheckExist_CategoriesKey(obj))
                    {
                        SetAlert("Key đã tồn tại!", AlertType.Error);
                    }
                    else
                    {

                        if (file != null)
                        {
                            if (file.ContentLength > 0)
                            {
                                string _FileName = Path.GetFileName(file.FileName);
                                // chưa xóa được file cũ
                                //if ( obj.PICTURE!=null)
                                //{
                                //     string _path1 = Path.Combine(Server.MapPath("~/UploadedFiles/FileLoaiTinBai"), obj.PICTURE);
                                //  FileInfo fileDelete = new FileInfo(_path1);
                                //  fileDelete.Delete();
                                //}
                                string _path = Path.Combine(Server.MapPath("~/Data/UploadedFiles/FileLoaiTinBai"), _FileName);
                                file.SaveAs(_path);
                                obj.PICTURE = _FileName;
                            }
                        }

                        if (objDAO.Update(obj))
                        {
                            SetAlert("Cập nhật loại tin bài thành công", AlertType.Success);
                            return RedirectToAction("Index", "CMS_Categories");
                        }
                        else
                        {
                            SetAlert("Cập nhật loại tin bài không thành công", AlertType.Error);
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


        public JsonResult GetPARENTCATE()
        {
            try
            {
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                List<SubSelectBox> listTree = new List<SubSelectBox>();
                SubSelectBox sc;
                var listData = MyContext.CMS_Categories.ToList();
                var listData0 = listData.Where(x => x.PARENTCATE == 0).OrderBy(x => x.ORDERS).ToList();
                string tag = "";
                foreach (var item in listData0)
                {
                    sc = new SubSelectBox();
                    sc.id = item.ID;
                    sc.name = item.NAME;
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
        public void BuildSubTree(List<SubSelectBox> listTree, int ParentID, List<CMS_Categories> listData, string tag)
        {
            try
            {
                SubSelectBox sc;
                var listChild = listData.Where(x => x.PARENTCATE == ParentID);
                if (listChild != null && listChild.Count() > 0)
                {
                    tag += "--- ";
                    foreach (var item in listChild)
                    {
                        sc = new SubSelectBox();
                        sc.id = item.ID;
                        sc.name = tag + item.NAME;
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

    }
}
