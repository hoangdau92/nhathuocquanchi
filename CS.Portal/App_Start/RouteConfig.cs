using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Core_MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Empty",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Core_MVC.Controllers" }
            );

            //dang nhap admin
            routes.MapRoute(
               name: "Dang Nhap",
               url: "dang-nhap",
               defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Core_MVC.Controllers" }
           );

            //Khach hang dang ky
            routes.MapRoute(
               name: "KhachHangDangKy",
               url: "tao-tai-khoan",
               defaults: new { controller = "Login", action = "KhachHangDangKy", id = UrlParameter.Optional },
               namespaces: new[] { "Core_MVC.Controllers" }
           );

            //Khach hang
            routes.MapRoute(
               name: "KhachHang",
               url: "tai-khoan/{action}",
               defaults: new { controller = "TaiKhoan", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Core_MVC.Controllers" }
           );

            //Gio Hang
            routes.MapRoute(
               name: "Don hang",
               url: "don-hang/{action}",
               defaults: new { controller = "DonHang", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Core_MVC.Controllers" }
           );

            //San pham
            routes.MapRoute(
               name: "Thuoc",
               url: "thuoc/{categoryname}-{id}",
               defaults: new { controller = "Thuoc", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Core_MVC.Controllers" }
           );


            //Chi tiet san pham
            routes.MapRoute(
               name: "ChiTietSanPham",
               url: "chi-tiet-san-pham/{productname}-{id}",
               defaults: new { controller = "Thuoc", action = "ChiTiet", id = UrlParameter.Optional },
               namespaces: new[] { "Core_MVC.Controllers" }
           );



            // routes.MapRoute(
            //    name: "News Category",
            //    url: "{pagekey}/{category}",
            //    defaults: new { controller = "Home", action = "Index", category = UrlParameter.Optional },
            //    namespaces: new[] { "Core_MVC.Controllers" }
            //);

            //gioithieu
            routes.MapRoute(
                 name: "lienhe",
                 url: "lien-he/{action}",
                 defaults: new { controller = "LienHe", action = "Index" },
                 namespaces: new[] { "Core_MVC.Controllers" }
             );

            //danh sach tin tuc
            routes.MapRoute(
                name: "tin tuc",
                url: "tin-tuc/{category}",
                defaults: new { controller = "News", action = "Index", category = UrlParameter.Optional},
                namespaces: new[] { "Core_MVC.Controllers" }
            );

            //chi tiet tin tuc
            routes.MapRoute(
                name: "chi tiet tin tuc",
                url: "{category}/{name}-{id}",
                defaults: new { controller = "News", action = "ChiTietTinTuc", category = UrlParameter.Optional, name = UrlParameter.Optional, id = UrlParameter.Optional },
                namespaces: new[] { "Core_MVC.Controllers" }
            );



            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Core_MVC.Controllers" }
            );

            // routes.MapRoute(
            // name: "News Detail2",
            // url: "{pagekey}/{category}/{pagename}/{pagetype}-{id}",
            // defaults: new { controller = "Home", action = "Index", category = UrlParameter.Optional, pagename = UrlParameter.Optional, pagetype = UrlParameter.Optional, id = UrlParameter.Optional },
            // namespaces: new[] { "Core_MVC.Controllers" }
            // );


        }
    }
}