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
    public class sspaTestParametersController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /sspaTestParameters/

        public ActionResult Index()
        {
            return View(db.tblSSPATestParameters.ToList());
        }

        //
        // GET: /sspaTestParameters/Details/5

        public ActionResult Details(int id = 0)
        {
            tblSSPATestParameters tblsspatestparameters = db.tblSSPATestParameters.Single(t => t.id == id);
            if (tblsspatestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestparameters);
        }

        //
        // GET: /sspaTestParameters/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /sspaTestParameters/Create

        [HttpPost]
        public ActionResult Create(tblSSPATestParameters tblsspatestparameters)
        {
            if (ModelState.IsValid)
            {
                db.tblSSPATestParameters.Add(tblsspatestparameters);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblsspatestparameters);
        }

        //
        // GET: /sspaTestParameters/Edit/5

        public ActionResult Edit(int id = 0)
        {
            tblSSPATestParameters tblsspatestparameters = db.tblSSPATestParameters.Single(t => t.id == id);
            if (tblsspatestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestparameters);
        }

        //
        // POST: /sspaTestParameters/Edit/5

        [HttpPost]
        public ActionResult Edit(tblSSPATestParameters tblsspatestparameters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblsspatestparameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblsspatestparameters);
        }

        //
        // GET: /sspaTestParameters/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblSSPATestParameters tblsspatestparameters = db.tblSSPATestParameters.Single(t => t.id == id);
            if (tblsspatestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestparameters);
        }

        //
        // POST: /sspaTestParameters/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblSSPATestParameters tblsspatestparameters = db.tblSSPATestParameters.Single(t => t.id == id);
            db.tblSSPATestParameters.Remove(tblsspatestparameters);
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