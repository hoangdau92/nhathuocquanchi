using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Portal.Core.DAO;
using CS.Portal.Core.EF;
using CS.Portal.Common;
using System.Web.Security;
using Core_MVC.Areas.Admin.Controllers;
using System.Data;
using Core_MVC.Models;

namespace Core_MVC.Controllers
{
    public class HomeController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        public ActionResult Index()
        {
            return View(); 
        }


        

        public PartialViewResult DangNhap(string title)
        {
            try
            {
                ViewBag.TITLE = title;
                return PartialView();
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        public PartialViewResult UnderConstruction(string title)
        {
            try
            {
                ViewBag.TITLE = title;
                return PartialView();
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        public PartialViewResult _Blank(string title)
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }


        public PartialViewResult Slider()
        {
            try
            {
                var slider = ctx.QC_Slider.OrderBy(x => x.thutu).ToList();
                return PartialView(slider);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        public PartialViewResult YKienKhachHang()
        {
            try
            {
                var ykien = ctx.QC_YKienKhachHang.Where(x=>x.hienthi == true).OrderBy(x => x.thutu).ToList();
                return PartialView(ykien);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        public PartialViewResult DoiTacTieuBieu()
        {
            try
            {
                var doitac = ctx.QC_DoiTacTieuBieu.OrderBy(x => x.thutu).ToList();
                return PartialView(doitac);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }


    }
}
