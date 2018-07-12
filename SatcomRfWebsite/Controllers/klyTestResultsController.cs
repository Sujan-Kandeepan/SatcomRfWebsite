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
    public class klyTestResultsController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /klyTestResults/

        public ActionResult Index()
        {
            return View(db.tblKLYTestResults.ToList());
        }

        //
        // GET: /klyTestResults/Details/5

        public ActionResult Details(long id = 0)
        {
            tblKLYTestResults tblklytestresults = db.tblKLYTestResults.Single(t => t.id == id);
            if (tblklytestresults == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestresults);
        }

        //
        // GET: /klyTestResults/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /klyTestResults/Create

        [HttpPost]
        public ActionResult Create(tblKLYTestResults tblklytestresults)
        {
            if (ModelState.IsValid)
            {
                db.tblKLYTestResults.Add(tblklytestresults);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblklytestresults);
        }

        //
        // GET: /klyTestResults/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblKLYTestResults tblklytestresults = db.tblKLYTestResults.Single(t => t.id == id);
            if (tblklytestresults == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestresults);
        }

        //
        // POST: /klyTestResults/Edit/5

        [HttpPost]
        public ActionResult Edit(tblKLYTestResults tblklytestresults)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblklytestresults).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblklytestresults);
        }

        //
        // GET: /klyTestResults/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblKLYTestResults tblklytestresults = db.tblKLYTestResults.Single(t => t.id == id);
            if (tblklytestresults == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestresults);
        }

        //
        // POST: /klyTestResults/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblKLYTestResults tblklytestresults = db.tblKLYTestResults.Single(t => t.id == id);
            db.tblKLYTestResults.Remove(tblklytestresults);
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