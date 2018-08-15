using Microsoft.Office.Interop.Excel;
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
using System.Globalization;
using System.Web.Script.Serialization;

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
            var list = from val in db.tblCalDevices
                       .Where(device => device.DeviceType.Equals(type))
                       .OrderBy(device => device.AssetNumber)
                       select val.AssetNumber;

            return Json(list.Distinct().ToList());
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
            try
            {
                var assetNumber = assetnum.Replace("_", " ");
                var calDate = DateTime.Now;
                if (!string.IsNullOrEmpty(date))
                {
                    var datePieces = date.Split('/');
                    calDate = new DateTime(Convert.ToInt32(datePieces[2]), Convert.ToInt32(datePieces[0]), Convert.ToInt32(datePieces[1]));
                }
                else
                {
                    var dates = (from val in db.tblCalData where val.AssetNumber.Equals(assetNumber) orderby val.CalDate descending select val.CalDate).ToList();
                    if (dates.Count() > 0) calDate = dates[0];
                }

                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.DateFormatString = "MM/dd/yyyy";

                string headers = JsonConvert.SerializeObject(new object());

                if (type.Equals("Attenuator"))
                {
                    var id = (from val in db.tblATCalHeaders
                              where val.AssetNumber.Equals(assetNumber) && val.CalDate.Equals(calDate)
                              select val.id).ToList();

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
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Content("Fail");
            }
        }

        public ActionResult GenerateTextFile(string type, string assetnum, string date)
        {
            try
            {
                string details = ((ContentResult)GetDetails(type, assetnum, date)).Content;
                if (details.Equals("Fail")) return Content("Could not generate text file!");

                JavaScriptSerializer jss = new JavaScriptSerializer();
                Dictionary<string, object> content = jss.Deserialize<object>(details) as Dictionary<string, object>;
                Dictionary<string, object> headers = jss.Deserialize<object>(content["headers"].ToString()) as Dictionary<string, object>;
                List<double> freqs = (from item in (content["freqs"] as object[]) select Convert.ToDouble(item)).ToList();
                List<double> calFactor = (from item in (content["calFactor"] as object[]) select Convert.ToDouble(item)).ToList();
                List<string> lines = new List<string>();

                Dictionary<string, string> types = new Dictionary<string, string>() {
                    { "Attenuator", "Attenuator" },
                    { "OutputCoupler", "Load-Coupler" },
                    { "PowerSensor", "Power Sensor" }
                };
                lines.Add(types[type] + " Asset Number: " + headers["AssetNumber"]);
                lines.Add("");

                lines.Add("CalibrationInfo");
                if (type.Equals("PowerSensor")) lines.Add("Series;Serial;Ref.CF;Certificate");
                else lines.Add("StartFreq;StopFreq;Points;Loss;Power;MaxOffset;Temp;Humidity;Lookback");
                lines.Add(String.Join(";", from item in lines.Last().Replace("Ref.CF", "RefCal").Split(';') select headers[item].ToString()));
                lines.Add("");

                lines.Add("Operator;ExpireDate");
                List<string> dates = new List<String>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                string datefound = headers[type.Equals("PowerSensor") ? "CalDate" : "ExpireDate"].ToString();
                string dateformatted = datefound.Split('/')[1] + "/" + dates[Convert.ToInt32(datefound.Split('/')[0]) - 1] + "/"
                    + (type.Equals("PowerSensor") ? (Convert.ToInt32(datefound.Split('/')[2]) + 1).ToString() : datefound.Split('/')[2]);
                lines.Add(headers["Operator"] + ";" + dateformatted);
                lines.Add("");

                lines.Add(type.Equals("PowerSensor") ? "Frequency(GHz);CF(%)" : "Frequency;Data");
                List<Tuple<double, double>> points = freqs.Zip(calFactor, (x, y) => Tuple.Create(x, y)).ToList();
                foreach (var point in points) lines.Add(point.Item1.ToString() + ";" + point.Item2.ToString());
                lines.Add("");
                lines.Add("");

                return Content(String.Join("\r\n", lines));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Content("Could not generate text file!\n" + e.ToString());
            }
        }

        public ActionResult ValidateForm(string type, string formString, string mode)
        {
            bool headersFilled = false, headersValid = false, freqsFilled = false, freqsValid = false,
                dataFilled = false, dataValid = false, matchesExisting = false;
            double ATTENUATOR_MAXRANGE = 1, OUTPUTCOUPLER_MAXRANGE = 2.5, POWERSENSOR_MINCHANGE = -2.5, POWERSENSOR_MAXCHANGE = 2.5;
            Dictionary<string, string> form = JsonConvert.DeserializeObject<Dictionary<string, string>>(formString);

            List<string> requiredHeaders = new List<string>();
            if (type.Equals("Attenuator") || type.Equals("OutputCoupler"))
            {
                requiredHeaders = new List<string>()
                {
                    "AssetNumber", "Operator", "CalDate", "StartFreq", "StopFreq",
                    "ExpireDate", "Loss", "Power", "MaxOffset", "Points"
                };
            }
            else if (type.Equals("PowerSensor"))
            {
                requiredHeaders = new List<string>()
                {
                    "AssetNumber", "Operator", "CalDate", "Series", "Serial",
                    "RefCal", "Certificate", "Points"
                };
            }
            if (requiredHeaders.Count() == 0) return Json(false);
            List<string> headerValues = (from item in requiredHeaders select form[item]).ToList();
            headersFilled = headerValues.Aggregate(true, (accumulator, next) => accumulator && !string.IsNullOrEmpty(next));

            try
            {
                if (type.Equals("Attenuator"))
                {
                    DateTime.ParseExact(form["CalDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime.ParseExact(form["ExpireDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    new ATCalibrationData
                    {
                        AssetNumber = form["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Request.LogonUserIdentity.Name.ToUpper(),
                        StartFreq = Convert.ToInt64(form["StartFreq"]),
                        StopFreq = Convert.ToInt64(form["StopFreq"]),
                        Points = Convert.ToInt32(form["Points"]),
                        Loss = Convert.ToInt64(form["Loss"]),
                        Power = Convert.ToInt64(form["Power"]),
                        MaxOffset = Convert.ToDouble(form["MaxOffset"]),
                        Temp = form["Temp"] != "" ? Convert.ToDouble(form["Temp"]) : (double?)null,
                        Humidity = form["Humidity"] != "" ? Convert.ToDouble(form["Humidity"]) : (double?)null,
                        Lookback = form["Lookback"] != "" ? form["Lookback"] : null,
                        Operator = form["Operator"],
                        CalDate = Convert.ToDateTime(form["CalDate"]),
                        ExpireDate = Convert.ToDateTime(form["ExpireDate"])
                    };
                }
                else if (type.Equals("OutputCoupler"))
                {
                    DateTime.ParseExact(form["CalDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime.ParseExact(form["ExpireDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    new OCCalibrationData
                    {
                        AssetNumber = form["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Request.LogonUserIdentity.Name.ToUpper(),
                        StartFreq = Convert.ToInt64(form["StartFreq"]),
                        StopFreq = Convert.ToInt64(form["StopFreq"]),
                        Points = Convert.ToInt32(form["Points"]),
                        Loss = Convert.ToInt64(form["Loss"]),
                        Power = Convert.ToInt64(form["Power"]),
                        MaxOffset = Convert.ToDouble(form["MaxOffset"]),
                        Temp = form["Temp"] != "" ? Convert.ToDouble(form["Temp"]) : (double?)null,
                        Humidity = form["Humidity"] != "" ? Convert.ToDouble(form["Humidity"]) : (double?)null,
                        Lookback = form["Lookback"] != "" ? form["Lookback"] : null,
                        Operator = form["Operator"],
                        CalDate = Convert.ToDateTime(form["CalDate"]),
                        ExpireDate = Convert.ToDateTime(form["ExpireDate"])
                    };
                }
                else if (type.Equals("PowerSensor"))
                {
                    DateTime.ParseExact(form["CalDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    new PSCalibrationData
                    {
                        AssetNumber = form["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Request.LogonUserIdentity.Name.ToUpper(),
                        Series = form["Series"],
                        Serial = form["Serial"],
                        RefCal = form["RefCal"],
                        Certificate = form["Certificate"],
                        Operator = form["Operator"],
                        CalDate = Convert.ToDateTime(form["CalDate"])
                    };
                }
                Convert.ToInt32(form["Points"]);
                headersValid = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            List<string> freqFields = (from item in form where item.Key.Contains("Frequency") select item.Key).ToList();
            List<string> freqStrings = (from item in freqFields select form[item]).ToList();
            freqsFilled = freqStrings.Aggregate(freqStrings.Count() > 0, (accumulator, next) => accumulator && !string.IsNullOrEmpty(next));
            List<double> freqValues = new List<double>();
            if (freqsFilled)
            {
                freqValues = (from item in freqStrings select Convert.ToDouble(item)).ToList();
                freqsValid = freqValues.SequenceEqual(freqValues.OrderBy(x => x)) && freqValues.SequenceEqual(freqValues.Distinct());
                if ((type.Equals("Attenuator") || type.Equals("OutputCoupler")) && !string.IsNullOrEmpty(form["StartFreq"]) && !string.IsNullOrEmpty(form["StopFreq"]))
                {
                    freqsValid = freqsValid && freqValues.First() == Convert.ToDouble(form["StartFreq"])
                         && freqValues.Last() == Convert.ToDouble(form["StopFreq"]);
                }
                List<double> atLeast1 = (from value in freqValues where value >= 1 select value).ToList();
                double interval = atLeast1[1] - atLeast1[0], previous = atLeast1[0] - interval;
                foreach (var num in atLeast1)
                {
                    if (num - previous != interval) freqsValid = false;
                    previous = num;
                }
            }

            List<string> dataFields = (from item in form where item.Key.Contains("CalFactor") select item.Key).ToList();
            List<string> dataStrings = (from item in dataFields select form[item]).ToList();
            dataFilled = dataStrings.Aggregate(dataStrings.Count() > 0, (accumulator, next) => accumulator && !string.IsNullOrEmpty(next));
            List<double> dataValues = new List<double>();
            if (dataFilled)
            {
                dataValues = (from item in dataStrings select Convert.ToDouble(item)).ToList();
                if (type.Equals("Attenuator"))
                {
                    dataValid = dataValues.Max() - dataValues.Min() < ATTENUATOR_MAXRANGE && dataValues.Max() < 0;
                }
                else if (type.Equals("OutputCoupler"))
                {
                    dataValid = dataValues.Max() - dataValues.Min() < OUTPUTCOUPLER_MAXRANGE && dataValues.Max() < 0;
                }
                else if (type.Equals("PowerSensor"))
                {
                    double previous = dataValues[0];
                    dataValid = true;
                    foreach (var num in dataValues)
                    {
                        if (num - previous < POWERSENSOR_MINCHANGE || num - previous > POWERSENSOR_MAXCHANGE) dataValid = false;
                        previous = num;
                    }
                    dataValid = dataValid && dataValues.Min() > 0;
                }
            }

            double estimate(List<double> freqs, List<double> data, double freq)
            {
                // Direct return of value if exists
                if (freqs.Contains(freq)) return data[freqs.IndexOf(freq)];
                
                // Estimation by linear interpolation
                int index = -1;
                for (int i = 0; i < freqs.Count() - 1; i++) if (freqs[i] <= freq && freqs[i + 1] > freq) index = i;
                if (index > -1)
                {
                    double x0 = freqs[index], x1 = freqs[index + 1], y0 = data[index], y1 = data[index + 1];
                    return (y0 * (x1 - freq) + y1 * (freq - x0)) / (x1 - x0);
                }
                
                // Estimation by extrapolation using linear regression
                double xMean = freqs.Aggregate(0.0, (sum, next) => sum + next) / freqs.Count();
                double yMean = data.Aggregate(0.0, (sum, next) => sum + next) / data.Count();
                List<Tuple<double, double>> line = freqs.Zip(data, (x, y) => Tuple.Create(x, y)).ToList();
                double slope = line.Aggregate(0.0, (sum, next) => sum + (next.Item1 - xMean) * (next.Item2 - yMean))
                    / line.Aggregate(0.0, (sum, next) => sum + Math.Pow(next.Item1 - xMean, 2));
                double intercept = yMean - slope * xMean;
                return slope * freq + intercept;
            }

            if (freqsValid && dataValid && !string.IsNullOrEmpty(form["AssetNumber"]))
            {
                string details = ((ContentResult)GetDetails(type, form["AssetNumber"], null)).Content;
                double TOLERANCE = type.Equals("PowerSensor") ? 5 : 1;
                if (!details.Equals("Fail"))
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    Dictionary<string, object> content = jss.Deserialize<object>(details) as Dictionary<string, object>;
                    List<double> freqs = (from item in (content["freqs"] as object[]) select Convert.ToDouble(item)).ToList();
                    List<double> calFactor = (from item in (content["calFactor"] as object[]) select Convert.ToDouble(item)).ToList();
                    List<Tuple<double, double>> givenPoints = freqValues.Zip(dataValues, (freq, data) => Tuple.Create(freq, data)).ToList();
                    matchesExisting = givenPoints.Aggregate(true, (accumulator, next) => accumulator && Math.Abs(estimate(freqs, calFactor, next.Item1) - next.Item2) < TOLERANCE);
                }
                else
                {
                    matchesExisting = true;
                }
            }
            if (mode.Equals("edit")) matchesExisting = true;

            string message = "";
            List<string> messages = new List<string>();
            if (!headersFilled) messages.Add("Not all required header fields have been filled. Fields not marked as 'Optional' are required for this record to be saved to the database.");
            if (!freqsFilled) messages.Add("Not all frequency fields have been filled. Frequency must be provided for the number of points specified, which also cannot be zero.");
            if (!dataFilled) messages.Add("Not all calibration data fields have been filled. Calibration factor must be provided for the number of points specified, which also cannot be zero.");
            if (headersFilled && !headersValid) messages.Add("One or more header fields were entered in an invalid format. Ensure that dates are in the MM/DD/YYYY format and numbers are specified appropriately.");
            if (freqsFilled && !freqsValid) messages.Add("Frequencies were not entered correctly. Values should be strictly increasing with consistent intervals and correspondent to header information on the left.");
            if (dataFilled && !dataValid) messages.Add("Abnormalities found in the calibration data. Values should be checked for correctness as large deviations/jumps or incorrect signs cannot be accepted.");
            if (freqsValid && dataValid && !matchesExisting) messages.Add("Given data lacks similarity to existing calibration records. Updated data values are expected to be approximately equal to what was previously recorded.");
            messages = (from item in messages select "&bull; " + item).ToList();
            message = String.Join("</br>", messages);
            bool isValid = headersFilled && headersValid && freqsFilled && freqsValid && dataFilled && dataValid && matchesExisting;
            return Json(new { isValid, message });
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
                    Delete("Attenuator", collection["AssetNumber"], collection["CalDate"]);

                    List<String> assetnums = (List<String>)GetAssetNumbers("Attenuator").Data;
                    if (!assetnums.Contains(collection["AssetNumber"]))
                    {
                        tblCalDevices device = new tblCalDevices
                        {
                            AssetNumber = collection["AssetNumber"],
                            DeviceType = "Attenuator"
                        };
                        db.tblCalDevices.Add(device);
                        db.SaveChanges();
                    }

                    return CreateAT(new ATCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Request.LogonUserIdentity.Name.ToUpper(),
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
                    Delete("OutputCoupler", collection["AssetNumber"], collection["CalDate"]);

                    List<String> assetnums = (List<String>)GetAssetNumbers("Output Coupler").Data;
                    if (!assetnums.Contains(collection["AssetNumber"]))
                    {
                        tblCalDevices device = new tblCalDevices
                        {
                            AssetNumber = collection["AssetNumber"],
                            DeviceType = "Output Coupler"
                        };
                        db.tblCalDevices.Add(device);
                        db.SaveChanges();
                    }

                    return CreateOC(new OCCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Request.LogonUserIdentity.Name.ToUpper(),
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
                    Delete("PowerSensor", collection["AssetNumber"], collection["CalDate"]);

                    List<String> assetnums = (List<String>)GetAssetNumbers("Power Sensor").Data;
                    if (!assetnums.Contains(collection["AssetNumber"]))
                    {
                        tblCalDevices device = new tblCalDevices
                        {
                            AssetNumber = collection["AssetNumber"],
                            DeviceType = "Power Sensor"
                        };
                        db.tblCalDevices.Add(device);
                        db.SaveChanges();
                    }

                    return CreatePS(new PSCalibrationData
                    {
                        AssetNumber = collection["AssetNumber"],
                        AddedDate = DateTime.Now,
                        EditedBy = Request.LogonUserIdentity.Name.ToUpper(),
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
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                if (Request.Url.ToString().Contains("Attenuator")) return Redirect(Url.Action("", "", null, Request.Url.Scheme) + "Calibration/Create/Attenuator?failed=true");
                if (Request.Url.ToString().Contains("OutputCoupler")) return Redirect(Url.Action("", "", null, Request.Url.Scheme) + "Calibration/Create/OutputCoupler?failed=true");
                if (Request.Url.ToString().Contains("PowerSensor")) return Redirect(Url.Action("", "", null, Request.Url.Scheme) + "Calibration/Create/PowerSensor?failed=true");
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

                return Redirect(Url.Action("", "", null, Request.Url.Scheme) + $"Calibration/Index/Attenuator?assetnum={atData.AssetNumber.Replace(" ", "_")}");
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

                return Redirect(Url.Action("", "", null, Request.Url.Scheme) + $"Calibration/Index/OutputCoupler?assetnum={ocData.AssetNumber.Replace(" ", "_")}");
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

                return Redirect(Url.Action("", "", null, Request.Url.Scheme) + $"Calibration/Index/PowerSensor?assetnum={psData.AssetNumber.Replace(" ", "_")}");
            }

            return View(psData);
        }

        public ActionResult CreateDataFields(int num, bool hasReturnLoss, string freqs, string calFactor, string returnLoss)
        {
            var freqList = JsonConvert.DeserializeObject<IEnumerable<string>>(freqs).ToList();
            var calFactorList = JsonConvert.DeserializeObject<IEnumerable<string>>(calFactor).ToList();
            var returnLossList = JsonConvert.DeserializeObject<IEnumerable<string>>(returnLoss).ToList();

            var html = num == 0 ? "<span style='color: grey'>Form fields to enter calfactor records will appear here</span>" : "";
            for (var i = 0; i < num; i++)
            {
                string freqValue = i < freqList.Count() ? freqList[i] : "";
                string calFactorValue = i < calFactorList.Count() ? calFactorList[i] : "";
                string returnLossValue = i < returnLossList.Count() ? returnLossList[i] : "";

                html += $"<div class='form-group row' style='margin-bottom: 5px' align='left'>";
                html += "<div class='col-md-" + (hasReturnLoss ? "4" : "6") + $"'><label class='control-label' for='Records_{i}__Frequency'>Frequency &middot;  {i + 1}</label><input class='form-control text-box single-line' data-val='true' data-val-number='The field Frequency must be a number.' data-val-required='The Frequency field is required.' id='Records_{i}__Frequency' name='Records[{i}].Frequency' type='number' value='{freqValue}'></div>";
                html += "<div class='col-md-" + (hasReturnLoss ? "4" : "6") + $"'><label class='control-label' for='Records_{i}__CalFactor'>Calibration Factor &middot; {i + 1}</label><input class='form-control text-box single-line' data-val='true' data-val-number='The field CalFactor must be a number.' data-val-required='The CalFactor field is required.' id='Records_{i}__CalFactor' name='Records[{i}].CalFactor' type='number' value='{calFactorValue}'></div>";
                if (hasReturnLoss) html += $"<div class='col-md-4'><label class='control-label' for='Records_{i}__ReturnLoss'>Return Loss &middot; {i + 1}</label><input class='form-control text-box single-line' data-val='true' data-val-number='The field ReturnLoss must be a number.' data-val-required='The ReturnLoss field is required.' id='Records_{i}__ReturnLoss' name='Records[{i}].ReturnLoss' placeholder='Optional' type='number' value='{returnLossValue}'></div>";
                html += "</div>";
            }
            return Content(html);
        }

        [HttpPost]
        public JsonResult ImportFile(string type)
        {
            try
            {
                if (Request.Files.Count == 0) return Json("Fail");
                var fileContent = Request.Files[0];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;
                    var fileName = Request.Files[0].FileName;
                    var path = Path.Combine(Server.MapPath("/App_Data"), fileName);
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
                        Debug.WriteLine(e.ToString());
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
                Debug.WriteLine(e.ToString());
                return Json("Fail");
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
        public ActionResult Edit(FormCollection collection)
        {
            var type = collection["TypePassed"];
            var assetnum = collection["AssetNumberPassed"];
            var date = collection["DatePassed"];
            var assetnumGiven = collection["AssetNumber"].Replace(" ", "_");
            var dateGiven = collection["CalDate"];

            try
            {
                Delete(type, assetnum, date);
                Create(collection);
                return Redirect(Url.Action("", "", null, Request.Url.Scheme) + $"Calibration/Details/{type}?assetnum={assetnumGiven}&date={dateGiven}");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
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
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return Content("Fail");
            }
        }
    }
}
