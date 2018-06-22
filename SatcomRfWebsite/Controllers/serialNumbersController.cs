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
            Console.WriteLine("just testing");
            return View(tblserialnumber);
        }

        //
        // POST: /serialNumbers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id = 0)
        {
            tblSerialNumber tblserialnumber = db.tblSerialNumbers.Single(t => t.id == id);

            /////////////////////////////////////// Delete TWT Tests
            var deleteSerialsTWT  = from serialNum in db.tblTWTTestResults
                                    where serialNum.ModelSN == tblserialnumber.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsTWT)
            {
                db.tblTWTTestResults.DeleteObject(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete KLY Tests
            var deleteSerialsKLY = from serialNum in db.tblKLYTestResults
                                    where serialNum.ModelSN == tblserialnumber.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsKLY)
            {
                db.tblKLYTestResults.DeleteObject(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete SSPA Tests
            var deleteSerialsSSPA = from serialNum in db.tblSSPATestResults
                                    where serialNum.ModelSN == tblserialnumber.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsSSPA)
            {
                db.tblSSPATestResults.DeleteObject(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete Monitor Data
            var deleteSerialsMD = from serialNum in db.tblMonitorDatas
                                    where serialNum.ModelSN == tblserialnumber.ModelSN
                                    select serialNum;

            foreach (var serial in deleteSerialsMD)
            {
                db.tblMonitorDatas.DeleteObject(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete ATE Output
            var deleteSerialsATE = from serialNum in db.tblATEOutputs
                                  where serialNum.ModelSN == tblserialnumber.ModelSN
                                  select serialNum;

            foreach (var serial in deleteSerialsATE)
            {
                db.tblATEOutputs.DeleteObject(serial);
            }
            db.SaveChanges();

            /////////////////////////////////////// Delete in Serial Numbers table
            var delteFromSerialTable = from serialNum in db.tblSerialNumbers
                                       where serialNum.ModelSN == tblserialnumber.ModelSN
                                       select serialNum;

            foreach (var serial in delteFromSerialTable)
            {
                db.tblSerialNumbers.DeleteObject(serial);
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

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}