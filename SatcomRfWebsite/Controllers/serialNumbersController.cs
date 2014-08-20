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
    public class serialNumbersController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /serialNumbers/

        public ActionResult Index()
        {
            return View(db.tblSerialNumbers.ToList());
        }

        //
        // GET: /serialNumbers/Details/5

        public ActionResult Details(long id = 0)
        {
            tblSerialNumber tblserialnumber = db.tblSerialNumbers.Single(t => t.id == id);
            if (tblserialnumber == null)
            {
                return HttpNotFound();
            }
            return View(tblserialnumber);
        }

        //
        // GET: /serialNumbers/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /serialNumbers/Create

        [HttpPost]
        public ActionResult Create(tblSerialNumber tblserialnumber)
        {
            if (ModelState.IsValid)
            {
                db.tblSerialNumbers.AddObject(tblserialnumber);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblserialnumber);
        }

        //
        // GET: /serialNumbers/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblSerialNumber tblserialnumber = db.tblSerialNumbers.Single(t => t.id == id);
            if (tblserialnumber == null)
            {
                return HttpNotFound();
            }
            return View(tblserialnumber);
        }

        //
        // POST: /serialNumbers/Edit/5

        [HttpPost]
        public ActionResult Edit(tblSerialNumber tblserialnumber)
        {
            if (ModelState.IsValid)
            {
                db.tblSerialNumbers.Attach(tblserialnumber);
                db.ObjectStateManager.ChangeObjectState(tblserialnumber, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblserialnumber);
        }

        //
        // GET: /serialNumbers/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblSerialNumber tblserialnumber = db.tblSerialNumbers.Single(t => t.id == id);
            if (tblserialnumber == null)
            {
                return HttpNotFound();
            }
            return View(tblserialnumber);
        }

        //
        // POST: /serialNumbers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblSerialNumber tblserialnumber = db.tblSerialNumbers.Single(t => t.id == id);
            db.tblSerialNumbers.DeleteObject(tblserialnumber);
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