using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CS.Portal.Common;
using CS.Portal.Core.EF;
using CS.Portal.Core.DAO;
using Newtonsoft.Json.Linq;
using CS.Portal.App_Start;
using Core_MVC.Models;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QC_KhachHang_NhomController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Admin/QC_KhachHang_Nhom/
        [CheckPermission]
        public ActionResult Index()
        {
            try
            {
                var khachHang_Nhom = (from n in ctx.QC_NhomKhachHang
                                      select n).OrderBy(x => x.thutu).ToList();
                TempData["khachhang_nhom"] = khachHang_Nhom;
                var khang_hang = ctx.QC_KhachHang.Where(x => x.kichhoat == true).ToList();
                TempData["khachhang"] = khang_hang;
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
        public String SetUsersInGroup(string[] arrChecked, string RoleID)
        {
            try
            {
                string[] arrUserID = arrChecked;
                int intRoleID = Convert.ToInt32(RoleID);
                int intUserID = 0;
                if (arrUserID != null && arrUserID.Length > 0)
                {
                    for (int i = 0; i < arrUserID.Length; i++)
                    {
                        intUserID = Convert.ToInt32(arrUserID[i].ToString());
                        QC_KhachHang_Nhom obj = new QC_KhachHang_Nhom();
                        obj.idkhachhang = intUserID;
                        obj.idnhom = intRoleID;
                        ctx.QC_KhachHang_Nhom.Add(obj);
                    }
                    ctx.SaveChanges();
                    return "true";
                }
                return "false";
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return "false";
            }
        }

        [CheckPermission]
        [HttpPost]
        public String SetUsersOutGroup(string[] arrChecked, string RoleID)
        {
            try
            {
                string[] arrUserID = arrChecked;
                int intRoleID = Convert.ToInt32(RoleID);
               

                if (arrUserID != null && arrUserID.Length > 0)
                {
                    for (int i = 0; i < arrUserID.Length; i++)
                    {
                        int intKhachhang = Convert.ToInt32(arrUserID[i]);
                       
                        var obj = ctx.QC_KhachHang_Nhom.Where(x => x.idkhachhang == intKhachhang && x.idnhom == intRoleID).FirstOrDefault();
                        ctx.QC_KhachHang_Nhom.Remove(obj);
                    }
                    ctx.SaveChanges();
                }
                return "true";
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return "false";
            }
        }

        [CheckPermission]
        public JsonResult GetUsersGroup(string Name, int RoleID)
        {
            try
            {
                Name = Name == null ? "" : Name;
                ctx.Configuration.ProxyCreationEnabled = false;
                var khachhang_ingroup = (from a in ctx.QC_KhachHang_Nhom
                                         join b in ctx.QC_KhachHang on a.idkhachhang equals b.id
                                         where a.idnhom == RoleID
                                         select new {b.tendangnhap, b.tendaydu, b.id }).ToList();
                List<int> lIDKH_ingroup = khachhang_ingroup.Select(x => x.id).ToList();
                var khachhang_outgroup = ctx.QC_KhachHang.Where(x => !lIDKH_ingroup.Contains(x.id)).ToList();
                khachhang_outgroup = khachhang_outgroup.Where(x => x.tendaydu.Contains(Name.Trim())).ToList();
                var jsonResults = new { listUserNotInGroup = khachhang_outgroup, listUserInGroup = khachhang_ingroup, status = true };
                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return null;
            }
        }
    }
}
