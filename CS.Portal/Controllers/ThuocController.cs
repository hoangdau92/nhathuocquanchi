using Core_MVC.Models;
using CS.Portal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core_MVC.Controllers
{
    public class ThuocController : BaseController
    {
        quanchiEntities ctx = new quanchiEntities();
        //
        // GET: /Thuoc/

        public ActionResult Index(int? id, string isspmoi)
        {
            id = id ?? 0;
            var lsp = ctx.QC_LoaiThuoc.FirstOrDefault(x => x.id == id);
            ViewBag.LOAISANPHAM = lsp == null ? "" : lsp.ten;
            var data = ctx.QC_Thuoc.Where(x => (x.idloaithuoc == id || id == 0)).OrderBy(x => x.ten).ToList();
            ViewBag.CATE = id;
            ViewBag.CATENAME = lsp == null ? "Tất cả thuốc" : lsp.ten;
            return View(data);
        }
        public PartialViewResult LoaiThuoc(string title)
        {
            try
            {
                var lsp = ctx.QC_LoaiThuoc.OrderBy(x => x.ten).ToList();
                ViewBag.TITLE = title;
                return PartialView(lsp);
            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return PartialView();
            }
        }
        public ActionResult ChiTiet(int? id)
        {
            id = id ?? 0;
            var sp = ctx.QC_Thuoc.FirstOrDefault(x => x.id == id);
            if (sp != null)
            {
                var spkhac = ctx.QC_Thuoc.Where(x => x.idloaithuoc == sp.idloaithuoc && x.id != sp.id).OrderByDescending(x => x.id).Take(4).ToList();
                TempData["sanphamcungloai"] = spkhac;
                TempData.Keep("sanphamcungloai");
                return View(sp);
            }
            return View();
        }

        public JsonResult Loc(int cate, bool spmoi, bool spuudai, bool spdocquyen)
        {
            try
            {
                //var data = ctx.QC_Thuoc.Where(x => x.idloaithuoc == cate && (x.uudai == spuudai || spuudai == false)
                //                                && (x.sanphammoi == spmoi || spmoi == false)
                //                                && (x.docquyen == spdocquyen || spdocquyen == false)).ToList();
                var data = (from x in ctx.QC_Thuoc
                            where (x.idloaithuoc == cate || cate == 0) && (x.uudai == spuudai || spuudai == false)
                                                && (x.sanphammoi == spmoi || spmoi == false)
                                                && (x.docquyen == spdocquyen || spdocquyen == false)
                            select new { x.id, x.ten, x.gia, x.anhdaidien }).ToList();

                return Json(new { status = true, data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Logs.WriteLog(ex);
                return Json(new { status = false, message = "Lỗi: " + ex }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
