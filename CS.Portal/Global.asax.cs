using Core_MVC.Models;
using CS.Portal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Core_MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            quanchiEntities ctx = new quanchiEntities();
            var lh = ctx.CSF_ThietLapWebsite.FirstOrDefault();
            Application["tenwebsite"] = lh.tenwebsite;
            Application["slogan"] = lh.slogan;
            Application["hotline"] = lh.hotline;
            Application["hotline_dichvu"] = lh.hotline_dichvu;
            Application["hotline_hotro"] = lh.hotline_hotro;
            Application["logo"] = lh.logo;
            Application["facebook"] = lh.facebook;
            Application["email"] = lh.email;
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
            }

        }
    }
}