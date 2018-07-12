using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SatcomRfWebsite.Models;

namespace SatcomRfWebsite.Controllers
{
    public class PSCalHeadersController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        // GET: PSCalHeaders
        public ActionResult Index()
        {
            return View(db.tblPSCalHeaders.ToList());
        }

        // GET: PSCalHeaders/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPSCalHeaders tblPSCalHeaders = db.tblPSCalHeaders.Find(id);
            if (tblPSCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblPSCalHeaders);
        }

        // GET: PSCalHeaders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PSCalHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,AssetNumber,Series,Serial,RefCal,Certificate,Operator,CalDate,AddedDate,EditedBy")] tblPSCalHeaders tblPSCalHeaders)
        {
            if (ModelState.IsValid)
            {
                db.tblPSCalHeaders.Add(tblPSCalHeaders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblPSCalHeaders);
        }

        // GET: PSCalHeaders/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPSCalHeaders tblPSCalHeaders = db.tblPSCalHeaders.Find(id);
            if (tblPSCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblPSCalHeaders);
        }

        // POST: PSCalHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,AssetNumber,Series,Serial,RefCal,Certificate,Operator,CalDate,AddedDate,EditedBy")] tblPSCalHeaders tblPSCalHeaders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblPSCalHeaders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblPSCalHeaders);
        }

        // GET: PSCalHeaders/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPSCalHeaders tblPSCalHeaders = db.tblPSCalHeaders.Find(id);
            if (tblPSCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblPSCalHeaders);
        }

        // POST: PSCalHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            tblPSCalHeaders tblPSCalHeaders = db.tblPSCalHeaders.Find(id);
            db.tblPSCalHeaders.Remove(tblPSCalHeaders);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
