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
            tblSSPATestParameter tblsspatestparameter = db.tblSSPATestParameters.Single(t => t.id == id);
            if (tblsspatestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestparameter);
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
        public ActionResult Create(tblSSPATestParameter tblsspatestparameter)
        {
            if (ModelState.IsValid)
            {
                db.tblSSPATestParameters.AddObject(tblsspatestparameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblsspatestparameter);
        }

        //
        // GET: /sspaTestParameters/Edit/5

        public ActionResult Edit(int id = 0)
        {
            tblSSPATestParameter tblsspatestparameter = db.tblSSPATestParameters.Single(t => t.id == id);
            if (tblsspatestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestparameter);
        }

        //
        // POST: /sspaTestParameters/Edit/5

        [HttpPost]
        public ActionResult Edit(tblSSPATestParameter tblsspatestparameter)
        {
            if (ModelState.IsValid)
            {
                db.tblSSPATestParameters.Attach(tblsspatestparameter);
                db.ObjectStateManager.ChangeObjectState(tblsspatestparameter, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblsspatestparameter);
        }

        //
        // GET: /sspaTestParameters/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblSSPATestParameter tblsspatestparameter = db.tblSSPATestParameters.Single(t => t.id == id);
            if (tblsspatestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tblsspatestparameter);
        }

        //
        // POST: /sspaTestParameters/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblSSPATestParameter tblsspatestparameter = db.tblSSPATestParameters.Single(t => t.id == id);
            db.tblSSPATestParameters.DeleteObject(tblsspatestparameter);
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