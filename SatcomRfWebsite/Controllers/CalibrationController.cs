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
        public ActionResult Create(FormCollection collection)
        {
            foreach (string key in collection.Keys)
            {
                System.Diagnostics.Debug.WriteLine(collection[key], key);
            }

            try
            {
                List<CalibrationRecord> records = new List<CalibrationRecord>();
                for (int i = 0; i < Convert.ToInt32(collection["Points"]); i++)
                {
                    records.Add(new CalibrationRecord
                    {
                        Frequency = Convert.ToDouble(collection[$"Records[{i}].Frequency"]),
                        CalFactor = Convert.ToDouble(collection[$"Records[{i}].CalFactor"])
                    });
                }

                if (Request.Url.ToString().Contains("Attenuator"))
                {
                    CreateAT(new ATCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Environment.UserName.ToUpper(),
                        Records = records,
                        StartFreq = Convert.ToInt64(collection["StartFreq"]),
                        StopFreq = Convert.ToInt64(collection["StopFreq"]),
                        Points = Convert.ToInt32(collection["Points"]),
                        Loss = Convert.ToInt64(collection["Loss"]),
                        Power = Convert.ToInt64(collection["Power"]),
                        MaxOffset = Convert.ToDouble(collection["MaxOffset"]),
                        Temp = collection["Temp"] != "" ? Convert.ToDouble(collection["Temp"]) : (double?)null,
                        Humidity = collection["Humidity"] != "" ? Convert.ToDouble(collection["Humidity"]) : (double?)null,
                        Lookback = collection["Lookback"] != "" ? collection["Lookback"] : null,
                        Operator = collection["Operator"],
                        ExpireDate = Convert.ToDateTime(collection["ExpireDate"])
                    });
                }
                else if (Request.Url.ToString().Contains("OutputCoupler"))
                {
                    CreateOC(new OCCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Environment.UserName.ToUpper(),
                        Records = records,
                        StartFreq = Convert.ToInt64(collection["StartFreq"]),
                        StopFreq = Convert.ToInt64(collection["StopFreq"]),
                        Points = Convert.ToInt32(collection["Points"]),
                        Loss = Convert.ToInt64(collection["Loss"]),
                        Power = Convert.ToInt64(collection["Power"]),
                        MaxOffset = Convert.ToDouble(collection["MaxOffset"]),
                        Temp = collection["Temp"] != "" ? Convert.ToDouble(collection["Temp"]) : (double?)null,
                        Humidity = collection["Humidity"] != "" ? Convert.ToDouble(collection["Humidity"]) : (double?)null,
                        Lookback = collection["Lookback"] != "" ? collection["Lookback"] : null,
                        Operator = collection["Operator"],
                        ExpireDate = Convert.ToDateTime(collection["ExpireDate"])
                    });
                }
                else if (Request.Url.ToString().Contains("PowerSensor"))
                {
                    CreatePS(new PSCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Environment.UserName.ToUpper(),
                        Records = records,
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
        public ActionResult CreateAT(ATCalibrationData atData)
        {
            if (ModelState.IsValid)
            {
                tblATCalHeaders atHeaders = new tblATCalHeaders
                {
                    AssetNumber = atData.AssetNumber,
                    StartFreq = atData.StartFreq,
                    StopFreq = atData.StopFreq,
                    Points = atData.Points,
                    Loss = atData.Loss,
                    Power = atData.Power,
                    MaxOffset = atData.MaxOffset,
                    Temp = atData.Temp,
                    Humidity = atData.Humidity,
                    Lookback = atData.Lookback,
                    Operator = atData.Operator,
                    ExpireDate = atData.ExpireDate,
                    AddedDate = atData.AddedDate,
                    EditedBy = atData.EditedBy
                };
                db.tblATCalHeaders.Add(atHeaders);
                

                foreach (CalibrationRecord record in atData.Records)
                {
                    tblCalData calData = new tblCalData
                    {
                        AssetNumber = atData.AssetNumber,
                        DeviceType = "Attenuator",
                        Frequency = record.Frequency,
                        CalFactor = record.CalFactor,
                        AddedDate = atData.AddedDate
                    };
                    db.tblCalData.Add(calData);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(atData);
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
                        DeviceType = "Output Coupler",
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
                        DeviceType = "Power Sensor",
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

       public ActionResult CreateDataFields(int num)
        {
            var html = "";
            for(var i = 0; i < num; i++)
            {
                html += "<div class='row' style='margin-bottom: 15px'>";
                html += $"<div class='col-lg-6'><input class='form-control text-box single-line' data-val='true' data-val-number='The field Frequency must be a number.' data-val-required='The Frequency field is required.' id='Records_{i}__Frequency' name='Records[{i}].Frequency' placeholder='Frequency' type='text' value=''></div>";
                html += $"<div class='col-lg-6'><input class='form-control text-box single-line' data-val='true' data-val-number='The field CalFactor must be a number.' data-val-required='The CalFactor field is required.' id='Records_{i}__CalFactor' name='Records[{i}].CalFactor' placeholder='Calibration Factor' type='text' value=''></div>";
                html += "</div>";
            }
            return Content(html);
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
