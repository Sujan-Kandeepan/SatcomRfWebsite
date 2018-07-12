using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SatcomRfWebsite.Controllers
{
    public class CalibrationController : Controller
    {
        // GET: Calibration
        public ActionResult Index()
        {
            return View();
        }

        // GET: Calibration/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Calibration/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Calibration/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Calibration/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Calibration/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Calibration/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Calibration/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
