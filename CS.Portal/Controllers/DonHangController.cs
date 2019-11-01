using Core_MVC.Common;
using Core_MVC.Models;
using CS.Portal.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Core_MVC.Controllers
{
    public class DonHangController : BaseController
    {
        //
        // GET: /DonHang/
        quanchiEntities ctx = new quanchiEntities();
        public ActionResult Index()
        {
            TempData["ptthanhtoan"] = ctx.QC_PhuongThucThanhToan.Where(x => x.kichhoat == true).ToList();
            TempData.Keep("ptthanhtoan");
            ViewBag.MADONHANG = "QC" + ConvertToUnixTime(DateTime.Now).ToString();
            return View();
        }

        public long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }

        public JsonResult AddToCart(int idthuoc, int quantity)
        {
            try
            {
                int soluong = 0;
                var sp = ctx.QC_Thuoc.Find(idthuoc);
                if (sp == null)
                {
                    return Json(new { status = false, message = "Oops! Lỗi cơ sở dữ liệu" }, JsonRequestBehavior.AllowGet);
                }
                if (Session["cart"] == null)
                {
                    List<ShoppingCart> lCart = new List<ShoppingCart>();
                    lCart.Add(new ShoppingCart
                    {
                        idsanpham = idthuoc,
                        donvitinh = sp.QC_DonViThuoc.ten,
                        giatien = (decimal)sp.gia
                                ,
                        loaisanpham = sp.QC_LoaiThuoc.ten,
                        tensanpham = sp.ten,
                        soluong = quantity,
                        anhdaidien = sp.anhdaidien
                    });
                    Session["cart"] = lCart;
                    soluong = 1;
                }
                else
                {
                    List<ShoppingCart> lCart = (List<ShoppingCart>)Session["cart"];
                    var cart = lCart.FirstOrDefault(x => x.idsanpham == idthuoc);
                    if (cart == null)
                    {
                        lCart.Add(new ShoppingCart
                        {
                            idsanpham = idthuoc,
                            donvitinh = sp.QC_DonViThuoc.ten,
                            giatien = (decimal)sp.gia
                                ,
                            loaisanpham = sp.QC_LoaiThuoc.ten,
                            tensanpham = sp.ten,
                            soluong = quantity,
                            anhdaidien = sp.anhdaidien
                        });
                    }
                    else
                    {
                        cart.soluong += quantity;
                    }
                    Session["cart"] = lCart;
                    soluong = lCart.Count();
                }
                return Json(new { status = true, message = "Sản phẩm: " + sp.ten + " đã được thêm vào giỏ hàng", soluong = soluong }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateQuantity(int idsanpham, int quantity)
        {
            try
            {
                List<ShoppingCart> lCart = new List<ShoppingCart>();
                if (Session["cart"] != null)
                {
                    lCart = (List<ShoppingCart>)Session["cart"];
                    var cart = lCart.FirstOrDefault(x => x.idsanpham == idsanpham);
                    cart.soluong = quantity;
                    Session["cart"] = lCart;
                    return Json(new { status = true, lCart = lCart }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "Không tồn tại sản phẩm." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveProduct(int idsanpham)
        {
            try
            {
                List<ShoppingCart> lCart = new List<ShoppingCart>();
                if (Session["cart"] != null)
                {
                    lCart = (List<ShoppingCart>)Session["cart"];
                    var cart = lCart.FirstOrDefault(x => x.idsanpham == idsanpham);
                    lCart.Remove(cart);
                    Session["cart"] = lCart;
                    return Json(new { status = true, lCart = lCart, soluong = lCart.Count() }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "Không tồn tại sản phẩm." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ThanhToan(string magiamgia, string ghichu, int ptthanhtoan, decimal sotiengiam, string madonhang, string diachigiaohang)
        {
            try
            {
                string username = HttpContext.User.Identity.Name;
                var kh = ctx.QC_KhachHang.Where(x => x.tendangnhap == username).FirstOrDefault();
                var mgg = ctx.QC_MaGiamGia.Where(x => x.tenma.ToLower().Trim() == magiamgia.ToLower().Trim() && x.kichhoat == true).FirstOrDefault();
                List<ShoppingCart> lCart = new List<ShoppingCart>();
                if (Session["cart"] != null && kh != null)
                {
                    lCart = (List<ShoppingCart>)Session["cart"];
                    QC_DonHang dh = new QC_DonHang();
                    dh.sotiengiam = 0;
                    if (mgg != null)
                    {
                        dh.idmagiamgia = mgg.id;
                        dh.sotiengiam = sotiengiam;
                    }
                    dh.madonhang = madonhang;
                    dh.ngaydathang = DateTime.Now;
                    dh.idkhachhang = kh.id;
                    dh.ghichu = ghichu;
                    dh.trangthai = false;
                    dh.diachinhanhang = diachigiaohang;
                    dh.idphuongthucthanhtoan = ptthanhtoan;
                    ctx.QC_DonHang.Add(dh);
                    ctx.SaveChanges();
                    decimal tongthanhtien = 0;
                    if (dh.id > 0)
                    {
                        foreach (var item in lCart)
                        {
                            QC_Thuoc_DonHang t_dh = new QC_Thuoc_DonHang();
                            t_dh.iddonhang = dh.id;
                            t_dh.idthuoc = item.idsanpham;
                            t_dh.giatien = item.giatien;
                            t_dh.soluong = item.soluong;
                            t_dh.thanhtien = item.soluong * item.giatien;
                            ctx.QC_Thuoc_DonHang.Add(t_dh);
                            tongthanhtien += item.soluong * item.giatien;
                        }
                        ctx.SaveChanges();
                    }
                    dh.thanhtientruocgiam = tongthanhtien;
                    dh.thanhtiensaugiam = tongthanhtien - dh.sotiengiam;
                    ctx.Entry(dh).State = EntityState.Modified;
                    ctx.SaveChanges();
                    sendEmail("quản trị viên", HttpContext.Application["email"].ToString(), dh.madonhang, dh.QC_KhachHang.tendaydu);
                    TempData["donhang"] = dh;
                    Session["cart"] = null;
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "Có lỗi xảy ra!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        private void sendEmail(string toName, string toMail, string madonhang, string tenkhachhang)
        {
            try
            {
                string breakline = "<br />";
                string fromMail = ConfigurationManager.AppSettings["fromMail"].ToString();
                string fromPassword = ConfigurationManager.AppSettings["fromPassword"].ToString();
                string fromName = ConfigurationManager.AppSettings["fromName"].ToString();
                string fromSubject = "Thông báo có đơn hàng mới";
                string url = ConfigurationManager.AppSettings["SiteUrl"].ToString();
                var fromAddress = new MailAddress(fromMail, fromName);
                var toAddress = new MailAddress(toMail);
                string body = "Xin chào <b>" + toName + "</b>." + breakline;
                body += "Đơn hàng <b>" + madonhang + "</b> do khách hàng <b>" + tenkhachhang + "</b> đặt đã được tạo thành công. Vui lòng kiểm tra và xử lý." + breakline;
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

        public ActionResult ThanhCong()
        {
            try
            {
                //var dh = ctx.QC_DonHang.OrderByDescending(x => x.id).FirstOrDefault();
                //TempData["donhang"] = dh;
                if (TempData["donhang"] != null)
                {
                    QC_DonHang data = (QC_DonHang)TempData["donhang"];
                    var dh = ctx.QC_DonHang.Where(x => x.id == data.id).FirstOrDefault();
                    string ten_mgg = "";
                    if (dh.idmagiamgia != null && dh.idmagiamgia != 0)
                    {
                        var mgg = ctx.QC_MaGiamGia.Find(dh.idmagiamgia);
                        ten_mgg = mgg == null ? "" : mgg.tenma;
                    }
                    ViewBag.MGG = ten_mgg;
                    return View(dh);
                }
                else
                    return RedirectToAction("", "Home");
            }
            catch (Exception ex)
            {
                SetAlert("Lỗi" + ex.Message.ToString(), AlertType.Error);
                Logs.WriteLog(ex);
                return View();
            }
        }

        public decimal TinhSoTienGiam(bool all, int loai_giamgia, decimal giatrigiam, List<int> doituongs)
        {
            decimal sotiengiam = 0;
            List<ShoppingCart> lCart = new List<ShoppingCart>();
            if (Session["cart"] != null)
            {
                lCart = (List<ShoppingCart>)Session["cart"];
                if (all)
                {
                    if (loai_giamgia == LOAI_GIAMGIA.PHANTRAM)
                    {
                        foreach (var item in lCart)
                        {
                            sotiengiam += item.giatien * item.soluong * giatrigiam / 100;
                        }
                    }
                    else
                    {
                        sotiengiam = giatrigiam;
                    }
                }
                else
                {
                    if (loai_giamgia == LOAI_GIAMGIA.PHANTRAM)
                    {
                        foreach (var item in lCart)
                        {
                            if (doituongs.Contains(item.idsanpham))
                            {
                                sotiengiam += item.giatien * item.soluong * giatrigiam / 100;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in lCart)
                        {
                            if (doituongs.Contains(item.idsanpham))
                            {
                                sotiengiam += giatrigiam * item.soluong;
                            }
                        }
                    }
                }

            }
            return Math.Round(sotiengiam, 0);
        }

        public JsonResult ApDungMaGiamGia(string magiamgia)
        {
            try
            {
                var mgg = ctx.QC_MaGiamGia.Where(x => x.tenma.ToLower().Trim() == magiamgia.ToLower().Trim() && x.kichhoat == true).FirstOrDefault();
                if (mgg == null)
                {
                    return Json(new { status = false, message = "Mã giảm giá không tồn tại !" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (mgg.tungay <= DateTime.Now && mgg.denngay >= DateTime.Now)
                    {
                        var doituongs = ctx.QC_DoiTuong_MaGiamGia.Where(x => x.idmagiamgia == mgg.id).ToList();
                        List<int> listid_dtgiamgia = doituongs.Select(x => (int)x.iddoituong).ToList();
                        var all_doituong = doituongs.Where(x => x.iddoituong == -1).FirstOrDefault();
                        string username = HttpContext.User.Identity.Name;
                        var kh = ctx.QC_KhachHang.Where(x => x.tendangnhap == username).FirstOrDefault();
                        decimal sotiengiam = 0;
                        ////
                        if (mgg.idkieudoituong == KIEU_DOITUONG.SANPHAM)
                        {
                            if (all_doituong == null)
                            {
                                sotiengiam = TinhSoTienGiam(false, (int)mgg.idloaigiamgia, (decimal)mgg.giatri, listid_dtgiamgia);
                                return Json(new { status = true, giatrigiam = sotiengiam }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                sotiengiam = TinhSoTienGiam(true, (int)mgg.idloaigiamgia, (decimal)mgg.giatri, listid_dtgiamgia);
                                return Json(new { status = true, giatrigiam = sotiengiam }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (mgg.idkieudoituong == KIEU_DOITUONG.NHOMKHACHHANG)
                        {
                            var thuocnhom = ctx.QC_KhachHang_Nhom.Where(x => x.idkhachhang == kh.id && listid_dtgiamgia.Contains((int)x.idnhom)).ToList();
                            if (thuocnhom != null && thuocnhom.Count() > 0 || all_doituong != null)
                            {
                                sotiengiam = TinhSoTienGiam(true, (int)mgg.idloaigiamgia, (decimal)mgg.giatri, listid_dtgiamgia);
                                return Json(new { status = true, giatrigiam = sotiengiam }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { status = false, message = "Bạn không thuộc đối tượng được giảm giá !" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (mgg.idkieudoituong == KIEU_DOITUONG.KHACHHANG)
                        {
                            if (listid_dtgiamgia.Contains(kh.id) || all_doituong != null)
                            {
                                sotiengiam = TinhSoTienGiam(true, (int)mgg.idloaigiamgia, (decimal)mgg.giatri, listid_dtgiamgia);
                                return Json(new { status = true, giatrigiam = sotiengiam }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { status = false, message = "Bạn không thuộc đối tượng được giảm giá !" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        return Json(new { status = true, message = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, message = "Mã giảm giá đã hết hạn sử dụng !" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Getcart()
        {
            try
            {
                List<ShoppingCart> lCart = new List<ShoppingCart>();
                if (Session["cart"] != null)
                {
                    lCart = (List<ShoppingCart>)Session["cart"];
                    return Json(new { status = true, lCart = lCart }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = false, message = "Bạn chưa chọn sản phẩm" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
