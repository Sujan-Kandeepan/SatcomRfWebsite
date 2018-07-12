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
            tblModelNames tblmodelnames = db.tblModelNames.Single(t => t.id == id);
            if (tblmodelnames == null)
            {
                return HttpNotFound();
            }
            return View(tblmodelnames);
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
        public ActionResult Create(tblModelNames tblmodelnames)
        {
            if (ModelState.IsValid)
            {
                db.tblModelNames.Add(tblmodelnames);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblmodelnames);
        }

        //
        // GET: /modelName/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblModelNames tblmodelnames = db.tblModelNames.Single(t => t.id == id);
            if (tblmodelnames == null)
            {
                return HttpNotFound();
            }
            return View(tblmodelnames);
        }

        //
        // POST: /modelName/Edit/5

        [HttpPost]
        public ActionResult Edit(tblModelNames tblmodelnames)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblmodelnames).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblmodelnames);
        }

        //
        // GET: /modelName/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblModelNames tblmodelnames = db.tblModelNames.Single(t => t.id == id);
            if (tblmodelnames == null)
            {
                return HttpNotFound();
            }
            return View(tblmodelnames);
        }

        //
        // POST: /modelName/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblModelNames tblmodelnames = db.tblModelNames.Single(t => t.id == id);
            db.tblModelNames.Remove(tblmodelnames);
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