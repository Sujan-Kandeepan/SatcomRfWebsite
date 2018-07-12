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
            tblProductTypes tblproducttypes = db.tblProductTypes.Single(t => t.id == id);
            if (tblproducttypes == null)
            {
                return HttpNotFound();
            }
            return View(tblproducttypes);
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
        public ActionResult Create(tblProductTypes tblproducttypes)
        {
            if (ModelState.IsValid)
            {
                db.tblProductTypes.Add(tblproducttypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblproducttypes);
        }

        //
        // GET: /productType/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblProductTypes tblproducttypes = db.tblProductTypes.Single(t => t.id == id);
            if (tblproducttypes == null)
            {
                return HttpNotFound();
            }
            return View(tblproducttypes);
        }

        //
        // POST: /productType/Edit/5

        [HttpPost]
        public ActionResult Edit(tblProductTypes tblproducttypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblproducttypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblproducttypes);
        }

        //
        // GET: /productType/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblProductTypes tblproducttypes = db.tblProductTypes.Single(t => t.id == id);
            if (tblproducttypes == null)
            {
                return HttpNotFound();
            }
            return View(tblproducttypes);
        }

        //
        // POST: /productType/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblProductTypes tblproducttypes = db.tblProductTypes.Single(t => t.id == id);
            db.tblProductTypes.Remove(tblproducttypes);
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