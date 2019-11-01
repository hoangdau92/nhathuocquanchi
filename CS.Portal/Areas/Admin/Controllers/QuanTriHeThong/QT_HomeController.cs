using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CS.Portal.Common;
using CS.Portal.Core.EF;
using CS.Portal.Core.DAO;
using CS.Portal.App_Start;
using Core_MVC.Models;

namespace Core_MVC.Areas.Admin.Controllers
{
    public class QT_HomeController : BaseController
    {
        //
        // GET: /Admin/Home/

        [CheckPermission]
        public ActionResult Index()
        {
            quanchiEntities ctx = new quanchiEntities();
            int nam = DateTime.Now.Year;
            int thang = DateTime.Now.Month;
            ViewBag.NAM = nam;
            ViewBag.THANG = thang;
            ////
            List<decimal> money = new List<decimal>();
            decimal tienthang = 0;
            var data = ctx.QC_DonHang.Where(x => x.trangthai == true && x.ngaydathang.Year == nam).ToList();
            for (int i = 1; i <= 12; i++)
            {
                tienthang = (decimal)data.Where(x => x.ngaydathang.Month == i).Sum(x => x.thanhtiensaugiam);
                money.Add(tienthang);
            }
            ViewBag.TIENTHEOTHANG = string.Join(",", money.ToArray());
            ////
            var data_lsp = ctx.QC_LoaiThuoc.ToList();
            List<string> tenloai = new List<string>();
            List<int> soluongsp = new List<int>();
            foreach (var item in data_lsp)
            {
                tenloai.Add(item.ten);
                soluongsp.Add(item.QC_Thuoc.Count());
            }
            ViewBag.TENLOAI = string.Join(",", tenloai.ToArray());
            ViewBag.SOLUONGSP = string.Join(",", soluongsp.ToArray());
            ////
            List<string> tensp = new List<string>();
            List<int> soluongban = new List<int>();
            List<SANPHAMBAN> lsp = new List<SANPHAMBAN>();
            var sp = ctx.QC_Thuoc.ToList();
            var sp_dh = ctx.QC_Thuoc_DonHang.Where(x => x.QC_DonHang.ngaydathang.Year == nam
                        && x.QC_DonHang.trangthai == true);
            foreach (var item in sp)
            {
                var temp = sp_dh.Where(x => x.idthuoc == item.id).ToList();
                if (temp != null && temp.Count() > 0)
                {
                    SANPHAMBAN o = new SANPHAMBAN();
                    o.soluong = (int)temp.Sum(x => x.soluong);
                    o.ten = item.ten;
                    lsp.Add(o);
                }
            }
            lsp = lsp.OrderBy(x => x.soluong).ToList();
            ViewBag.TENSP = string.Join(",", lsp.Select(x => x.ten));
            ViewBag.SOLUONGBAN = string.Join(",", lsp.Select(x => x.soluong));
            //thong ke theo khach hang
            List<KHACHHANGMUA> lkh = new List<KHACHHANGMUA>();
            var datakh = ctx.QC_DonHang.Where(x => x.trangthai == true).GroupBy(x => x.idkhachhang).ToList();
            var khs = ctx.QC_KhachHang.Where(x => x.kichhoat == true).ToList();
            foreach (var item in datakh)
            {
                KHACHHANGMUA obj = new KHACHHANGMUA();
                var kh = khs.FirstOrDefault(x => x.id == item.Key);
                obj.ten = kh.tendaydu + " (" + kh.tendangnhap + ")";
                obj.tien = (decimal)data.Where(x => x.idkhachhang == item.Key).Sum(x => x.thanhtiensaugiam);
                lkh.Add(obj);
            }
            lkh = lkh.OrderByDescending(x => x.tien).ToList();
            ViewBag.TENKH = string.Join(",", lkh.Select(x => x.ten));
            ViewBag.TIENMUA = string.Join(",", lkh.Select(x => x.tien));
            //thong tin chung
            ViewBag.SODONHANG = ctx.QC_DonHang.Where(x => x.trangthai == true).Count();
            ViewBag.DHCHUAXULY = ctx.QC_DonHang.Where(x => x.trangthai == false).Count();
            ViewBag.SOTHANHVIEN = ctx.QC_KhachHang.Where(x => x.kichhoat == true).Count();
            ViewBag.SODONHANGTHANG = ctx.QC_DonHang.Where(x => x.trangthai == true && x.ngaydathang.Year == nam && x.ngaydathang.Month == thang).Count();

            return View();
        }

