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

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QT_UserRoleController : BaseController
    {
        [CheckPermission]
        public ActionResult Index()
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                var roles = objRolesDAO.GetAll();
                TempData["Roles"] = roles.ToList();
                var users = objUsersDAO.Search("").ToList();
                TempData["Users"] = users;
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
                CSF_UserRole_DAO objUserRoleDAO = new CSF_UserRole_DAO();
                if (arrUserID != null && arrUserID.Length > 0)
                {
                    for (int i = 0; i < arrUserID.Length; i++)
                    {
                        intUserID = Convert.ToInt32(arrUserID[i].ToString());
                        objUserRoleDAO.Insert(intUserID, intRoleID);
                    }
                    return "true";
                }
                return "false";
            }
            catch (Exception ex)
            {
                //SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
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
                CSF_UserRole_DAO objUserRoleDAO = new CSF_UserRole_DAO();
                if (arrUserID != null && arrUserID.Length > 0)
                {
                    string listUserID = "(";
                    for (int i = 0; i < arrUserID.Length; i++)
                    {
                        listUserID += arrUserID[i].ToString() + ",";
                    }
                    listUserID = listUserID.Substring(0, listUserID.Length - 1);
                    listUserID += ")";
                    if (objUserRoleDAO.RemoveUsersOutGroup(intRoleID, listUserID))
                    {
                        return "true";
                    }
                }
                return "false";
            }
            catch (Exception ex)
            {
                //SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return "false";
            }
        }

        //[CheckPermission]
        public JsonResult GetUsersGroup(string Name, string RoleID)
        {
            try
            {
                int intRoleID = Convert.ToInt32(RoleID);
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                List<CSF_Users_GetUsersNotInGroup_Result> listUser = objUsersDAO.GetUsersNotInGroup(Name, intRoleID).ToList();
                //List user in group
                List<CSF_Users_GetUsersInGroup_Result> listUser1 = objUsersDAO.GetUsersInGroup(intRoleID).ToList();

                var jsonResults = new { listUserNotInGroup = listUser, listUserInGroup = listUser1, status = true };
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
