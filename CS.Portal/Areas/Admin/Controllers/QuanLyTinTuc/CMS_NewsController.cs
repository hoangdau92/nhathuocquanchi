using CS.Portal.Core.DAO;
using CS.Portal.Core.EF;
using CS.Portal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using CS.Portal.App_Start;
using System.Data.Entity;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class CMS_NewsController : BaseController
    {
        //
        // GET: /Admin/News/
        [CheckPermission]
        public ActionResult Index(string keyWord, string category, string newsStatus, int? page)
        {
            try
            {
                int categoryID = category != null ? Convert.ToInt32(category) : 0;
                int newsStatusID = newsStatus != null ? Convert.ToInt32(newsStatus) : 0;
                keyWord = keyWord != null ? keyWord : "";
                CMS_News_DAO objDAO = new CMS_News_DAO();
                var data = objDAO.Search(keyWord, categoryID, newsStatusID);
                ViewBag.SearchString = keyWord;
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
                CMS_News_DAO objDAO = new CMS_News_DAO();
                CSF_MVCEntities MVCContext = new CSF_MVCEntities();
                //var listData = MVCContext.CMS_Categories.Where(x => x.PUBLISH == true).AsQueryable().Select(new x SubSelectBox { }).ToList();
                List<SubSelectBox> listTree = new List<SubSelectBox>();
                SubSelectBox sc;
                var listData = (from a in MVCContext.CMS_Categories where a.PUBLISH == true select new SubSelectBox() { id = a.ID, name = a.NAME, parentid = (int)a.PARENTCATE }).ToList();
                var listData0 = listData.Where(x => x.parentid == 0).ToList();
                string tag = "";
                foreach (var item in listData0)
                {
                    sc = new SubSelectBox();
                    sc.id = item.id;
                    sc.name = item.name;
                    listTree.Add(sc);
                    BuildSubTree(listTree, item.id, listData, tag);
                }
                TempData["categories"] = listTree;
                TempData.Keep("categories");
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
        [ValidateInput(false)]
        public ActionResult Create(FormCollection fc, CMS_News obj)
        {
            try
            {
                TempData.Keep("categories");
                if (ModelState.IsValid)
                {

                    CMS_News_DAO objDAO = new CMS_News_DAO();
                    CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                    int userID = objUserDao.GetUserIDByUserName(HttpContext.User.Identity.Name);
                    obj.ID_USERCREATE = userID;
                    if (obj.PICTURE == null || obj.PICTURE == "")
                    {
                        obj.PICTURE = "/Images/webimg/noimage.png";
                    }
                    int ReturnID = objDAO.Insert(obj);
                    if (ReturnID > 0)
                    {
                        SetAlert("Thêm tin tức thành công", AlertType.Success);
                        return RedirectToAction("Index", "CMS_News");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm tin tức không thành công");
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
                CSF_MVCEntities entity = new CSF_MVCEntities();
                List<SubSelectBox> listTree = new List<SubSelectBox>();
                SubSelectBox sc;
                var listData = (from a in entity.CMS_Categories where a.PUBLISH == true select new SubSelectBox() { id = a.ID, name = a.NAME, parentid = (int)a.PARENTCATE }).ToList();
                var listData0 = listData.Where(x => x.parentid == 0).ToList();
                string tag = "";
                foreach (var item in listData0)
                {
                    sc = new SubSelectBox();
                    sc.id = item.id;
                    sc.name = item.name;
                    listTree.Add(sc);
                    BuildSubTree(listTree, item.id, listData, tag);
                }
                TempData["categories"] = listTree;
                TempData.Keep("categories");
                var news = entity.CMS_News.Find(id);
                if (news != null)
                {
                    return View(news);
                }
                return RedirectToAction("Index", "CMS_News");
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
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection fc, CMS_News obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CMS_News_DAO objDAO = new CMS_News_DAO();
                    CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                    int userID = objUserDao.GetUserIDByUserName(HttpContext.User.Identity.Name);
                    obj.ID_USERMODIFY = userID;
                    bool update = false;
                    update = objDAO.Update(obj);
                    if (update)
                    {
                        SetAlert("Cập nhật tin tức thành công", AlertType.Success);
                        return RedirectToAction("Index", "CMS_News");
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
                CMS_News_DAO objDAO = new CMS_News_DAO();
                if (objDAO.Delete(id))
                {
                    SetAlert("Xóa tin thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi xóa tin" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission]
        public JsonResult GuiDuyet(string newsid)
        {
            try
            {
                CSF_MVCEntities entity = new CSF_MVCEntities();
                int NEWSID = Convert.ToInt32(newsid);
                var news = entity.CMS_News.Find(NEWSID);
                if (news != null && (news.ID_NEWS_STATUS == 1 || news.ID_NEWS_STATUS == 3 || news.ID_NEWS_STATUS == 6))
                {
                    news.ID_NEWS_STATUS = 2;
                    entity.Entry(news).State = EntityState.Modified;
                    entity.SaveChanges();
                }
                SetAlert("Gửi duyệt thành công", AlertType.Success);
                return Json(new { state = true, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public void BuildSubTree(List<SubSelectBox> listTree, int ParentID, List<SubSelectBox> listData, string tag)
        {
            try
            {
                SubSelectBox sc;
                var listChild = listData.Where(x => x.parentid == ParentID);
                if (listChild != null && listChild.Count() > 0)
                {
                    tag += "--- ";
                    foreach (var item in listChild)
                    {
                        sc = new SubSelectBox();
                        sc.id = item.id;
                        sc.name = tag + item.name;
                        listTree.Add(sc);
                        BuildSubTree(listTree, item.id, listData, tag);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [CheckPermission]
        public ActionResult ChiTiet(int id)
        {
            try
            {
                ViewBag.URLIMAGE = System.Configuration.ConfigurationManager.AppSettings["UrlImage"];
                CSF_MVCEntities entity = new CSF_MVCEntities();
                var news = entity.CMS_News.Find(id);
                if (news != null )
                {
                    var loaitintuc = entity.CMS_Categories.Find(news.ID_CATEGORIES);
                    TempData["categories"] = loaitintuc.NAME;
                    TempData.Keep("categories");
                    return View(news);
                }
                return RedirectToAction("Index", "CMS_News");
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