        public class KHACHHANGMUA
        {
            public string ten { get; set; }
            public decimal tien { get; set; }
        }

        public class SANPHAMBAN
        {
            public string ten { get; set; }
            public int soluong { get; set; }
        }

        #region MENU
        public PartialViewResult AdminMenu()
        {
            try
            {
                CSF_MVCEntities entities = new CSF_MVCEntities();
                CSF_Users_DAO objUserDao = new CSF_Users_DAO();
                string username = HttpContext.User.Identity.Name;
                int intGuestGroup = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["IDGuestGroup"]);
                List<int> lRoleID = objUserDao.GetRoleIDByUserName(username, intGuestGroup);
                string stringRoleID = String.Join(",", lRoleID);
                //
                List<CSF_Pages> listAllPage = new List<CSF_Pages>();
                if (username.Trim().ToLower() != "host")
                {
                    var lPageActiveID = entities.CSF_Pages_GetPageByRoleID(stringRoleID).Select(x => (int)x.id).ToList();
                    listAllPage = entities.CSF_Pages.Where(x => lPageActiveID.Contains(x.ID) && x.IsAdmin == true).OrderBy(x => x.Order).ToList();
                }
                else
                {
                    listAllPage = entities.CSF_Pages.Where(x => x.IsAdmin == true && x.IsBlank == false && x.IsActive == true).OrderBy(x => x.Order).ToList();
                }
                //
                string stringMenu = buildTreeMenu(listAllPage);
                MainMenu mainMenu = new MainMenu();
                mainMenu.stringMenu = stringMenu;
                return PartialView(mainMenu);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }

        private string buildTreeMenu(List<CSF_Pages> listAllPage)
        {
            try
            {
                string url = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"] + "/admin/";
                url = url.ToLower();
                string stringMenu = "<ul class='nav' id='side-menu' aria-expanded='false'>";
                stringMenu += "<li><a href='" + url.Replace("/admin", "") + "' target='_blank'><i class='fas fa-home'></i> Về trang chủ</a></li>";
                var parentPage = listAllPage.Where(x => x.ParentID == 0).ToList();
                int level = 1;
                foreach (var page in parentPage)
                {
                    level = 1;
                    var childPage = listAllPage.Where(x => x.ParentID == page.ID).ToList();
                    if (childPage.Any())
                    {
                        stringMenu += "<li>";
                        stringMenu += "<a href='" + url + page.Key + "'>" + page.Icon + ' ' + page.Name + " <span class='fa arrow'></span></a>";
                        stringMenu += getSubMenu(childPage, listAllPage, url, level);
                        stringMenu += "</li>";
                    }
                    else
                    {
                        stringMenu += "<li><a href='" + url + page.Key + "'>" + page.Icon + ' ' + page.Name + "</a></li>";
                    }
                }
                stringMenu += "</ul>";
                return stringMenu;
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                throw;
            }
        }

        private string getSubMenu(List<CSF_Pages> childPage, List<CSF_Pages> listAllPage, string url, int level)
        {
            try
            {
                level++;
                string submenu = "";
                if (level == 2)
                {
                    submenu += "<ul class='nav nav-second-level collapse' aria-expanded='true'>";
                }
                else
                {
                    submenu += "<ul class='nav nav-third-level collapse' aria-expanded='true'>";
                }
                foreach (var page in childPage)
                {
                    var xChildPage = listAllPage.Where(x => x.ParentID == page.ID).ToList();
                    if (xChildPage.Any())
                    {
                        submenu += "<li>";
                        submenu += "<a href='" + url + page.Key + "'>" + page.Icon + ' ' + page.Name + " <span class='fa arrow'></span></a>";
                        submenu += getSubMenu(xChildPage, listAllPage, url, level);
                        submenu += "</li>";
                    }
                    else
                    {
                        submenu += "<li><a href='" + url + page.Key + "'>" + page.Icon + ' ' + page.Name + "</a></li>";
                    }
                }
                submenu += "</ul>";
                return submenu;
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                throw ex;
            }
        }
        #endregion
    }
}
