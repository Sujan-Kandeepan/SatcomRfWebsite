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
            tblTWTTestParameters tbltwttestparameters = db.tblTWTTestParameters.Single(t => t.id == id);
            if (tbltwttestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestparameters);
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
        public ActionResult Create(tblTWTTestParameters tbltwttestparameters)
        {
            if (ModelState.IsValid)
            {
                db.tblTWTTestParameters.Add(tbltwttestparameters);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbltwttestparameters);
        }

        //
        // GET: /twtTestParameters/Edit/5

        public ActionResult Edit(int id = 0)
        {
            tblTWTTestParameters tbltwttestparameters = db.tblTWTTestParameters.Single(t => t.id == id);
            if (tbltwttestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestparameters);
        }

        //
        // POST: /twtTestParameters/Edit/5

        [HttpPost]
        public ActionResult Edit(tblTWTTestParameters tbltwttestparameters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbltwttestparameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbltwttestparameters);
        }

        //
        // GET: /twtTestParameters/Delete/5

        public ActionResult Delete(int id = 0)
        {
            tblTWTTestParameters tbltwttestparameters = db.tblTWTTestParameters.Single(t => t.id == id);
            if (tbltwttestparameters == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestparameters);
        }

        //
        // POST: /twtTestParameters/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblTWTTestParameters tbltwttestparameters = db.tblTWTTestParameters.Single(t => t.id == id);
            db.tblTWTTestParameters.Remove(tbltwttestparameters);
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