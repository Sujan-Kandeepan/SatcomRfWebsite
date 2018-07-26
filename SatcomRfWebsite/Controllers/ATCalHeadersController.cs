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
    public class ATCalHeadersController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        // GET: ATCalHeaders
        public ActionResult Index()
        {
            return View(db.tblATCalHeaders.ToList());
        }

        // GET: ATCalHeaders/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblATCalHeaders tblATCalHeaders = db.tblATCalHeaders.Find(id);
            if (tblATCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblATCalHeaders);
        }

        // GET: ATCalHeaders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ATCalHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,AssetNumber,StartFreq,StopFreq,Points,Loss,Power,MaxOffset,Temp,Humidity,Lookback,Operator,CalDate,ExpireDate,AddedDate,EditedBy")] tblATCalHeaders tblATCalHeaders)
        {
            if (ModelState.IsValid)
            {
                db.tblATCalHeaders.Add(tblATCalHeaders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblATCalHeaders);
        }

        // GET: ATCalHeaders/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblATCalHeaders tblATCalHeaders = db.tblATCalHeaders.Find(id);
            if (tblATCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblATCalHeaders);
        }

        // POST: ATCalHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,AssetNumber,StartFreq,StopFreq,Points,Loss,Power,MaxOffset,Temp,Humidity,Lookback,Operator,CalDate,ExpireDate,AddedDate,EditedBy")] tblATCalHeaders tblATCalHeaders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblATCalHeaders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblATCalHeaders);
        }

        // GET: ATCalHeaders/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblATCalHeaders tblATCalHeaders = db.tblATCalHeaders.Find(id);
            if (tblATCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblATCalHeaders);
        }

        // POST: ATCalHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            tblATCalHeaders tblATCalHeaders = db.tblATCalHeaders.Find(id);
            db.tblATCalHeaders.Remove(tblATCalHeaders);
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
