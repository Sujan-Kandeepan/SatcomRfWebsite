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
            tblSSPATestResults tblsspatestresults = db.tblSSPATestResults.Single(t => t.id == id);
            if (tblsspatestresults == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestresults);
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
        public ActionResult Create(tblSSPATestResults tblsspatestresults)
        {
            if (ModelState.IsValid)
            {
                db.tblSSPATestResults.Add(tblsspatestresults);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblsspatestresults);
        }

        //
        // GET: /sspaTestResults/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblSSPATestResults tblsspatestresults = db.tblSSPATestResults.Single(t => t.id == id);
            if (tblsspatestresults == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestresults);
        }

        //
        // POST: /sspaTestResults/Edit/5

        [HttpPost]
        public ActionResult Edit(tblSSPATestResults tblsspatestresults)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblsspatestresults).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblsspatestresults);
        }

        //
        // GET: /sspaTestResults/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblSSPATestResults tblsspatestresults = db.tblSSPATestResults.Single(t => t.id == id);
            if (tblsspatestresults == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestresults);
        }

        //
        // POST: /sspaTestResults/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblSSPATestResults tblsspatestresults = db.tblSSPATestResults.Single(t => t.id == id);
            db.tblSSPATestResults.Remove(tblsspatestresults);
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