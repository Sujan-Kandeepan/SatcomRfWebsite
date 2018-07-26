﻿using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

        public JsonResult GetAssetNumbers(string type)
        {
            if (type.Equals("Attenuator"))
            {
                return Json((from val in db.tblATCalHeaders.OrderBy(header => header.AssetNumber) select val.AssetNumber).Distinct().ToList());
            }
            else if (type.Equals("Output Coupler"))
            {
                return Json((from val in db.tblOCCalHeaders.OrderBy(header => header.AssetNumber) select val.AssetNumber).Distinct().ToList());
            }
            else if (type.Equals("Power Sensor"))
            {
                return Json((from val in db.tblPSCalHeaders.OrderBy(header => header.AssetNumber) select val.AssetNumber).Distinct().ToList());
            }

            return Json(new List<string>());
        }

        public ActionResult GetData(string type, string assetNumber)
        {
            List<DateTime> dates;
            if (type.Equals("Attenuator"))
            {
                dates = (from val in db.tblATCalHeaders where val.AssetNumber.Equals(assetNumber)
                         orderby val.CalDate select val.CalDate).Distinct().ToList(); 
            }
            else if (type.Equals("Output Coupler"))
            {
                dates = (from val in db.tblOCCalHeaders where val.AssetNumber.Equals(assetNumber)
                         orderby val.CalDate select val.CalDate).Distinct().ToList();
            }
            else if (type.Equals("Power Sensor"))
            {
                dates = (from val in db.tblPSCalHeaders where val.AssetNumber.Equals(assetNumber)
                         orderby val.CalDate select val.CalDate).Distinct().ToList();
            }
            else
            {
                return Json(new object());
            }

            dates.Reverse();

            List<double> freqs = (from val in db.tblCalData where val.AssetNumber.Equals(assetNumber)
                                  orderby val.Frequency select val.Frequency).Distinct().ToList();

            List<List<string>> calFactor = new List<List<string>>();
            List<List<string>> returnLoss = new List<List<string>>();
            foreach (var date in dates)
            {
                List<string> calFactorSublist = new List<string>();
                List<string> returnLossSublist = new List<string>();
                foreach (var freq in freqs)
                {
                    var calFactorSingle = (from val in db.tblCalData
                                      where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(date) && val.Frequency.Equals(freq)
                                      select val.CalFactor).ToList();

                    var returnLossSingle = (from val in db.tblCalData
                                           where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(date) && val.Frequency.Equals(freq)
                                           select val.ReturnLoss).ToList();

                    calFactorSublist.Add(calFactorSingle.Count() > 0 ? Math.Round(calFactorSingle[0], 3).ToString() : "---");
                    returnLossSublist.Add(returnLossSingle.Count() > 0 && returnLossSingle[0].HasValue ? Math.Round(returnLossSingle[0].Value, 3).ToString() : "---");
                }
                calFactor.Add(calFactorSublist);
                returnLoss.Add(returnLossSublist);
            }

            return Content(JsonConvert.SerializeObject(new {
                dates = from date in dates select date.ToString("MM/dd/yyyy"), freqs, calFactor, returnLoss
            }), "application/json");
        }

        // GET: Calibration/Details/5
        public ActionResult Details()
        {
            return View();
        }

        public ActionResult GetDetails(string type, string assetnum, string date)
        {
            var assetNumber = assetnum.Replace("_", " ");
            var datePieces = date.Split('/');
            var calDate = new DateTime(Convert.ToInt32(datePieces[2]), Convert.ToInt32(datePieces[0]), Convert.ToInt32(datePieces[1]));

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.DateFormatString = "MM/dd/yyyy";

            string headers = JsonConvert.SerializeObject(new object());

            if (type.Equals("Attenuator"))
            {
                var id = (from val in db.tblATCalHeaders
                          where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                          select val.id ).ToList();

                tblATCalHeaders data = db.tblATCalHeaders.Find(id[0]);
                headers = JsonConvert.SerializeObject(data, jsonSettings);
            }
            else if (type.Equals("OutputCoupler"))
            {
                var id = (from val in db.tblOCCalHeaders
                          where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                          select val.id).ToList();

                tblOCCalHeaders data = db.tblOCCalHeaders.Find(id[0]);
                headers = JsonConvert.SerializeObject(data, jsonSettings);
            }
            else if (type.Equals("PowerSensor"))
            {
                var id = (from val in db.tblPSCalHeaders
                          where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                          select val.id).ToList();

                tblPSCalHeaders data = db.tblPSCalHeaders.Find(id[0]);
                headers = JsonConvert.SerializeObject(data, jsonSettings);
            }

            List<double> freqs = (from val in db.tblCalData
                                  where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                                  orderby val.Frequency
                                  select val.Frequency).Distinct().ToList();

            List<string> calFactor = new List<string>();
            List<string> returnLoss = new List<string>();
            foreach (var freq in freqs)
            {
                var calFactorSingle = (from val in db.tblCalData
                                       where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate) && val.Frequency.Equals(freq)
                                       select val.CalFactor).ToList();

                var returnLossSingle = (from val in db.tblCalData
                                        where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate) && val.Frequency.Equals(freq)
                                        select val.ReturnLoss).ToList();

                calFactor.Add(calFactorSingle.Count() > 0 ? Math.Round(calFactorSingle[0], 3).ToString() : "---");
                returnLoss.Add(returnLossSingle.Count() > 0 && returnLossSingle[0].HasValue ? Math.Round(returnLossSingle[0].Value, 3).ToString() : "---");
            }

            return Content(JsonConvert.SerializeObject(new { headers, freqs, calFactor, returnLoss }), "application/json");
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
                        CalFactor = Convert.ToDouble(collection[$"Records[{i}].CalFactor"]),
                        ReturnLoss = !Request.Url.ToString().Contains("PowerSensor") && collection[$"Records[{i}].ReturnLoss"] != "" ? Convert.ToDouble(collection[$"Records[{i}].ReturnLoss"]) : (double?)null
                    });
                }

                if (Request.Url.ToString().Contains("Attenuator"))
                {
                    return CreateAT(new ATCalibrationData
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
                        CalDate = Convert.ToDateTime(collection["CalDate"]),
                        ExpireDate = Convert.ToDateTime(collection["ExpireDate"])
                    });
                }
                else if (Request.Url.ToString().Contains("OutputCoupler"))
                {
                    return CreateOC(new OCCalibrationData
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
                        CalDate = Convert.ToDateTime(collection["CalDate"]),
                        ExpireDate = Convert.ToDateTime(collection["ExpireDate"])
                    });
                }
                else if (Request.Url.ToString().Contains("PowerSensor"))
                {
                    return CreatePS(new PSCalibrationData
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
                    CalDate = atData.CalDate,
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
                        ReturnLoss = record.ReturnLoss,
                        CalDate = atData.CalDate
                    };
                    db.tblCalData.Add(calData);
                }

                db.SaveChanges();

                return Redirect($"~/Calibration/Index/Attenuator?assetnum={atData.AssetNumber.Replace(" ", "_")}");
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
                    CalDate = ocData.CalDate,
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
                        ReturnLoss = record.ReturnLoss,
                        CalDate = ocData.CalDate
                    };
                    db.tblCalData.Add(calData);
                }

                db.SaveChanges();

                return Redirect($"~/Calibration/Index/OutputCoupler?assetnum={ocData.AssetNumber.Replace(" ", "_")}");
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
                        ReturnLoss = record.ReturnLoss,
                        CalDate = psData.CalDate
                    };
                    db.tblCalData.Add(calData);
                }

                db.SaveChanges();

                return Redirect($"~/Calibration/Index/PowerSensor?assetnum={psData.AssetNumber.Replace(" ", "_")}");
            }

            return View(psData);
        }

        public ActionResult CreateDataFields(int num, bool returnloss)
        {
            var html = num == 0 ? "<span style='color: grey'>Form fields to enter calfactor records will appear here</span>" : "";
            for (var i = 0; i < num; i++)
            {
                html += $"<div class='form-group row' style='margin-bottom: 5px' align='left'>";
                html += "<div class='col-md-" + (returnloss ? "4" : "6") + $"'><label class='control-label' for='Records_{i}__Frequency'>Frequency &middot;  {i + 1}</label><input class='form-control text-box single-line' data-val='true' data-val-number='The field Frequency must be a number.' data-val-required='The Frequency field is required.' id='Records_{i}__Frequency' name='Records[{i}].Frequency' type='number' min='0' value=''></div>";
                html += "<div class='col-md-" + (returnloss ? "4" : "6") + $"'><label class='control-label' for='Records_{i}__CalFactor'>Calibration Factor &middot; {i + 1}</label><input class='form-control text-box single-line' data-val='true' data-val-number='The field CalFactor must be a number.' data-val-required='The CalFactor field is required.' id='Records_{i}__CalFactor' name='Records[{i}].CalFactor' type='number' min='0' value=''></div>";
                if (returnloss) html += $"<div class='col-md-4'><label class='control-label' for='Records_{i}__ReturnLoss'>Return Loss &middot; {i + 1}</label><input class='form-control text-box single-line' data-val='true' data-val-number='The field ReturnLoss must be a number.' data-val-required='The ReturnLoss field is required.' id='Records_{i}__CalFactor' name='Records[{i}].ReturnLoss' placeholder='Optional' type='number' min='0' value=''></div>";
                html += "</div>";
            }
            return Content(html);
        }

        [HttpPost]
        public JsonResult ImportFile(string type)
        {
            try
            {
                if (Request.Files.Count == 0) return Json("No file found");
                var fileContent = Request.Files[0];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;
                    var fileName = Request.Files[0].FileName;
                    var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }

                    DateTime startTime = DateTime.Now;

                    Application app = new Application();
                    Workbook wb = app.Workbooks.Open(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    Worksheet sheet = (Worksheet)wb.Sheets[1];
                    for (int i = 2; i <= wb.Sheets.Count; i++)
                    {
                        if (wb.Sheets[i].UsedRange.Rows.Count < sheet.UsedRange.Rows.Count && wb.Sheets[i].UsedRange.Rows.Count > 1)
                        {
                            sheet = wb.Sheets[i];
                        }
                    }
                    Range range = sheet.UsedRange;

                    try
                    {
                        if (type.Equals("Attenuator"))
                        {
                            List<CalibrationRecord> records = new List<CalibrationRecord>();
                            for (int i = 9; i <= range.Rows.Count; i++)
                            {
                                records.Add(new CalibrationRecord
                                {
                                    Frequency = Convert.ToDouble(range.Cells[i, 1].Text),
                                    CalFactor = Convert.ToDouble(range.Cells[i, 2].Text)
                                });
                            }

                            string dateString = range.Cells[8, 2].Value.ToString();
                            string[] pieces = dateString.Split(' ')[0].Split('/');
                            DateTime date = new DateTime(Convert.ToInt32(pieces[2]), Convert.ToInt32(pieces[0]), Convert.ToInt32(pieces[1]));

                            ATCalibrationData data = new ATCalibrationData
                            {
                                AssetNumber = range.Cells[1, 2].Text,
                                Records = records,
                                StartFreq = Convert.ToInt64(range.Cells[3, 2].Text),
                                StopFreq = Convert.ToInt64(range.Cells[4, 2].Text),
                                Points = Convert.ToInt32(range.Cells[5, 2].Text),
                                Loss = Convert.ToInt64(range.Cells[3, 4].Text),
                                Power = Convert.ToInt64(range.Cells[4, 4].Text),
                                MaxOffset = Convert.ToDouble(range.Cells[5, 4].Text),
                                Temp = Convert.ToDouble(range.Cells[3, 6].Text),
                                Humidity = Convert.ToDouble(range.Cells[4, 6].Text),
                                Lookback = range.Cells[5, 6].Text,
                                Operator = range.Cells[7, 2].Text,
                                CalDate = DateTime.Now.Date,
                                AddedDate = DateTime.Now.Date,
                                ExpireDate = date
                            };
                            wb.Close(false, path, false);
                            return Json(JsonConvert.SerializeObject(data));
                        }
                        else if (type.Equals("OutputCoupler"))
                        {
                            List<CalibrationRecord> records = new List<CalibrationRecord>();
                            for (int i = 9; i <= range.Rows.Count; i++)
                            {
                                records.Add(new CalibrationRecord
                                {
                                    Frequency = Convert.ToDouble(range.Cells[i, 1].Text),
                                    CalFactor = Convert.ToDouble(range.Cells[i, 2].Text)
                                });
                            }

                            string datestring = range.Cells[8, 2].Value.ToString();
                            string[] pieces = datestring.Split(' ')[0].Split('/');
                            DateTime date = new DateTime(Convert.ToInt32(pieces[2]), Convert.ToInt32(pieces[0]), Convert.ToInt32(pieces[1]));

                            OCCalibrationData data = new OCCalibrationData
                            {
                                AssetNumber = range.Cells[1, 2].Text,
                                Records = records,
                                StartFreq = Convert.ToInt64(range.Cells[3, 2].Text),
                                StopFreq = Convert.ToInt64(range.Cells[4, 2].Text),
                                Points = Convert.ToInt32(range.Cells[5, 2].Text),
                                Loss = Convert.ToInt64(range.Cells[3, 4].Text),
                                Power = Convert.ToInt64(range.Cells[4, 4].Text),
                                MaxOffset = Convert.ToDouble(range.Cells[5, 4].Text),
                                Temp = Convert.ToDouble(range.Cells[3, 6].Text),
                                Humidity = Convert.ToDouble(range.Cells[4, 6].Text),
                                Lookback = range.Cells[5, 6].Text,
                                Operator = range.Cells[7, 2].Text,
                                CalDate = DateTime.Now.Date,
                                AddedDate = DateTime.Now.Date,
                                ExpireDate = date
                            };
                            wb.Close(false, path, false);
                            return Json(JsonConvert.SerializeObject(data));
                        }
                        else if (type.Equals("PowerSensor"))
                        {
                            List<CalibrationRecord> records = new List<CalibrationRecord>();
                            for (int i = 12; i <= range.Rows.Count; i++)
                            {
                                records.Add(new CalibrationRecord
                                {
                                    Frequency = Convert.ToDouble(range.Cells[i, 1].Value),
                                    CalFactor = Convert.ToDouble(range.Cells[i, 4].Value)
                                });
                            }

                            string datestring = range.Cells[1, 7].Value.ToString();
                            string[] pieces = datestring.Split(' ')[0].Split('/');
                            DateTime date = new DateTime(Convert.ToInt32(pieces[2]), Convert.ToInt32(pieces[0]), Convert.ToInt32(pieces[1]));

                            PSCalibrationData data = new PSCalibrationData
                            {
                                AssetNumber = range.Cells[7, 5].Text,
                                Records = records,
                                Series = range.Cells[6, 1].Text,
                                Serial = range.Cells[7, 3].Text,
                                RefCal = range.Cells[7, 6].Text,
                                Certificate = range.Cells[4, 7].Text,
                                Operator = range.Cells[7, 8].Text,
                                CalDate = date
                            };
                            wb.Close(false, path, false);
                            return Json(JsonConvert.SerializeObject(data));
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());
                        wb.Close(false, path, false);
                        foreach (var process in Process.GetProcessesByName("EXCEL"))
                        {
                            if (process.StartTime > startTime)
                            {
                                process.Kill();
                            }
                        }
                        return Json("Fail");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Could not get data from file");
            }

            return Json("File uploaded successfully");
        }

        // GET: Calibration/Edit/5
        public ActionResult Edit(string assetnum, string date)
        {
            return View();
        }

        // POST: Calibration/Edit/5
        [HttpPost]
        public ActionResult Edit(string type, string assetnum, string date)
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
        public ActionResult Delete(string type, string assetnum, string date)
        {
            try
            {
                var assetNumber = assetnum.Replace("_", " ");
                var datePieces = date.Split('/');
                var calDate = new DateTime(Convert.ToInt32(datePieces[2]), Convert.ToInt32(datePieces[0]), Convert.ToInt32(datePieces[1]));
                var deviceType = "";

                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.DateFormatString = "MM/dd/yyyy";

                if (type.Equals("Attenuator"))
                {
                    deviceType = "Attenuator";

                    var ids = (from val in db.tblATCalHeaders
                              where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                              select val.id).ToList();

                    foreach (long id in ids) {
                        tblATCalHeaders data = db.tblATCalHeaders.Find(id);
                        db.tblATCalHeaders.Remove(data);
                    }
                }
                else if (type.Equals("OutputCoupler"))
                {
                    deviceType = "Output Coupler";

                    var ids = (from val in db.tblOCCalHeaders
                               where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                              select val.id).ToList();

                    foreach (long id in ids)
                    {
                        tblOCCalHeaders data = db.tblOCCalHeaders.Find(id);
                        db.tblOCCalHeaders.Remove(data);
                    }
                }
                else if (type.Equals("PowerSensor"))
                {
                    deviceType = "Power Sensor";

                    var ids = (from val in db.tblPSCalHeaders
                              where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                              select val.id).ToList();

                    foreach (long id in ids)
                    {
                        tblPSCalHeaders data = db.tblPSCalHeaders.Find(id);
                        db.tblPSCalHeaders.Remove(data);
                    }
                }

                var records = (from val in db.tblCalData
                           where val.AssetNumber.Equals(assetnum) && val.CalDate.Equals(calDate) && val.DeviceType.Equals(deviceType)
                           select val.id).ToList();
                foreach (long id in records)
                {
                    tblCalData data = db.tblCalData.Find(id);
                    db.tblCalData.Remove(data);
                }

                db.SaveChanges();

                return Content(JsonConvert.SerializeObject(new { type, assetnum, date }));
            }
            catch
            {
                return Content("Fail");
            }
        }
    }
}
