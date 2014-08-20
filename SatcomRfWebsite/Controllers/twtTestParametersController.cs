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
    public class twtTestParametersController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /twtTestParameters/

        public ActionResult Index()
        {
            return View(db.tblTWTTestParameters.ToList());
        }

        //
        // GET: /twtTestParameters/Details/5

        public ActionResult Details(int id = 0)
        {
            tblTWTTestParameter tbltwttestparameter = db.tblTWTTestParameters.Single(t => t.id == id);
            if (tbltwttestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestparameter);
        }

        //
        // GET: /twtTestParameters/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /twtTestParameters/Create

        [HttpPost]
        public ActionResult Create(tblTWTTestParameter tbltwttestparameter)
        {
            if (ModelState.IsValid)
            {
                db.tblTWTTestParameters.AddObject(tbltwttestparameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbltwttestparameter);
        }

        //
        // GET: /twtTestParameters/Edit/5

        public ActionResult Edit(int id = 0)
        {
            tblTWTTestParameter tbltwttestparameter = db.tblTWTTestParameters.Single(t => t.id == id);
            if (tbltwttestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestparameter);
        }

        //
        // POST: /twtTestParameters/Edit/5

        [HttpPost]
        public ActionResult Edit(tblTWTTestParameter tbltwttestparameter)
        {
            if (ModelState.IsValid)
            {
                db.tblTWTTestParameters.Attach(tbltwttestparameter);
                db.ObjectStateManager.ChangeObjectState(tbltwttestparameter, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbltwttestparameter);
        }

        //
        // GET: /twtTestParameters/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblTWTTestParameter tbltwttestparameter = db.tblTWTTestParameters.Single(t => t.id == id);
            if (tbltwttestparameter == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestparameter);
        }

        //
        // POST: /twtTestParameters/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblTWTTestParameter tbltwttestparameter = db.tblTWTTestParameters.Single(t => t.id == id);
            db.tblTWTTestParameters.DeleteObject(tbltwttestparameter);
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