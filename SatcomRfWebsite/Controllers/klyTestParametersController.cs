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
            tblKLYTestParameters tblklytestparameters = db.tblKLYTestParameters.Single(t => t.id == id);
            if (tblklytestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestparameters);
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
        public ActionResult Create(tblKLYTestParameters tblklytestparameters)
        {
            if (ModelState.IsValid)
            {
                db.tblKLYTestParameters.Add(tblklytestparameters);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblklytestparameters);
        }

        //
        // GET: /klyTestParameters/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblKLYTestParameters tblklytestparameters = db.tblKLYTestParameters.Single(t => t.id == id);
            if (tblklytestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestparameters);
        }

        //
        // POST: /klyTestParameters/Edit/5

        [HttpPost]
        public ActionResult Edit(tblKLYTestParameters tblklytestparameters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblklytestparameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblklytestparameters);
        }

        //
        // GET: /klyTestParameters/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblKLYTestParameters tblklytestparameters = db.tblKLYTestParameters.Single(t => t.id == id);
            if (tblklytestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestparameters);
        }

        //
        // POST: /klyTestParameters/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblKLYTestParameters tblklytestparameters = db.tblKLYTestParameters.Single(t => t.id == id);
            db.tblKLYTestParameters.Remove(tblklytestparameters);
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