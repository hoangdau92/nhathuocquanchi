using Core_MVC.Models;
using CS.Portal.App_Start;
using CS.Portal.Common;
using CS.Portal.Core.DAO;
using CS.Portal.Core.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Core_MVC.Controllers
{
    public class LoginController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Login/

        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("", "qt_home", new { area = "admin" });
            }
            else
                return View();
        }

        public ActionResult KhachHangDangKy()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("", "home");
            }
            else
                return View();
        }
        private Random random = new Random();

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void sendEmail(string maxacthuc, string toName, string toMail, int idkh)
        {
            try
            {
                string breakline = "<br />";
                string fromMail = ConfigurationManager.AppSettings["fromMail"].ToString();
                string fromPassword = ConfigurationManager.AppSettings["fromPassword"].ToString();
                string fromName = ConfigurationManager.AppSettings["fromName"].ToString();
                string fromSubject = ConfigurationManager.AppSettings["fromSubject"].ToString();
                string url = ConfigurationManager.AppSettings["SiteUrl"].ToString();
                string linkkichhoat = url + "/tai-khoan/kichhoat?id=" + idkh + "&ma=" + maxacthuc;
                var fromAddress = new MailAddress(fromMail, fromName);
                var toAddress = new MailAddress(toMail);
                string body = "Xin chào <b>" + toName + "</b>." + breakline;
                body += "Cảm ơn bạn đã đăng ký thành viên của nhà thuốc Quân Chi. Hãy <b><a href='" + linkkichhoat + "' target='_blank'> nhấn vào đây </a></b> để kích hoạt tài khoản của bạn!" + breakline;
                body += "Trân trọng cảm ơn!" + breakline;
                body += "<i>(Đây là tin nhắn tự động, vui lòng không trả lời tin nhắn này)<i>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = fromSubject,
                    Body = body
                })
                {
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
            }
        }

        [HttpPost]
        public ActionResult KhachHangDangKy(FormCollection fc, QC_KhachHang obj, HttpPostedFileBase file)
        {
            try
            {
                var khach_hang = ctx.QC_KhachHang.Where(x => x.tendangnhap.ToLower() == obj.tendangnhap.ToLower()).FirstOrDefault();
                if (khach_hang != null)
                {
                    ModelState.AddModelError("", "Đã tên đăng nhập đã tồn tại!");
                    return View();
                }
                khach_hang = ctx.QC_KhachHang.Where(x => x.email.Trim().ToLower() == obj.email.Trim().ToLower()).FirstOrDefault();
                if (khach_hang != null)
                {
                    ModelState.AddModelError("", "Email này đã tồn tại, vui lòng sử dụng email khác!");
                    return View();
                }
                if (file !=null)
                {
                    if (file.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Images/AnhDaiDien"), _FileName);
                        file.SaveAs(_path);
                    }
                    obj.anhdaidien = file.FileName;
                }
                string maxacthuc = RandomString(8);
                var encryptedMd5Pass = Encryptor.MD5Hash(obj.matkhau);
                obj.matkhau = encryptedMd5Pass;
                obj.ngaydangky = DateTime.Now;
                obj.diemtichluy = 0;
                obj.kichhoat = false;
                obj.makichhoat = maxacthuc;
                obj.anhdaidien = "/Images/webimg/noimage.png";
                ctx.QC_KhachHang.Add(obj);
                ctx.SaveChanges();
                if (obj.id > 0)
                {
                    SetAlert("Tạo tài khoản thành công", AlertType.Success);
                    sendEmail(maxacthuc, obj.tendaydu, obj.email, obj.id);
                    TempData["thongbao"] = "Tài khoản đã được tạo thành công. Một email kích hoạt đã được gửi tới " + obj.email + ", vui lòng kiểm tra hộp thư và xác nhận. Xin cảm ơn !";
                    string url = ConfigurationManager.AppSettings["SiteUrl"] + "/tai-khoan/success";
                    return Redirect(url);
                }
                else
                {
                    ModelState.AddModelError("", "Thêm mới không thành công");
                    RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return View();
            }
        }


        #region Login,Logout

        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string strUserName = model.UserName.Trim();
                string strPass = Encryptor.MD5Hash(model.Password);
                CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                int intResult = objUserDao.Login(strUserName, strPass);
                switch (intResult)
                {
                    case 0://Tên đăng nhập hoặc mật khẩu không đúng
                        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                        break;
                    case 1://Đăng nhập thành công
                        if (model.RememberPass)
                        {
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, model.UserName.Trim(), DateTime.Now, DateTime.Now.AddMinutes(60), false, "");
                            //Encrypt the ticket
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            //Create a cookie and add the encrypted ticket to the cookie as data
                            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                            //Add the cookie to the outgoing cookie collection
                            Response.Cookies.Add(authCookie);
                        }
                        FormsAuthentication.SetAuthCookie(model.UserName.Trim(), false);
                        string url = ConfigurationManager.AppSettings["SiteUrl"];
                        return Redirect(url + "/admin/qt_home");
                    case -1:
                        ModelState.AddModelError("", "Tài khoản chưa được kích hoạt!");
                        break;
                    case -2:
                        ModelState.AddModelError("", "Mật khẩu không đúng!");
                        break;
                    default:
                        break;
                }
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Session["cart"] = null;
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}
