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

        public ActionResult Index(string beginning = "", string findby = "serial")
        {
            if (beginning.Equals(""))
            {
                return View(db.tblSerialNumbers.OrderBy(x => x.ModelSN).ToList());
            }
            else if (findby.Equals("name"))
            {
                return View(db.tblSerialNumbers.Where(x => x.ModelName.StartsWith(beginning)).OrderBy(x => x.ModelName).ToList());
            }
            else
            {
                return View(db.tblSerialNumbers.Where(x => x.ModelSN.StartsWith(beginning)).OrderBy(x => x.ModelSN).ToList());
            }
        }

        //
        // GET: /serialNumbers/Details/5

        public ActionResult Details(long id = 0)
        {
            tblSerialNumbers tblserialnumbers = db.tblSerialNumbers.Single(t => t.id == id);
            if (tblserialnumbers == null)
            {
                return HttpNotFound();
            }
            return View(tblserialnumbers);
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
        public ActionResult Create(tblSerialNumbers tblserialnumbers)
        {
            if (ModelState.IsValid)
            {
                db.tblSerialNumbers.Add(tblserialnumbers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblserialnumbers);
        }

        //
        // GET: /serialNumbers/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblSerialNumbers tblserialnumbers = db.tblSerialNumbers.Single(t => t.id == id);
            if (tblserialnumbers == null)
            {
                return HttpNotFound();
            }
            return View(tblserialnumbers);
        }

        //
        // POST: /serialNumbers/Edit/5

        [HttpPost]
        public ActionResult Edit(tblSerialNumbers tblserialnumbers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblserialnumbers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblserialnumbers);
        }

        //
        // GET: /serialNumbers/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblSerialNumbers tblserialnumbers = db.tblSerialNumbers.Single(t => t.id == id);
            if (tblserialnumbers == null)
            {
                return HttpNotFound();
            }
            Console.WriteLine("just testing");
            return View(tblserialnumbers);
        }

        //
        // POST: /serialNumbers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id = 0)
        {
            tblSerialNumbers tblserialnumbers = db.tblSerialNumbers.Single(t => t.id == id);

            /////////////////////////////////////// Delete TWT Tests
            var deleteSerialsTWT  = from serialNum in db.tblTWTTestResults
                                    where serialNum.ModelSN == tblserialnumbers.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsTWT)
            {
                db.tblTWTTestResults.Remove(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete KLY Tests
            var deleteSerialsKLY = from serialNum in db.tblKLYTestResults
                                    where serialNum.ModelSN == tblserialnumbers.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsKLY)
            {
                db.tblKLYTestResults.Remove(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete SSPA Tests
            var deleteSerialsSSPA = from serialNum in db.tblSSPATestResults
                                    where serialNum.ModelSN == tblserialnumbers.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsSSPA)
            {
                db.tblSSPATestResults.Remove(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete Monitor Data
            var deleteSerialsMD = from serialNum in db.tblMonitorData
                                    where serialNum.ModelSN == tblserialnumbers.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsMD)
            {
                db.tblMonitorData.Remove(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete ATE Output
            var deleteSerialsATE = from serialNum in db.tblATEOutput
                                  where serialNum.ModelSN == tblserialnumbers.ModelSN
                                  select serialNum;

            foreach (var serial in deleteSerialsATE)
            {
                db.tblATEOutput.Remove(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete in Serial Numbers table
            var delteFromSerialTable = from serialNum in db.tblSerialNumbers
                                       where serialNum.ModelSN == tblserialnumbers.ModelSN
                                       select serialNum;

            foreach (var serial in delteFromSerialTable)
            {
                db.tblSerialNumbers.Remove(serial);
            }
            db.SaveChanges();

            /*try
            {
                db.SaveChanges();
            }
            catch( Exception e )
            {
                Console.WriteLine(e);
            }*/

            return Redirect("/serialnumbers?beginning=A&findby=serial");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}