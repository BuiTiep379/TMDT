using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopGiay.Models;
using PagedList;
using System.Net;
using System.Data.Entity;

namespace ShopGiay.Areas.Admin.Controllers
{
    public class QLPromoCodeController : Controller
    {
        ShopGiayEntities db = new ShopGiayEntities();
        // GET: Admin/QLPromoCode
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachPromoCode(string search, int? page, int? size)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });

            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.Size = items;
            ViewBag.CurrentSize = size;

            page = (page ?? 1);
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 5);
            var listCode = from c in db.PROMOCODEs select c;
            listCode = listCode.OrderBy(x => x.Id);

            if (search != null)
            {
                listCode = listCode.Where(x => x.Name.Contains(search));
            }
            return View(listCode.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult KichHoatCode(int id)
        {
            var code = db.PROMOCODEs.Find(id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;

            }   
            else
            {
                code.Status = true;
                db.Entry(code).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachPromoCode");
            }    
        }
        public ActionResult HuyCode(int id)
        {
            var code = db.PROMOCODEs.Find(id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;

            }
            else
            {
                code.Status = false;
                db.Entry(code).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachPromoCode");
            }
        }
        [HttpGet]
        public ActionResult AddPromoCode()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPromoCode(FormCollection form)
        {
            string code = form["Code"].ToString();
            string name = form["Name"].ToString();
            decimal value = decimal.Parse(form["Value"].ToString());
            bool status = bool.Parse(form["Status"]);
            var kt = db.PROMOCODEs.Where(x => x.Code == code);
            if (kt.Count() == 0)
            {
                var c = new PROMOCODE()
                {
                    Name = name,
                    Code = code,
                    Value = value,
                    Status = status,
                };
                db.PROMOCODEs.Add(c);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm code thành công!";
                return View(c);
            }
            else
            {
                TempData["ThongBao"] = "Code đã tồn tại!";
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditPromoCode(int? id)
        {
            PROMOCODE code = db.PROMOCODEs.SingleOrDefault(x => x.Id == id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(code);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPromoCode(PROMOCODE code)
        {
            if (ModelState.IsValid)
            {
                db.Entry(code).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ThongBao"] = "Chỉnh sửa code thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Chỉnh sửa code không thành công!";
            }
            return View(code);
        }

        public ActionResult DeletePromoCode(int? id)
        {
            PROMOCODE code = db.PROMOCODEs.SingleOrDefault(x => x.Id == id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(code);
        }

        [HttpPost, ActionName("DeletePromoCode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            PROMOCODE code = db.PROMOCODEs.SingleOrDefault(x => x.Id == id);
            if (code == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.PROMOCODEs.Remove(code);
            db.SaveChanges();
            return RedirectToAction("DanhSachPromoCode");
        }
    }
}