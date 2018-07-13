using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SatcomRfWebsite.Models;

namespace SatcomRfWebsite.Controllers
{
    public class CalibrationController : Controller
    {
        private rfDbEntities db = new rfDbEntities();

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
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            foreach (string key in collection.Keys)
            {
                System.Diagnostics.Debug.WriteLine(collection[key], key);
            }

            try
            {
                // TODO: Add insert logic here
                if (Request.Url.ToString().Contains("Attenuator"))
                {
                    CreateAT(new ATCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = Convert.ToDateTime(collection["AddedDate"]),
                        EditedBy = collection["EditedBy"],
                        Records = new List<CalibrationRecord>()
                        {
                            new CalibrationRecord { Frequency = 1, CalFactor = 2 },
                            new CalibrationRecord { Frequency = 3, CalFactor = 4 },
                            new CalibrationRecord { Frequency = 5, CalFactor = 6 }
                        },
                        StartFreq = Convert.ToInt64(collection["StartFreq"]),
                        StopFreq = Convert.ToInt64(collection["StopFreq"]),
                        Points = Convert.ToInt32(collection["StopFreq"]),
                        Loss = Convert.ToInt64(collection["Loss"]),
                        Power = Convert.ToInt64(collection["Power"]),
                        MaxOffset = Convert.ToDouble(collection["MaxOffset"]),
                        Temp = Convert.ToDouble(collection["Temp"]),
                        Humidity = Convert.ToDouble(collection["Humidity"]),
                        Lookback = collection["Lookback"],
                        Operator = collection["Operator"],
                        ExpireDate = Convert.ToDateTime(collection["ExpireDate"])
                    });
                }
                else if (Request.Url.ToString().Contains("OutputCoupler"))
                {
                    CreateOC(new OCCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = Convert.ToDateTime(collection["AddedDate"]),
                        EditedBy = collection["EditedBy"],
                        Records = new List<CalibrationRecord>()
                        {
                            new CalibrationRecord { Frequency = 1, CalFactor = 2 },
                            new CalibrationRecord { Frequency = 3, CalFactor = 4 },
                            new CalibrationRecord { Frequency = 5, CalFactor = 6 }
                        },
                        StartFreq = Convert.ToInt64(collection["StartFreq"]),
                        StopFreq = Convert.ToInt64(collection["StopFreq"]),
                        Points = Convert.ToInt32(collection["StopFreq"]),
                        Loss = Convert.ToInt64(collection["Loss"]),
                        Power = Convert.ToInt64(collection["Power"]),
                        MaxOffset = Convert.ToDouble(collection["MaxOffset"]),
                        Temp = Convert.ToDouble(collection["Temp"]),
                        Humidity = Convert.ToDouble(collection["Humidity"]),
                        Lookback = collection["Lookback"],
                        Operator = collection["Operator"],
                        ExpireDate = Convert.ToDateTime(collection["ExpireDate"])
                    });
                }
                else if (Request.Url.ToString().Contains("PowerSensor"))
                {
                    CreatePS(new PSCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = Convert.ToDateTime(collection["AddedDate"]),
                        EditedBy = collection["EditedBy"],
                        Records = new List<CalibrationRecord>()
                        {
                            new CalibrationRecord { Frequency = 1, CalFactor = 2 },
                            new CalibrationRecord { Frequency = 3, CalFactor = 4 },
                            new CalibrationRecord { Frequency = 5, CalFactor = 6 }
                        },
                        Series = collection["Series"],
                        Serial = collection["Serial"],
                        RefCal = collection["RefCal"],
                        Certificate = collection["Certificate"],
                        Operator = collection["Operator"],
                        CalDate = Convert.ToDateTime(collection["CalDate"])
                    });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateAT(ATCalibrationData atCalibrationData)
        {
            if (ModelState.IsValid)
            {
                tblATCalHeaders atCalHeaders = new tblATCalHeaders
                {
                    AssetNumber = atCalibrationData.AssetNumber,
                    StartFreq = atCalibrationData.StartFreq,
                    StopFreq = atCalibrationData.StopFreq,
                    Points = atCalibrationData.Points,
                    Loss = atCalibrationData.Loss,
                    Power = atCalibrationData.Power,
                    MaxOffset = atCalibrationData.MaxOffset,
                    Temp = atCalibrationData.Temp,
                    Humidity = atCalibrationData.Humidity,
                    Lookback = atCalibrationData.Lookback,
                    Operator = atCalibrationData.Operator,
                    ExpireDate = atCalibrationData.ExpireDate,
                    AddedDate = atCalibrationData.AddedDate,
                    EditedBy = atCalibrationData.EditedBy
                };
                db.tblATCalHeaders.Add(atCalHeaders);
                

                foreach (CalibrationRecord record in atCalibrationData.Records)
                {
                    tblCalData calData = new tblCalData
                    {
                        AssetNumber = atCalibrationData.AssetNumber,
                        Frequency = record.Frequency,
                        CalFactor = record.CalFactor,
                        AddedDate = atCalibrationData.AddedDate
                    };
                    db.tblCalData.Add(calData);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(atCalibrationData);
        }

        [HttpPost]
        public ActionResult CreateOC(OCCalibrationData ocData)
        {
            if (ModelState.IsValid)
            {
                tblOCCalHeaders ocHeaders = new tblOCCalHeaders
                {
                    AssetNumber = ocData.AssetNumber,
                    StartFreq = ocData.StartFreq,
                    StopFreq = ocData.StopFreq,
                    Points = ocData.Points,
                    Loss = ocData.Loss,
                    Power = ocData.Power,
                    MaxOffset = ocData.MaxOffset,
                    Temp = ocData.Temp,
                    Humidity = ocData.Humidity,
                    Lookback = ocData.Lookback,
                    Operator = ocData.Operator,
                    ExpireDate = ocData.ExpireDate,
                    AddedDate = ocData.AddedDate,
                    EditedBy = ocData.EditedBy
                };
                db.tblOCCalHeaders.Add(ocHeaders);


                foreach (CalibrationRecord record in ocData.Records)
                {
                    tblCalData calData = new tblCalData
                    {
                        AssetNumber = ocData.AssetNumber,
                        Frequency = record.Frequency,
                        CalFactor = record.CalFactor,
                        AddedDate = ocData.AddedDate
                    };
                    db.tblCalData.Add(calData);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(ocData);
            
        }

        [HttpPost]
        public ActionResult CreatePS(PSCalibrationData psData)
        {
            if (ModelState.IsValid)
            {
                tblPSCalHeaders psHeaders = new tblPSCalHeaders
                {
                    AssetNumber = psData.AssetNumber,
                    Series = psData.Series,
                    Serial = psData.Serial,
                    RefCal = psData.RefCal,
                    Certificate = psData.Certificate,
                    Operator = psData.Operator,
                    CalDate = psData.CalDate,
                    AddedDate = psData.AddedDate,
                    EditedBy = psData.EditedBy

                };
                db.tblPSCalHeaders.Add(psHeaders);


                foreach (CalibrationRecord record in psData.Records)
                {
                    tblCalData calData = new tblCalData
                    {
                        AssetNumber = psData.AssetNumber,
                        Frequency = record.Frequency,
                        CalFactor = record.CalFactor,
                        AddedDate = psData.AddedDate
                    };
                    db.tblCalData.Add(calData);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(psData);
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
