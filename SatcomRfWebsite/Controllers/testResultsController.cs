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
    public class testsDataController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

        //
        // GET: /twtTestResults/

        public ActionResult Index()
        {
            return View(db.tblTWTTestResults.ToList());
        }

        //
        // GET: /twtTestResults/Search/CHPA/VZU-...

        public ActionResult TestResults(string prodT = "", string modelN = "", string filter = "")
        {
            var model = new AteDataTopViewModel(); 

            //var model = new SearchModel();
            var testsModel = new TestResultsModel();

            ViewBag.getResetPath = "testsData/TestResults";
            ViewBag.getProdType = prodT;
            ViewBag.getModName = modelN;
            ViewBag.getFilter = filter;

            //Do this if product type was selected
            if (prodT != "")
            {
                //limit model names related to product type only.
                //model.modelNames(prodT);
                model.searchModel.modelNames(prodT);

                if (modelN != "")
                {
                    model.searchModel.getTubes(modelN);
                    model.searchModel.parseFilter(filter);

                    model.testResModel.generateTestsResults(prodT, modelN, model.searchModel.testType, model.searchModel.tubeName, model.searchModel.options);
                }
            }
            else
            {
                // get all product types and model names
                model.searchModel.allProdTypesModelNames();
            }

            return View(model);
        }

        //
        // GET: /twtTestResults/Details/5

        public ActionResult Details(long id = 0)
        {
            tblTWTTestResult tbltwttestresult = db.tblTWTTestResults.Single(t => t.id == id);
            if (tbltwttestresult == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestresult);
        }

        //
        // GET: /twtTestResults/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /twtTestResults/Create

        [HttpPost]
        public ActionResult Create(tblTWTTestResult tbltwttestresult)
        {
            if (ModelState.IsValid)
            {
                db.tblTWTTestResults.AddObject(tbltwttestresult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbltwttestresult);
        }

        //
        // GET: /twtTestResults/Edit/5

        public ActionResult Edit(long id = 0)
        {
            tblTWTTestResult tbltwttestresult = db.tblTWTTestResults.Single(t => t.id == id);
            if (tbltwttestresult == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestresult);
        }

        //
        // POST: /twtTestResults/Edit/5

        [HttpPost]
        public ActionResult Edit(tblTWTTestResult tbltwttestresult)
        {
            if (ModelState.IsValid)
            {
                db.tblTWTTestResults.Attach(tbltwttestresult);
                db.ObjectStateManager.ChangeObjectState(tbltwttestresult, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbltwttestresult);
        }

        //
        // GET: /twtTestResults/Delete/5

        public ActionResult Delete(long id = 0)
        {
            tblTWTTestResult tbltwttestresult = db.tblTWTTestResults.Single(t => t.id == id);
            if (tbltwttestresult == null)
            {
                return HttpNotFound();
            }
            return View(tbltwttestresult);
        }

        //
        // POST: /twtTestResults/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            tblTWTTestResult tbltwttestresult = db.tblTWTTestResults.Single(t => t.id == id);
            db.tblTWTTestResults.DeleteObject(tbltwttestresult);
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