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
    public class modelNameController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /modelName/

        public ActionResult Index()
        {
            return View(db.tblModelNames.ToList());
        }

        //
        // GET: /modelName/Details/5

        public ActionResult Details(long id = 0)
        {
            tblModelName tblmodelname = db.tblModelNames.Single(t => t.id == id);
            if (tblmodelname == null)
            {
                return HttpNotFound();
            }
            return View(tblmodelname);
        }

        //
        // GET: /modelName/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /modelName/Create

        [HttpPost]
        public ActionResult Create(tblModelName tblmodelname)
        {
            if (ModelState.IsValid)
            {
                db.tblModelNames.AddObject(tblmodelname);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblmodelname);
        }

        //
        // GET: /modelName/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblModelName tblmodelname = db.tblModelNames.Single(t => t.id == id);
            if (tblmodelname == null)
            {
                return HttpNotFound();
            }
            return View(tblmodelname);
        }

        //
        // POST: /modelName/Edit/5

        [HttpPost]
        public ActionResult Edit(tblModelName tblmodelname)
        {
            if (ModelState.IsValid)
            {
                db.tblModelNames.Attach(tblmodelname);
                db.ObjectStateManager.ChangeObjectState(tblmodelname, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblmodelname);
        }

        //
        // GET: /modelName/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblModelName tblmodelname = db.tblModelNames.Single(t => t.id == id);
            if (tblmodelname == null)
            {
                return HttpNotFound();
            }
            return View(tblmodelname);
        }

        //
        // POST: /modelName/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblModelName tblmodelname = db.tblModelNames.Single(t => t.id == id);
            db.tblModelNames.DeleteObject(tblmodelname);
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