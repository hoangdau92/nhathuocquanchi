using CS.Portal.Common;
using CS.Portal.Core.DAO;
using CS.Portal.Core.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Core_MVC.Common;

namespace Core_MVC.Controllers
{
    public class NewsController : BaseController
    {
        //
        // GET: /News/

        public ActionResult Index(string category, int? page)
        {

            CSF_MVCEntities ctx = new CSF_MVCEntities();
            var cate = ctx.CMS_Categories.Where(x => x.KEY == category).FirstOrDefault();
            if (cate != null)
            {
                ViewBag.CATE = cate.NAME;
                var data = ctx.CMS_News.Where(x => x.ID_CATEGORIES == cate.ID && x.ID_NEWS_STATUS == NewsStatus.DaCongBo).OrderByDescending(x => x.CREATEDATE).ToList();
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(data.ToPagedList(pageNumber, pageSize));
            }
            return View();
        }

        public PartialViewResult ChiTietTinTuc(string id)
        {
            try
            {
                int newsid = Convert.ToInt32(id);
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                var news = MyContext.CMS_News.Find(newsid);
                if (news != null)
                {
                    int cateid = (int)news.ID_CATEGORIES;
                    var cate = MyContext.CMS_Categories.Find(cateid);
                    string catekey = cate.KEY;
                    ViewBag.CateName = cate.NAME;
                    var tinlienquan = MyContext.CMS_News.Where(x => x.ID_NEWS_STATUS == NewsStatus.DaCongBo && x.ID_CATEGORIES == cateid
                            && x.ID != newsid).OrderByDescending(x => x.CREATEDATE).Take(4).ToList();
                    TempData["tinlienquan"] = tinlienquan;
                    TempData.Keep("tinlienquan");
                }
                return PartialView(news);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        public PartialViewResult TinTucMoi()
        {
            try
            {
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                var news = MyContext.CMS_News.Where(x => x.ID_NEWS_STATUS == NewsStatus.DaCongBo).OrderByDescending(x => x.CREATEDATE).Take(6).ToList();
                return PartialView(news);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        public PartialViewResult DanhMucTinTuc()
        {
            try
            {
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                var cates = MyContext.CMS_Categories.Where(x => x.PUBLISH == true).OrderBy(x => x.ORDERS).ToList();
                return PartialView(cates);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        public PartialViewResult TinNoiBat()
        {
            try
            {
                CSF_MVCEntities MyContext = new CSF_MVCEntities();
                var news = MyContext.CMS_News.Where(x => x.ID_NEWS_STATUS == NewsStatus.DaCongBo && x.ISFOCUS == 1).OrderByDescending(x => x.CREATEDATE).Take(3).ToList();
                return PartialView(news);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

    }
}
