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

namespace Core_MVC.Areas.Admin.Controllers.Khac
{
    public class CSF_LogsController : BaseController
    {
        //
        // GET: /Admin/CSF_Logs/

        [CheckPermission]
        public ActionResult Index(string keyWord, int? page, string datetimepicker1, string datetimepicker2)
        {
            try
            {
                CSF_Logs_DAO objDAO = new CSF_Logs_DAO();
                keyWord = keyWord != null ? keyWord : "";
                ViewBag.SearchString = keyWord;
                ViewBag.datetimepicker1 = datetimepicker1;
                ViewBag.datetimepicker2 = datetimepicker2;

                datetimepicker1 = datetimepicker1 != null ? datetimepicker1 : "";
                datetimepicker2 = datetimepicker2 != null ? datetimepicker2 : "";
                if (datetimepicker1 != "")
                {
                    datetimepicker1 = Convert.ToDateTime(datetimepicker1).ToString("MM-dd-yyyy");
                }
                if (datetimepicker1 != "")
                {
                    datetimepicker2 = Convert.ToDateTime(datetimepicker2).ToString("MM-dd-yyyy");
                }
                var data = objDAO.LayTheoTieuChi(keyWord, datetimepicker1, datetimepicker2);
               
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
        [HttpPost]
        [CheckPermission]
        public JsonResult setXoa(string[] arrChecked)
        {
            try
            {
                bool ikq = false;
                string[] arrID = arrChecked;
                CSF_Logs_DAO objDao = new CSF_Logs_DAO();
                int iID = 0;
                if (arrID != null && arrID.Length > 0)
                {
                    for (int i = 0; i < arrID.Length; i++)
                    {
                        iID = Convert.ToInt32(arrID[i].ToString());
                        ikq = objDao.Delete(iID);
                    }

                }
                if (ikq)
                {

                    return Json(new { status = true, message = "Xóa log thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Xóa log không thành công" }, JsonRequestBehavior.AllowGet);
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
