using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SatcomRfWebsite.Models;

namespace SatcomRfWebsite.Controllers
{
    public class productTypeController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /productType/

        public ActionResult Index()
        {
            return View(db.tblProductTypes.ToList());
        }

        //
        // GET: /productType/Details/5

        public ActionResult Details(long id = 0)
        {
            tblProductType tblproducttype = db.tblProductTypes.Single(t => t.id == id);
            if (tblproducttype == null)
            {
                return HttpNotFound();
            }
            return View(tblproducttype);
        }

        //
        // GET: /productType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /productType/Create

        [HttpPost]
        public ActionResult Create(tblProductType tblproducttype)
        {
            if (ModelState.IsValid)
            {
                db.tblProductTypes.AddObject(tblproducttype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblproducttype);
        }

        //
        // GET: /productType/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblProductType tblproducttype = db.tblProductTypes.Single(t => t.id == id);
            if (tblproducttype == null)
            {
                return HttpNotFound();
            }
            return View(tblproducttype);
        }

        //
        // POST: /productType/Edit/5

        [HttpPost]
        public ActionResult Edit(tblProductType tblproducttype)
        {
            if (ModelState.IsValid)
            {
                db.tblProductTypes.Attach(tblproducttype);
                db.ObjectStateManager.ChangeObjectState(tblproducttype, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblproducttype);
        }

        //
        // GET: /productType/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblProductType tblproducttype = db.tblProductTypes.Single(t => t.id == id);
            if (tblproducttype == null)
            {
                return HttpNotFound();
            }
            return View(tblproducttype);
        }

        //
        // POST: /productType/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblProductType tblproducttype = db.tblProductTypes.Single(t => t.id == id);
            db.tblProductTypes.DeleteObject(tblproducttype);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}