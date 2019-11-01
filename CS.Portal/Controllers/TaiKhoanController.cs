using Core_MVC.Models;
using CS.Portal.Common;
using CS.Portal.Core.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Core_MVC.Controllers
{
    public class TaiKhoanController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /TaiKhoan/

        public ActionResult Index()
        {
            string username = HttpContext.User.Identity.Name;
            if (username != null)
            {
                var data = ctx.QC_KhachHang.Where(x => x.tendangnhap == username).FirstOrDefault();
                var ten_nhom = (from a in ctx.QC_KhachHang_Nhom
                               join b in ctx.QC_NhomKhachHang on a.idnhom equals b.id
                               where a.idkhachhang == data.id
                               orderby b.thutu descending
                               select new { b.ten }).FirstOrDefault();
                ViewBag.NHOM = ten_nhom != null ? ten_nhom.ten : "";
                string url = ConfigurationManager.AppSettings["UrlAvatar"].ToString();
                string SiteUrl = ConfigurationManager.AppSettings["SiteUrl"].ToString();
                ViewBag.DUONGDANANH = SiteUrl + url;
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult SuaThongTin()
        {
            string username = HttpContext.User.Identity.Name;
            if (username != null)
            {
                string url = ConfigurationManager.AppSettings["UrlAvatar"].ToString();
                string SiteUrl = ConfigurationManager.AppSettings["SiteUrl"].ToString();
                ViewBag.DUONGDANANH = SiteUrl + url;
                var data = ctx.QC_KhachHang.Where(x => x.tendangnhap == username).FirstOrDefault();
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DonHang()
        {
            try
            {
                string username = HttpContext.User.Identity.Name;
                if (username != null)
                {
                    var id_khachhang = (from g in ctx.QC_KhachHang
                                        where g.tendangnhap == username
                                        select g.id).FirstOrDefault();
                    var data = ctx.QC_DonHang.OrderBy(x => x.ngaydathang).Where(x => x.idkhachhang == id_khachhang).ToList();
                    return View(data);

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

        //lấy thông tin đơn hàng
        public JsonResult LayThongTinDonHang(int iddonhang)
        {
            try
            {
                var sanpham_donhang = (from tdh in ctx.QC_Thuoc_DonHang
                                       where tdh.iddonhang == iddonhang
                                       select new
                                       {
                                           TenThuoc = tdh.QC_Thuoc.ten,
                                           LoaiThuoc = tdh.QC_Thuoc.QC_LoaiThuoc.ten,
                                           SoLuong = tdh.soluong,
                                           GiaTien = tdh.giatien,
                                           ThanhTien = tdh.thanhtien,
                                           TongTienGiam = tdh.QC_DonHang.sotiengiam,
                                           DonVi = tdh.QC_Thuoc.QC_DonViThuoc.ten,
                                       }).ToList();

                var donhang = (from dh in ctx.QC_DonHang
                               join p in ctx.QC_PhuongThucThanhToan on dh.idphuongthucthanhtoan equals p.id
                               where dh.id == iddonhang
                               select new
                               {
                                   MaDonHang = dh.madonhang,
                                   NgayDatHang = dh.ngaydathang,
                                   PhuongThucThanhToan = p.tieude,
                                   Idmagiamgia = dh.idmagiamgia
                               }).FirstOrDefault();

                //lấy mã giảm giá
                string ten_magiamgia = "";
                if (donhang.Idmagiamgia != null && donhang.Idmagiamgia > 0)
                {
                    var magiamgia = ctx.QC_MaGiamGia.Where(x => x.id == donhang.Idmagiamgia).FirstOrDefault();
                    ten_magiamgia = magiamgia.tenma;
                }
                //lấy tiền sau khi giảm
                var thanhtiensaugiam = (from g in ctx.QC_DonHang
                                        where g.id == iddonhang
                                        select new { g.thanhtiensaugiam, g.sotiengiam }).FirstOrDefault();
                if (sanpham_donhang != null)
                {
                    return Json(new { status = true, data = sanpham_donhang, thanhtien = thanhtiensaugiam, tenmagiamgia = ten_magiamgia, don_hang = donhang }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "Không có đơn hàng nào." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SuaThongTin(string tendaydu, string dienthoai, string matkhau, string diachi, HttpPostedFileBase avatar_file)
        {
            try
            {
                string username = HttpContext.User.Identity.Name;
                if (username != null)
                {
                    var user = ctx.QC_KhachHang.Where(x => x.tendangnhap == username).FirstOrDefault();
                    string strPass = Encryptor.MD5Hash(matkhau);
                    user.tendaydu = tendaydu;
                    if (matkhau != "")
                    {
                        user.matkhau = strPass;
                    }

                    if (avatar_file != null)
                    {
                        if (avatar_file.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(avatar_file.FileName);
                            string _path = Path.Combine(Server.MapPath("~/Images/AnhDaiDien"), _FileName);
                            avatar_file.SaveAs(_path);
                        }
                        user.anhdaidien = avatar_file.FileName;
                    }

                    user.dienthoai = dienthoai;
                    user.diachi = diachi;
                    ctx.Entry(user).State = EntityState.Modified;
                    int cn = ctx.SaveChanges();
                    if (cn > 0)
                    {
                        SetAlert("Cập nhật thành công", AlertType.Success);
                        return RedirectToAction("Index", "TaiKhoan");
                    }
                    else
                    {
                        SetAlert("Cập nhật không thành công", AlertType.Error);
                    }
                    return View(user);
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
        public ActionResult KichHoat(int id, string ma)
        {
            string message = "";
            string cl = "";
            quanchiEntities ctx = new quanchiEntities();
            var kh = ctx.QC_KhachHang.Where(x => x.id == id && x.makichhoat == ma.Trim()).FirstOrDefault();
            if (kh == null)
            {
                message = "Tài khoản không tồn tại. Vui lòng đăng ký tài khoản mới !";
                cl = "error";
            }
            else
            {
                if (kh.kichhoat == false)
                {
                    kh.kichhoat = true;
                    ctx.Entry(kh).State = EntityState.Modified;
                    int nhom_thanhvienmoi = Convert.ToInt32(ConfigurationManager.AppSettings["NhomThanhVienMoi"].ToString());
                    QC_KhachHang_Nhom obj = new QC_KhachHang_Nhom();
                    obj.idkhachhang = id;
                    obj.idnhom = nhom_thanhvienmoi;
                    ctx.QC_KhachHang_Nhom.Add(obj);
                    ctx.SaveChanges();
                    message = "Tài khoản đã được kích hoạt thành công. Hãy đăng nhập trải nghiệm website của chúng tôi. Xin chân thành cảm ơn!";
                    cl = "success";
                }
                else
                {
                    string url = ConfigurationManager.AppSettings["SiteUrl"];
                    return Redirect(url);
                }
            }
            ViewBag.MESS = message;
            ViewBag.CLASS = cl;
            return View();
        }

        public ActionResult Success()
        {
            if (TempData["thongbao"] == null)
            {
                string url = ConfigurationManager.AppSettings["SiteUrl"];
                return Redirect(url);
            }
            return View();
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        public ActionResult QuenMatKhau()
        {
            return View();
        }

        public ActionResult DoiMatKhau(string ma, string email)
        {
            ma = ma.ToLower().Trim();
            email = email.ToLower().Trim();
            quanchiEntities ctx = new quanchiEntities();
            var kh = ctx.QC_KhachHang.Where(x => x.makichhoat == ma && x.email == email).FirstOrDefault();
            if (kh == null)
            {
                string url = ConfigurationManager.AppSettings["SiteUrl"];
                return Redirect(url);
            }
            else
            {
                return View(kh);
            }
        }

        [HttpPost]
        public ActionResult DangNhap(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string strUserName = model.UserName.Trim();
                string strPass = Encryptor.MD5Hash(model.Password);
                quanchiEntities ctx = new quanchiEntities();
                var kh = ctx.QC_KhachHang.Where(x => x.tendangnhap == strUserName && x.matkhau == strPass).FirstOrDefault();
                if (kh == null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                }
                else
                {
                    if (kh.kichhoat == false)
                    {
                        ModelState.AddModelError("", "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email đăng ký của bạn.");
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName.Trim(), false);
                        Session["tenkhachhang"] = kh.tendaydu;
                        string url = ConfigurationManager.AppSettings["SiteUrl"];
                        return Redirect(url);
                    }
                }
            }
            return View(model);
        }

        public ActionResult GopYKhieuNai()
        {

            return View();
        }

        [HttpPost]
        public ActionResult GopYKhieuNai(QC_Gopy_KhieuNai obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (String.IsNullOrEmpty(obj.noidunggopy) == true && String.IsNullOrEmpty(obj.noidungkhieunai) == true)
                    {
                        return Content("Lỗi!");
                    }
                    string username = HttpContext.User.Identity.Name;
                    if (username != null)
                    {
                        //lấy id khách hang
                        var data = (from k in ctx.QC_KhachHang
                                    where k.tendangnhap == username
                                    select new { k.id }).FirstOrDefault();

                        obj.idkhachhang = data.id;
                        obj.ngaygopy = DateTime.Now;
                        ctx.QC_Gopy_KhieuNai.Add(obj);
                        int cn = ctx.SaveChanges();
                        if (cn > 0)
                        {
                            SetAlert("Thêm mới thành công", AlertType.Success);
                            return RedirectToAction("HienThiGopY", "TaiKhoan");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Thêm mới không thành công");
                        }
                        return View("Index");
                    }
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

        //hiển thị gớp ý khiếu nại
        public ActionResult PhanHoi()
        {
            try
            {
                string username = HttpContext.User.Identity.Name;
                if (username != null)
                {
                    var id_khachhang = (from g in ctx.QC_KhachHang
                                        where g.tendangnhap == username
                                        select g.id).FirstOrDefault();
                    var data = ctx.QC_Gopy_KhieuNai.OrderBy(x => x.ngaygopy).Where(x => x.idkhachhang == id_khachhang).ToList();
                    return View(data);
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

        //lấy lịch sử đơn hàng
        public JsonResult LayLichSuDonHang()
        {
            try
            {
                string username = HttpContext.User.Identity.Name;
                if (username != null)
                {
                    var user = ctx.QC_KhachHang.Where(x => x.tendangnhap == username).FirstOrDefault();
                    //var donhang = ctx.QC_DonHang.OrderByDescending(x=>x.ngaydathang).Where(x => x.idkhachhang == user.id).ToList();
                    var donhang = (from dh in ctx.QC_DonHang
                                   where dh.idkhachhang == user.id
                                   select new { dh.madonhang, dh.ngaydathang, dh.thanhtiensaugiam, dh.trangthai, dh.id,dh.diachinhanhang }).ToList();
                    return Json(new { status = true, data = donhang }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        //Thêm mới góp ý phản hồi theo khách hàng
        public JsonResult ThemMoiGopYPhanHoi(string khieunai, string gopy)
        {
            try
            {
                if (String.IsNullOrEmpty(khieunai) == true && String.IsNullOrEmpty(gopy) == true)
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
                string username = HttpContext.User.Identity.Name;
                if (username != null)
                {
                    //lấy id khách hang
                    var data = (from k in ctx.QC_KhachHang
                                where k.tendangnhap == username
                                select new { k.id }).FirstOrDefault();

                    QC_Gopy_KhieuNai obj = new QC_Gopy_KhieuNai();
                    obj.idkhachhang = data.id;
                    obj.noidunggopy = gopy;
                    obj.noidungkhieunai = khieunai;
                    obj.ngaygopy = DateTime.Now;
                    ctx.QC_Gopy_KhieuNai.Add(obj);
                    ctx.SaveChanges();
                    var gopy_phanhoi = ctx.QC_Gopy_KhieuNai;
                    return Json(new { status = true, obj }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        //Xem thông tin phản hồi của khách hàng
        public JsonResult XemNoiDungPhanHoi()
        {
            try
            {
                string username = HttpContext.User.Identity.Name;
                if (username != null)
                {
                    var id_khachhang = (from g in ctx.QC_KhachHang
                                        where g.tendangnhap == username
                                        select g.id).FirstOrDefault();
                    //var phanhoi = ctx.QC_Gopy_KhieuNai.OrderBy(x => x.ngaygopy).Where(x => x.idkhachhang == id_khachhang).ToList();
                    var phanhoi = (from g in ctx.QC_Gopy_KhieuNai
                                   where g.idkhachhang == id_khachhang
                                   orderby g.ngaygopy descending
                                   select new { g.ngaygopy, g.ngayphanhoi, g.noidunggopy, g.noidungkhieunai, g.noidungphanhoi }).ToList();
                    return Json(new { status = true, data = phanhoi }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "Lỗi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GuiXacThucEmail(string email, string tendangnhap)
        {
            try
            {
                if (email != null && email != "")
                {
                    email = email.Trim().ToLower();
                    tendangnhap = tendangnhap.Trim().ToLower();
                    var kh = ctx.QC_KhachHang.Where(x => x.email.ToLower().Trim() == email && x.kichhoat == true && x.tendangnhap == tendangnhap).FirstOrDefault();
                    if (kh == null)
                    {
                        return Json(new { status = false, message = "Khách hàng không tồn tại." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string matkhaumoi = RandomString(6);
                        var encryptedMd5Pass = Encryptor.MD5Hash(matkhaumoi);
                        kh.matkhau = encryptedMd5Pass;
                        ctx.Entry(kh).State = EntityState.Modified;
                        ctx.SaveChanges();
                        sendEmail(matkhaumoi, kh.tendaydu, kh.email, kh.tendangnhap);
                        return Json(new { status = true, message = "Email thay đổi mật khẩu đã được gửi tới <b>" + kh.email + "</b>. Vui lòng kiểm tra hòm thư của bạn." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { status = false, message = "Có lỗi xảy ra." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        private Random random = new Random();

        public string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void sendEmail(string matkhaumoi, string toName, string toMail, string username)
        {
            try
            {
                string breakline = "<br />";
                string fromMail = ConfigurationManager.AppSettings["fromMail"].ToString();
                string fromPassword = ConfigurationManager.AppSettings["fromPassword"].ToString();
                string fromName = ConfigurationManager.AppSettings["fromName"].ToString();
                string fromSubject = "Xác nhận thay đổi mật khẩu";
                string url = ConfigurationManager.AppSettings["SiteUrl"].ToString();
                string linkdangnhap = url + "/tai-khoan/dangnhap";
                var fromAddress = new MailAddress(fromMail, fromName);
                var toAddress = new MailAddress(toMail);
                string body = "Xin chào <b>" + toName + "</b>." + breakline;
                body += "Mật khẩu cho tài khoản " + username + ". Đã được đổi thành: <b>" + matkhaumoi + "</b>"+breakline;
                body += "Hãy <b><a href='" + linkdangnhap + "' target='_blank'> đăng nhập </a></b>lại để thay đổi mật khẩu của mình!" + breakline;
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
    }
}
