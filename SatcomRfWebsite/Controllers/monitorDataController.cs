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
    public class monitorDataController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /monitorData/

        public ActionResult Index()
        {
            return View(db.tblMonitorData.ToList());
        }

        //
        // GET: /monitorData/Details/5

        public ActionResult Details(long id = 0)
        {
            tblMonitorData tblmonitordata = db.tblMonitorData.Single(t => t.id == id);
            if (tblmonitordata == null)
            {
                return HttpNotFound();
            }
            return View(tblmonitordata);
        }

        //
        // GET: /monitorData/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /monitorData/Create

        [HttpPost]
        public ActionResult Create(tblMonitorData tblmonitordata)
        {
            if (ModelState.IsValid)
            {
                db.tblMonitorData.Add(tblmonitordata);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblmonitordata);
        }

        //
        // GET: /monitorData/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblMonitorData tblmonitordata = db.tblMonitorData.Single(t => t.id == id);
            if (tblmonitordata == null)
            {
                return HttpNotFound();
            }
            return View(tblmonitordata);
        }

        //
        // POST: /monitorData/Edit/5

        [HttpPost]
        public ActionResult Edit(tblMonitorData tblmonitordata)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblmonitordata).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblmonitordata);
        }

        //
        // GET: /monitorData/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblMonitorData tblmonitordata = db.tblMonitorData.Single(t => t.id == id);
            if (tblmonitordata == null)
            {
                return HttpNotFound();
            }
            return View(tblmonitordata);
        }

        //
        // POST: /monitorData/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblMonitorData tblmonitordata = db.tblMonitorData.Single(t => t.id == id);
            db.tblMonitorData.Remove(tblmonitordata);
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