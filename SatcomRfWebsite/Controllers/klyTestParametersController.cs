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
    public class klyTestParametersController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /klyTestParameters/

        public ActionResult Index()
        {
            return View(db.tblKLYTestParameters.ToList());
        }

        //
        // GET: /klyTestParameters/Details/5

        public ActionResult Details(long id = 0)
        {
            tblKLYTestParameter tblklytestparameter = db.tblKLYTestParameters.Single(t => t.id == id);
            if (tblklytestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestparameter);
        }

        //
        // GET: /klyTestParameters/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /klyTestParameters/Create

        [HttpPost]
        public ActionResult Create(tblKLYTestParameter tblklytestparameter)
        {
            if (ModelState.IsValid)
            {
                db.tblKLYTestParameters.AddObject(tblklytestparameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblklytestparameter);
        }

        //
        // GET: /klyTestParameters/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblKLYTestParameter tblklytestparameter = db.tblKLYTestParameters.Single(t => t.id == id);
            if (tblklytestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestparameter);
        }

        //
        // POST: /klyTestParameters/Edit/5

        [HttpPost]
        public ActionResult Edit(tblKLYTestParameter tblklytestparameter)
        {
            if (ModelState.IsValid)
            {
                db.tblKLYTestParameters.Attach(tblklytestparameter);
                db.ObjectStateManager.ChangeObjectState(tblklytestparameter, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblklytestparameter);
        }

        //
        // GET: /klyTestParameters/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblKLYTestParameter tblklytestparameter = db.tblKLYTestParameters.Single(t => t.id == id);
            if (tblklytestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestparameter);
        }

        //
        // POST: /klyTestParameters/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblKLYTestParameter tblklytestparameter = db.tblKLYTestParameters.Single(t => t.id == id);
            db.tblKLYTestParameters.DeleteObject(tblklytestparameter);
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