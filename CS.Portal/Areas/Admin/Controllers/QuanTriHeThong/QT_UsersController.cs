using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CS.Portal.Common;
using CS.Portal.Core.EF;
using CS.Portal.Core.DAO;
using PagedList;
using CS.Portal.App_Start;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QT_UsersController : BaseController
    {
        [CheckPermission]
        public ActionResult Index(string searchString, int? page)
        {
            try
            {
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                var users = objUsersDAO.Search(searchString);
                ViewBag.SearchString = searchString;
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                users = users.OrderBy(s => s.ID);
                return View(users.ToPagedList(pageNumber, pageSize));
                //return View(users);
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
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                var roles = objRolesDAO.GetAll();
                TempData["Roles"] = roles.ToList();
                TempData.Keep("Roles");
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
        public ActionResult Create(FormCollection fc, CSF_Users user)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                if (ModelState.IsValid)
                {
                    CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                    //Kiểm tra trùng tên tài khoản
                    if (objUsersDAO.CheckUserName(user.UserName.Trim()))
                    {
                        ModelState.AddModelError("", "Tên người dùng đã tồn tại!");
                        TempData.Keep("Roles");
                        return View();
                    }

                    int ReturnUserID = objUsersDAO.Insert(user);
                    if (ReturnUserID > 0)
                    {
                        SetAlert("Thêm người dùng thành công", AlertType.Success);
                        //Thêm người dùng vào nhóm
                        CSF_UserRole_DAO objUserRoleDAO = new CSF_UserRole_DAO();
                        if (fc["chkRole_"] != null)
                        {
                            string[] arrRoleCheckBox = fc["chkRole_"].Split(',');
                            int intRoleID = 0;
                            for (int i = 0; i < arrRoleCheckBox.Length; i++)
                            {
                                intRoleID = Convert.ToInt32(arrRoleCheckBox[i].ToString());
                                objUserRoleDAO.Insert(user.ID, intRoleID);
                            }
                        }
                        return RedirectToAction("Index", "QT_Users");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm người dùng không thành công");
                    }
                    return View("Index");
                }
                TempData.Keep("Roles");
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
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                if (objUsersDAO.Delete(id))
                {
                    SetAlert("Xóa người dùng thành công", AlertType.Success);
                    return Json(new { status = true, message = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, message = "Lỗi xóa người dùng" }, JsonRequestBehavior.AllowGet);
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
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                var user = objUsersDAO.GetUserByID(id);
                if (user ==null )  return RedirectToAction("Index", "QT_Users");
                CSF_Roles_DAO objRolesDAO = new CSF_Roles_DAO();
                var roles = objRolesDAO.GetAll();
                TempData["Roles"] = roles.ToList();
                TempData.Keep("Roles");
                //Hiển thị Role đã có của User
                CSF_UserRole_DAO objUserRoleDAO = new CSF_UserRole_DAO();
                List<CSF_UserRole_GetByUser_Result> userRole = objUserRoleDAO.GetListRoleByUserID(id);
                TempData["userRole"] = userRole;
                TempData.Keep("userRole");
                return View(user);
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
        public ActionResult Edit(FormCollection fc, CSF_Users user)
        {
            try
            {
                //if (!this.HasPermission) return RedirectToAction("Unauthorized", "Home");
                //Xóa nhóm hiện tại của người dùng
                CSF_UserRole_DAO objUserRoleDAO = new CSF_UserRole_DAO();
                objUserRoleDAO.DelRoleByUserID(user.ID);
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                if (objUsersDAO.Update(user))
                {
                    if (fc["chkRole_"] != null)
                    {
                        string[] arrRoleCheckBox = fc["chkRole_"].Split(',');
                        int intRoleID = 0;
                        for (int i = 0; i < arrRoleCheckBox.Length; i++)
                        {
                            intRoleID = Convert.ToInt32(arrRoleCheckBox[i].ToString());
                            objUserRoleDAO.Insert(user.ID, intRoleID);
                        }
                    }
                    SetAlert("Cập nhật người dùng thành công", AlertType.Success);
                    return RedirectToAction("Index", "QT_Users");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật người dùng không thành công");
                }
                return View("Index");
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        public ActionResult UpdateInformation(string id)
        {
            try
            {
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                var user = objUsersDAO.GetByUserName(id);
                if (user == null) return RedirectToAction("Index", "QT_Home");
                if (id.ToLower().Trim() != HttpContext.User.Identity.Name.ToLower().Trim()) return RedirectToAction("Index", "QT_Home");
                ViewBag.ID = user.ID;
                return View(user);
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateInformation(FormCollection fc, CSF_Users user)
        {
            try
            {
                CSF_Users_DAO objUsersDAO = new CSF_Users_DAO();
                if (objUsersDAO.Update(user))
                {
                    SetAlert("Cập nhật thông tin thành công", AlertType.Success);
                    ViewBag.ID = user.ID;
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thông tin không thành công");
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

    }
}
