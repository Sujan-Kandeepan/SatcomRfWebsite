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
    public class sspaTestResultsController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /sspaTestResults/

        public ActionResult Index()
        {
            return View(db.tblSSPATestResults.ToList());
        }

        //
        // GET: /sspaTestResults/Details/5

        public ActionResult Details(long id = 0)
        {
            tblSSPATestResult tblsspatestresult = db.tblSSPATestResults.Single(t => t.id == id);
            if (tblsspatestresult == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestresult);
        }

        //
        // GET: /sspaTestResults/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /sspaTestResults/Create

        [HttpPost]
        public ActionResult Create(tblSSPATestResult tblsspatestresult)
        {
            if (ModelState.IsValid)
            {
                db.tblSSPATestResults.AddObject(tblsspatestresult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblsspatestresult);
        }

        //
        // GET: /sspaTestResults/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblSSPATestResult tblsspatestresult = db.tblSSPATestResults.Single(t => t.id == id);
            if (tblsspatestresult == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestresult);
        }

        //
        // POST: /sspaTestResults/Edit/5

        [HttpPost]
        public ActionResult Edit(tblSSPATestResult tblsspatestresult)
        {
            if (ModelState.IsValid)
            {
                db.tblSSPATestResults.Attach(tblsspatestresult);
                db.ObjectStateManager.ChangeObjectState(tblsspatestresult, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblsspatestresult);
        }

        //
        // GET: /sspaTestResults/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblSSPATestResult tblsspatestresult = db.tblSSPATestResults.Single(t => t.id == id);
            if (tblsspatestresult == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestresult);
        }

        //
        // POST: /sspaTestResults/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblSSPATestResult tblsspatestresult = db.tblSSPATestResults.Single(t => t.id == id);
            db.tblSSPATestResults.DeleteObject(tblsspatestresult);
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