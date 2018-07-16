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
    public class CalDataController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        // GET: CalData
        public ActionResult Index()
        {
            return View(db.tblCalData.ToList());
        }

        // GET: CalData/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCalData tblCalData = db.tblCalData.Find(id);
            if (tblCalData == null)
            {
                return HttpNotFound();
            }
            return View(tblCalData);
        }

        // GET: CalData/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CalData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,AssetNumber,DeviceType,Frequency,CalFactor,AddedDate")] tblCalData tblCalData)
        {
            if (ModelState.IsValid)
            {
                db.tblCalData.Add(tblCalData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblCalData);
        }

        // GET: CalData/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCalData tblCalData = db.tblCalData.Find(id);
            if (tblCalData == null)
            {
                return HttpNotFound();
            }
            return View(tblCalData);
        }

        // POST: CalData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,AssetNumber,DeviceType,Frequency,CalFactor,AddedDate")] tblCalData tblCalData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCalData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblCalData);
        }

        // GET: CalData/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCalData tblCalData = db.tblCalData.Find(id);
            if (tblCalData == null)
            {
                return HttpNotFound();
            }
            return View(tblCalData);
        }

        // POST: CalData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            tblCalData tblCalData = db.tblCalData.Find(id);
            db.tblCalData.Remove(tblCalData);
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
