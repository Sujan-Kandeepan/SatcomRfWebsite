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
            tblKLYTestResult tblklytestresult = db.tblKLYTestResults.Single(t => t.id == id);
            if (tblklytestresult == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestresult);
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
        public ActionResult Create(tblKLYTestResult tblklytestresult)
        {
            if (ModelState.IsValid)
            {
                db.tblKLYTestResults.AddObject(tblklytestresult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblklytestresult);
        }

        //
        // GET: /klyTestResults/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblKLYTestResult tblklytestresult = db.tblKLYTestResults.Single(t => t.id == id);
            if (tblklytestresult == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestresult);
        }

        //
        // POST: /klyTestResults/Edit/5

        [HttpPost]
        public ActionResult Edit(tblKLYTestResult tblklytestresult)
        {
            if (ModelState.IsValid)
            {
                db.tblKLYTestResults.Attach(tblklytestresult);
                db.ObjectStateManager.ChangeObjectState(tblklytestresult, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblklytestresult);
        }

        //
        // GET: /klyTestResults/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblKLYTestResult tblklytestresult = db.tblKLYTestResults.Single(t => t.id == id);
            if (tblklytestresult == null)
            {
                return HttpNotFound();
            }
            return View(tblklytestresult);
        }

        //
        // POST: /klyTestResults/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblKLYTestResult tblklytestresult = db.tblKLYTestResults.Single(t => t.id == id);
            db.tblKLYTestResults.DeleteObject(tblklytestresult);
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