using Core_MVC.Models;
using CS.Portal.App_Start;
using CS.Portal.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core_MVC.Areas.Admin.Controllers.jFood
{
    public class CSF_ThietLapWebsiteController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/CSF_ThietLapWebsite/
        [CheckPermission]
        public ActionResult Index()
        {
            ViewBag.URLIMAGE = System.Configuration.ConfigurationManager.AppSettings["UrlImage"];
            var tt = ctx.CSF_ThietLapWebsite.FirstOrDefault();
            return View(tt);
        }


        [CheckPermission]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(FormCollection fc, CSF_ThietLapWebsite obj)
        {
            try
            {
                ViewBag.URLIMAGE = System.Configuration.ConfigurationManager.AppSettings["UrlImage"];
                obj.hienthi_logo =true;
                if (ModelState.IsValid)
                {
                    ctx.Entry(obj).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        HttpContext.Application["tenwebsite"] = obj.tenwebsite;
                        HttpContext.Application["slogan"] = obj.slogan;
                        HttpContext.Application["hotline"] = obj.hotline;
                        HttpContext.Application["hotline_dichvu"] = obj.hotline_dichvu;
                        HttpContext.Application["hotline_hotro"] = obj.hotline_hotro;
                        HttpContext.Application["logo"] = obj.logo;
                        HttpContext.Application["facebook"] = obj.facebook;
                        HttpContext.Application["email"] = obj.email;
                        return RedirectToAction("Index", "CSF_ThietLapWebsite");
                    }
                    else
                    {
                        SetAlert("Cập nhật không thành công", AlertType.Error);
                    }

                }
                SetAlert("Cập nhật không thành công", AlertType.Error);
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
