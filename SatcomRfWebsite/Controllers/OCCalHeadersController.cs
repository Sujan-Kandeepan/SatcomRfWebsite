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
    public class OCCalHeadersController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        // GET: OCCalHeaders
        public ActionResult Index()
        {
            return View(db.tblOCCalHeaders.ToList());
        }

        // GET: OCCalHeaders/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOCCalHeaders tblOCCalHeaders = db.tblOCCalHeaders.Find(id);
            if (tblOCCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblOCCalHeaders);
        }

        // GET: OCCalHeaders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OCCalHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,AssetNumber,StartFreq,StopFreq,Points,Loss,Power,MaxOffset,Temp,Humidity,Lookback,Operator,ExpireDate,AddedDate,EditedBy")] tblOCCalHeaders tblOCCalHeaders)
        {
            if (ModelState.IsValid)
            {
                db.tblOCCalHeaders.Add(tblOCCalHeaders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblOCCalHeaders);
        }

        // GET: OCCalHeaders/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOCCalHeaders tblOCCalHeaders = db.tblOCCalHeaders.Find(id);
            if (tblOCCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblOCCalHeaders);
        }

        // POST: OCCalHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,AssetNumber,StartFreq,StopFreq,Points,Loss,Power,MaxOffset,Temp,Humidity,Lookback,Operator,ExpireDate,AddedDate,EditedBy")] tblOCCalHeaders tblOCCalHeaders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblOCCalHeaders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblOCCalHeaders);
        }

        // GET: OCCalHeaders/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOCCalHeaders tblOCCalHeaders = db.tblOCCalHeaders.Find(id);
            if (tblOCCalHeaders == null)
            {
                return HttpNotFound();
            }
            return View(tblOCCalHeaders);
        }

        // POST: OCCalHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            tblOCCalHeaders tblOCCalHeaders = db.tblOCCalHeaders.Find(id);
            db.tblOCCalHeaders.Remove(tblOCCalHeaders);
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
