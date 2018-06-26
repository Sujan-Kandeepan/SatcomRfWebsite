//Note: ClosedXML requires DocumentFormat.OpenXML v:2.5 exactly (as of time of writing)
//TODO: Add comments maybe?
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using ClosedXML.Excel;

using SatcomRfWebsite.Models;

namespace SatcomRfWebsite.Controllers
{
    public class DataNotFoundException : SystemException
    {
        public DataNotFoundException() : base() { }
    }

    public class ExcelFileResponse : IHttpActionResult
    {
        private byte[] filecontent;
        private HttpRequestMessage src;
        private string filename;

        public ExcelFileResponse(byte[] bin, HttpRequestMessage req, string infilename)
        {
            filecontent = bin;
            src = req;
            filename = infilename;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.RequestMessage = src;
            response.Content = new ByteArrayContent(filecontent);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = filename
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

            return Task.FromResult(response);
        }
    }

    public class ListAPIController : ApiController
    {
        [NonAction]
        private int Compare(TestData lhs, TestData rhs)
        {
            if (lhs.TestName == rhs.TestName && lhs.Channel != "" && rhs.Channel != "")
            {
                int numlhs, numrhs;
                bool trylhs = int.TryParse(lhs.Channel, out numlhs);
                bool tryrhs = int.TryParse(rhs.Channel, out numrhs);

                if ((trylhs == true) && (tryrhs == true))
                {
                    if (numlhs != numrhs)
                    {
                        return numlhs - numrhs;
                    }
                }

                if (lhs.Channel == rhs.Channel && lhs.Power != "" && rhs.Power != "")
                {
                    return lhs.Power.CompareTo(rhs.Power);
                }

                return lhs.Channel.CompareTo(rhs.Channel);
            }

            return lhs.TestName.CompareTo(rhs.TestName);
        }

        [NonAction]
        private string GetSqlConnectString()
        {
            return ConfigurationManager.ConnectionStrings["ATE_SQL"].ConnectionString;
        }

        [NonAction]
        private List<string> ReadAllData(SqlDataReader sql, string column)
        {
            var result = new List<string>();
            while (sql.Read())
            {
                var tmp = (IDataRecord)sql;
                result.Add(tmp[column].ToString());
            }
            return result;
        }

        [NonAction]
        public List<TestData> InternalGetTableData(string modelName, string productType, string testType, string tubeName, string options)
        {
            var data = new List<TestData>();
            using (var conn = new SqlConnection(GetSqlConnectString()))
            {
                conn.Open();
                var cmd = new SqlCommand("", conn);
                List<string> sqlResultData = null;

                if (modelName == "all")
                {
                    cmd.CommandText = "SELECT ModelName FROM dbo.tblModelNames WHERE ProductType = @productType;";
                    var productTypeParam = new SqlParameter("@productType", SqlDbType.NVarChar, 10);
                    productTypeParam.Value = productType;
                    cmd.Parameters.Add(productTypeParam);
                    cmd.Prepare();

                    SqlDataReader sqlResultModels = cmd.ExecuteReader();

                    if (!sqlResultModels.HasRows)
                    {
                        throw new DataNotFoundException();
                    }

                    List<string> sqlResultModelData = ReadAllData(sqlResultModels, "ModelName");
                    sqlResultModels.Close();
                    cmd.CommandText = "SELECT ModelSN FROM dbo.tblSerialNumbers WHERE ModelName = @name;";
                    var modelParam = new SqlParameter("@name", SqlDbType.NVarChar, 30);
                    cmd.Parameters.Add(modelParam);
                    cmd.Prepare();
                    sqlResultData = new List<string>();

                    foreach (string i in sqlResultModelData)
                    {
                        modelParam.Value = i;
                        SqlDataReader sqlResultTmp = cmd.ExecuteReader();

                        if (!sqlResultTmp.HasRows)
                        {
                            throw new DataNotFoundException();
                        }
                        sqlResultData.AddRange(ReadAllData(sqlResultTmp, "ModelSN"));
                        sqlResultTmp.Close();
                    }

                }
                else
                {
                    cmd.CommandText = "SELECT ModelSN FROM dbo.tblSerialNumbers WHERE ModelName = @name;";
                    var modelParam = new SqlParameter("@name", SqlDbType.NVarChar, 30);
                    modelParam.Value = modelName;
                    cmd.Parameters.Add(modelParam);
                    cmd.Prepare();

                    SqlDataReader sqlResult = cmd.ExecuteReader();

                    if (!sqlResult.HasRows)
                    {
                        throw new DataNotFoundException();
                    }
                    sqlResultData = ReadAllData(sqlResult, "ModelSN");
                    sqlResult.Close();
                }

                if (productType.ToUpper().Contains("GENIV"))
                {
                    cmd.CommandText = "SELECT TestName,dbo.tblKLYTestResults.StartTime,Result,Units,Channel,LowLimit,UpLimit,P2,TestType,Audit,Itar,LongModelName,TubeSN,TubeName,SsaSN,LinSN,LipaSN,BucSN,BipaSN,BlipaSN FROM dbo.tblKLYTestResults JOIN dbo.tblATEOutput ON dbo.tblKLYTestResults.ModelSN = dbo.tblATEOutput.ModelSN WHERE dbo.tblKLYTestResults.ModelSn = @sn AND NOT Result = 'PASS' AND NOT Result = 'FAIL';";
                }
                else
                {
                    cmd.CommandText = "SELECT TestName,dbo.tblTWTTestResults.StartTime,Result,Units,Channel,LowLimit,UpLimit,P2,TestType,Audit,Itar,LongModelName,TubeSN,TubeName,SsaSN,LinSN,LipaSN,BucSN,BipaSN,BlipaSN FROM dbo.tblTWTTestResults JOIN dbo.tblATEOutput ON dbo.tblTWTTestResults.ModelSN = dbo.tblATEOutput.ModelSN WHERE dbo.tblTWTTestResults.ModelSn = @sn AND NOT Result = 'PASS' AND NOT Result = 'FAIL';";
                }

                var snParam = new SqlParameter("@sn", SqlDbType.NVarChar, 25);
                cmd.Parameters.Add(snParam);
                cmd.Prepare();

                var raw = new Dictionary<string, TestInfo>();
                foreach (var i in sqlResultData)
                {
                    snParam.Value = i;
                    SqlDataReader sqlResult2 = cmd.ExecuteReader();

                    while (sqlResult2.Read())
                    {
                        var tmp2 = (IDataRecord)sqlResult2;
                        var tinfo = new TestInfo(tmp2["TestName"].ToString(), tmp2["Channel"].ToString(), tmp2["P2"].ToString(), tmp2["Units"].ToString(),
                            new List<ResultData>() { new ResultData(i, tmp2["TestType"].ToString(), tmp2["StartTime"].ToString(), tmp2["Result"].ToString(), "---", tmp2["LowLimit"].ToString(),
                            tmp2["UpLimit"].ToString(), tmp2["Audit"].ToString(), tmp2["Itar"].ToString(), tmp2["LongModelName"].ToString(), tmp2["TubeSN"].ToString(), tmp2["TubeName"].ToString(),
                            tmp2["SsaSN"].ToString(), tmp2["LinSN"].ToString(), tmp2["LipaSN"].ToString(), tmp2["BucSN"].ToString(), tmp2["BipaSN"].ToString(), tmp2["BlipaSN"].ToString())});
                        var key = tmp2["TestName"].ToString() + tmp2["Channel"].ToString() + tmp2["P2"].ToString();


                        string[] flagList = !(options == null || options == "" || options == "none") ? options.Split(',') : new string[0];

                        if (testType != null && !testType.Equals("null") && !testType.Equals("none")
                            && !testType.Equals("undefined") && !tmp2["TestType"].Equals(testType)
                            || tubeName != null && !tubeName.Equals("null") && !tubeName.Equals("none") 
                            && !tubeName.Equals("undefined") && !tmp2["TubeName"].Equals(tubeName)
                            || flagList.Contains("Audit") && tmp2["Audit"].ToString().Equals("False")
                            || flagList.Contains("Itar") && tmp2["Itar"].ToString().Equals("False")
                            || flagList.Contains("SsaSN") && tmp2["SsaSN"].ToString().Equals("")
                            || flagList.Contains("LinSN") && tmp2["LinSN"].ToString().Equals("")
                            || flagList.Contains("LipaSN") && tmp2["LipaSN"].ToString().Equals("")
                            || flagList.Contains("BucSN") && tmp2["BucSN"].ToString().Equals("")
                            || flagList.Contains("BipaSN") && tmp2["BipaSN"].ToString().Equals("")
                            || flagList.Contains("BlipaSN") && tmp2["BlipaSN"].ToString().Equals(""))
                        {
                            continue;
                        }
                        else if (raw.ContainsKey(key))
                        {
                            raw[key].Results.Add(new ResultData(i, tmp2["TestType"].ToString(), tmp2["StartTime"].ToString(), tmp2["Result"].ToString(), "---", tmp2["LowLimit"].ToString(), 
                            tmp2["UpLimit"].ToString(), tmp2["Audit"].ToString(), tmp2["Itar"].ToString(), tmp2["LongModelName"].ToString(), tmp2["TubeSN"].ToString(), tmp2["TubeName"].ToString(),
                            tmp2["SsaSN"].ToString(), tmp2["LinSN"].ToString(), tmp2["LipaSN"].ToString(), tmp2["BucSN"].ToString(), tmp2["BipaSN"].ToString(), tmp2["BlipaSN"].ToString()));
                        }
                        else
                        {
                            raw.Add(key, tinfo);
                        }
                    }

                    var keys = new List<string>(raw.Keys);
                    foreach (string key in keys)
                    {
                        raw[key].Results = (raw[key].Results.OrderBy(x => x.SerialNumber)).ToList();
                    }

                    sqlResult2.Close();
                }

                cmd.Dispose();
                for (var i = 0; i < raw.Count(); i++)
                {
                    try
                    {
                        /* --- Listing all fields associated with individual test records ---
                        foreach (var v in raw.ElementAt(i).Value.Results)
                        {
                            System.Diagnostics.Debug.WriteLine(v);
                        }
                        System.Diagnostics.Debug.WriteLine("");*/
                        var longest = raw.ElementAt(i).Value.Results.OrderByDescending(x => x.Result.Length).First();
                        int rounding = 15;
                        if (longest.Result.IndexOf(".") != -1)
                        {
                            rounding = longest.Result.Length - longest.Result.IndexOf(".") - 1;
                        }
                        if (rounding > 5)
                        {
                            rounding = 5;
                        }
                        var tmp = new TestData();
                        //foreach (ResultData x in raw.ElementAt(i).Value.Results.OrderBy(x => Convert.ToDouble(Regex.Replace(x.Result.Replace("Below ", "").Replace("+/-", "").Replace(":1", ""), "[^0-9.E-]", "")))) { System.Diagnostics.Debug.Write("<" + x.Result + "> "); }
                        //System.Diagnostics.Debug.WriteLine("");
                        var rawtmp = raw.ElementAt(i).Value.Results.Select(val => { val.Result = Convert.ToString(Convert.ToDouble(Regex.Replace(val.Result.Replace("Below ", "").Replace("+/-", "").Replace(":1", ""), "[^0-9.E-]", ""))); return val; });
                        var rawtmp2 = from val in rawtmp select Convert.ToDouble(val.Result);
                        tmp.TestName = raw.ElementAt(i).Value.TestName;
                        tmp.Unit = raw.ElementAt(i).Value.Units;
                        tmp.Channel = (string.IsNullOrEmpty(raw.ElementAt(i).Value.Channel) ? "N/A" : raw.ElementAt(i).Value.Channel);
                        tmp.Power = (string.IsNullOrEmpty(raw.ElementAt(i).Value.Power) ? "N/A" : raw.ElementAt(i).Value.Power);
                        tmp.MinResult = Math.Round(rawtmp2.Min(), rounding).ToString("G4", CultureInfo.InvariantCulture);
                        tmp.MaxResult = Math.Round(rawtmp2.Max(), rounding).ToString("G4", CultureInfo.InvariantCulture);
                        tmp.AvgResult = Math.Round(rawtmp2.Average(), rounding).ToString("G4", CultureInfo.InvariantCulture);

                        var tempSum = 0.0;
                        foreach (var item in rawtmp2)
                        {
                            tempSum += Math.Pow(item - rawtmp2.Average(), 2);
                        }
                        var tmpStd = Math.Sqrt(tempSum / rawtmp2.Count());
                        tmp.StdDev = Math.Round(tmpStd, rounding).ToString("G4", CultureInfo.InvariantCulture);

                        bool emptyLowLimit = false, variableLowLimit = false, emptyUpLimit = false, variableUpLimit = false;
                        for (int r = 0; r < raw.ElementAt(i).Value.Results.Count(); r++)
                        {
                            if (string.IsNullOrEmpty(raw.ElementAt(i).Value.Results[r].LowLimit))
                            {
                                emptyLowLimit = true;
                            }
                            else if (raw.ElementAt(i).Value.Results[r].LowLimit != raw.ElementAt(i).Value.Results[0].LowLimit)
                            {
                                variableLowLimit = true;
                            }

                            if (string.IsNullOrEmpty(raw.ElementAt(i).Value.Results[r].UpLimit))
                            {
                                emptyUpLimit = true;
                            }
                            else if (raw.ElementAt(i).Value.Results[r].UpLimit != raw.ElementAt(i).Value.Results[0].UpLimit)
                            {
                                variableUpLimit = true;
                            }
                        }

                        var cpl = Double.PositiveInfinity;
                        var cpu = Double.PositiveInfinity;
                        if (!emptyLowLimit && !variableLowLimit)
                        {
                            cpl = (Convert.ToDouble(tmp.AvgResult) - Convert.ToDouble(Regex.Replace(raw.ElementAt(i).Value.Results[0].LowLimit.Replace("Below ", "").Replace("+/-", "").Replace(":1", ""), "[^0-9.E-]", ""))) / (3 * tmpStd);
                        }
                        if (!emptyUpLimit && !variableUpLimit) {
                            cpu = (Convert.ToDouble(Regex.Replace(raw.ElementAt(i).Value.Results[0].UpLimit.Replace("Below ", "").Replace("+/-", "").Replace(":1", ""), "[^0-9.E-]", "")) - Convert.ToDouble(tmp.AvgResult)) / (3 * tmpStd);
                        }
                        tmp.Cpk = Math.Round(Math.Min(cpu, cpl), rounding).ToString("G4", CultureInfo.InvariantCulture);
                        if (Double.IsInfinity(Convert.ToDouble(tmp.Cpk)))
                        {
                            tmp.Cpk = "N/A";
                        }

                        tmp.AllResults = rawtmp.ToList();

                        tmp.UnitConv = "---";
                        tmp.MinResultConv = "---";
                        tmp.MaxResultConv = "---";
                        tmp.AvgResultConv = "---";
                        tmp.StdDevConv = "---";

                        string[] largeW = { "kW", "MW", "GW", "TW", "PW", "EW", "ZW", "YW" };
                        string[] smallW = { "mW", "µW", "nW", "pW", "fW", "aW", "zW", "yW" };
                        double tempMin = 0, tempMax = 0, tempAvg = 0, tempStd = 0, tempSum2;
                        int wIndex;

                        switch (tmp.Unit)
                        {
                            case "dB":
                            case "dBc":
                            case "dB/MHz":
                            case "dBW/4KHz":
                                tempMin = Math.Pow(10, rawtmp2.Min() / 10);
                                tempMax = Math.Pow(10, rawtmp2.Max() / 10);
                                tempAvg = Math.Pow(10, rawtmp2.Average() / 10);

                                tempSum2 = 0.0;
                                for (int z = 0; z < rawtmp.Count(); z++)
                                {
                                    double c = Math.Pow(10, Convert.ToDouble((rawtmp.ToList())[z].Result) / 10);
                                    tempSum2 += Math.Pow(c - tempAvg, 2);
                                    tmp.AllResults[z].ResultConv = Convert.ToString(c);
                                }
                                tempStd = Math.Sqrt(tempSum2 / rawtmp.Count());

                                break;
                            case "dBm":
                                tempMin = Math.Pow(10, (rawtmp2.Min() - 30) / 10);
                                tempMax = Math.Pow(10, (rawtmp2.Max() - 30) / 10);
                                tempAvg = Math.Pow(10, (rawtmp2.Average() - 30) / 10);

                                tempSum2 = 0.0;
                                for (int z = 0; z < rawtmp.Count(); z++)
                                {
                                    double c = Math.Pow(10, (Convert.ToDouble((rawtmp.ToList())[z].Result) - 30) / 10);
                                    tempSum2 += Math.Pow(c - tempAvg, 2);
                                    tmp.AllResults[z].ResultConv = Convert.ToString(c);
                                }
                                tempStd = Math.Sqrt(tempSum2 / rawtmp.Count());

                                break;
                            case "deg/dB":
                            case "o/dB":
                                tempMin = 1 / Math.Pow(10, 1 / rawtmp2.Min() / 10);
                                tempMax = 1 / Math.Pow(10, 1 / rawtmp2.Max() / 10);
                                tempAvg = 1 / Math.Pow(10, 1 / rawtmp2.Average() / 10);

                                tempSum2 = 0.0;
                                for (int z = 0; z < rawtmp.Count(); z++)
                                {
                                    double c = 1 / Math.Pow(10, 1 / Convert.ToDouble((rawtmp.ToList())[z].Result) / 10);
                                    tempSum2 += Math.Pow(c - tempAvg, 2);
                                    tmp.AllResults[z].ResultConv = Convert.ToString(c);
                                }
                                tempStd = Math.Sqrt(tempSum2 / rawtmp.Count());

                                string[] temp = largeW;
                                largeW = smallW;
                                smallW = temp;

                                break;
                        }

                        if (tmp.Unit.Contains("dB"))
                        {
                            tmp.UnitConv = "W";

                            wIndex = 0;
                            for (int x = 0; x < 8; x++)
                            {
                                if (tempAvg > 1000)
                                {
                                    tempMin /= 1000;
                                    tempMax /= 1000;
                                    tempAvg /= 1000;
                                    tempStd /= 1000;
                                    for (int z = 0; z < tmp.AllResults.Count(); z++)
                                    {
                                        tmp.AllResults[z].ResultConv = Convert.ToString(Convert.ToDouble(tmp.AllResults[z].ResultConv) / 1000);
                                    }
                                    tmp.UnitConv = largeW[wIndex < 7 ? wIndex++ : wIndex];
                                }
                                else if (tempAvg != 0 && tempAvg < 1 || Math.Round(tempStd, rounding) == 0 && tempMin != tempMax && tempAvg < 1)
                                {
                                    tempMin *= 1000;
                                    tempMax *= 1000;
                                    tempAvg *= 1000;
                                    tempStd *= 1000;
                                    for (int z = 0; z < tmp.AllResults.Count(); z++)
                                    {
                                        tmp.AllResults[z].ResultConv = Convert.ToString(Convert.ToDouble(tmp.AllResults[z].ResultConv) * 1000);
                                }
                                    tmp.UnitConv = smallW[wIndex < 7 ? wIndex++ : wIndex];
                                }
                                else
                                {
                                    x = 8;
                                }
                            }

                            tmp.MinResultConv = Math.Round(tempMin, rounding).ToString("G4", CultureInfo.InvariantCulture);
                            tmp.MaxResultConv = Math.Round(tempMax, rounding).ToString("G4", CultureInfo.InvariantCulture);
                            tmp.AvgResultConv = Math.Round(tempAvg, rounding).ToString("G4", CultureInfo.InvariantCulture);
                            tmp.StdDevConv = Math.Round(tempStd, rounding).ToString("G4", CultureInfo.InvariantCulture);
                        }

                    for (int x = 0; x < tmp.AllResults.Count(); x++)
                    {
                        tmp.AllResults[x].Result = (Math.Round(Convert.ToDouble(tmp.AllResults[x].Result), rounding)).ToString("G4", CultureInfo.InvariantCulture);
                        if (!tmp.AllResults[x].ResultConv.Equals("---"))
                        {
                            tmp.AllResults[x].ResultConv = (Math.Round(Convert.ToDouble(tmp.AllResults[x].ResultConv), rounding)).ToString("G4", CultureInfo.InvariantCulture);
                        }
                    }

                    switch (tmp.Unit)
                        {
                            case "dB/MHz":
                                tmp.UnitConv += "/MHz";
                                break;
                            case "dBW/4KHz":
                                tmp.UnitConv += "/4KHz";
                                break;
                            case "deg/dB":
                                tmp.UnitConv = "deg/" + tmp.UnitConv;
                                break;
                            case "o/dB":
                                tmp.UnitConv = "o/" + tmp.UnitConv;
                                break;
                        }

                        tmp.ParsableResults = String.Join(",", tmp.AllResults);

                        data.Add(tmp);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Write(e.ToString());
                    }
                }

                conn.Close();
            }

            data.Sort(new Comparison<TestData>(Compare));
            return data;
        }

        public IHttpActionResult GetTableData(string modelName, string productType, string testType, string tubeName, string options)
        {
            try
            {
                List<TestData> data = InternalGetTableData(modelName, productType, testType, tubeName, options);
                return Ok(data);
            }
            catch (DataNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.ToString());
                return InternalServerError();
            }
        }

        public IHttpActionResult GetTableFile(string modelName, string productType, string testType, string tubeName, string options)
        {
            try
            {
                List<TestData> data = InternalGetTableData(modelName, productType, testType, tubeName, options);
                string[][] headers = new string[1][];
                headers[0] = new string[] { "Testname", "Channel", "Power", "Serial Number", "Test Type", "Start Time", "Audit", "Itar", "Long Model Name", "TubeSN", "Tube Name", "SsaSN", "LinSN", "LipaSN", "BucSN", "BipaSN", "BlipaSN", "Result", "Min", "Max", "Average", "Std. Deviation", "Unit", "Result (Conv)", "Min (Conv)", "Max (Conv)", "Average (Conv)", "Std. Deviation (Conv)", "Unit (Conv)", "LowLimit", "UpLimit", "Cpk" };
                var file = new MemoryStream();
                var document = new XLWorkbook();
                var worksheet = document.Worksheets.Add("Table Data");
                worksheet.Cell(1, 1).InsertData(headers);
                var insertionIndex = 2;
                foreach (TestData test in data)
                {
                    for (int i = 0; i < test.AllResults.Count(); i++)
                    {
                        var serial = test.AllResults[i].SerialNumber;
                        var thisTestType = test.AllResults[i].TestType;
                        var startTime = test.AllResults[i].StartTime;
                        var audit = test.AllResults[i].Audit;
                        var itar = test.AllResults[i].Itar;
                        var longModelName = test.AllResults[i].LongModelName;
                        var tubeSN = test.AllResults[i].TubeSN;
                        var thisTubeName = test.AllResults[i].TubeName;
                        var ssaSN = test.AllResults[i].SsaSN;
                        var linSN = test.AllResults[i].LinSN;
                        var lipaSN = test.AllResults[i].LipaSN;
                        var bucSN = test.AllResults[i].BucSN;
                        var bipaSN = test.AllResults[i].BipaSN;
                        var blipaSN = test.AllResults[i].BlipaSN;
                        var result = test.AllResults[i].Result;
                        var resultConv = test.AllResults[i].ResultConv;
                        var lowLimit = test.AllResults[i].LowLimit;
                        var upLimit = test.AllResults[i].UpLimit;

                        worksheet.Cell(insertionIndex, 1).SetValue(test.TestName);
                        worksheet.Cell(insertionIndex, 2).SetValue(test.Channel);
                        worksheet.Cell(insertionIndex, 3).SetValue(test.Power);
                        worksheet.Cell(insertionIndex, 4).SetValue(serial != "" ? serial : "---");
                        worksheet.Cell(insertionIndex, 5).SetValue(thisTestType != "" ? thisTestType : "---");
                        worksheet.Cell(insertionIndex, 6).SetValue(startTime != "" ? startTime : "---");
                        worksheet.Cell(insertionIndex, 7).SetValue(audit != "" ? audit : "---");
                        worksheet.Cell(insertionIndex, 8).SetValue(itar != "" ? itar : "---");
                        worksheet.Cell(insertionIndex, 9).SetValue(longModelName != "" ? longModelName : "---");
                        worksheet.Cell(insertionIndex, 10).SetValue(tubeSN != "" ? tubeSN : "---");
                        worksheet.Cell(insertionIndex, 11).SetValue(thisTubeName != "" ? thisTubeName : "---");
                        worksheet.Cell(insertionIndex, 12).SetValue(ssaSN != "" ? ssaSN : "---");
                        worksheet.Cell(insertionIndex, 13).SetValue(linSN != "" ? linSN : "---");
                        worksheet.Cell(insertionIndex, 14).SetValue(lipaSN != "" ? lipaSN : "---");
                        worksheet.Cell(insertionIndex, 15).SetValue(bucSN != "" ? bucSN : "---");
                        worksheet.Cell(insertionIndex, 16).SetValue(bipaSN != "" ? bipaSN : "---");
                        worksheet.Cell(insertionIndex, 17).SetValue(blipaSN != "" ? blipaSN : "---");
                        worksheet.Cell(insertionIndex, 18).SetValue(result != "" ? result : "---");
                        worksheet.Cell(insertionIndex, 19).SetValue(test.MinResult);
                        worksheet.Cell(insertionIndex, 20).SetValue(test.MaxResult);
                        worksheet.Cell(insertionIndex, 21).SetValue(test.AvgResult);
                        worksheet.Cell(insertionIndex, 22).SetValue(test.StdDev);
                        worksheet.Cell(insertionIndex, 23).SetValue(test.Unit);
                        worksheet.Cell(insertionIndex, 24).SetValue(resultConv != "" ? resultConv : "---");
                        worksheet.Cell(insertionIndex, 25).SetValue(test.MinResultConv);
                        worksheet.Cell(insertionIndex, 26).SetValue(test.MaxResultConv);
                        worksheet.Cell(insertionIndex, 27).SetValue(test.AvgResultConv);
                        worksheet.Cell(insertionIndex, 28).SetValue(test.StdDevConv);
                        worksheet.Cell(insertionIndex, 29).SetValue(test.UnitConv);
                        worksheet.Cell(insertionIndex, 30).SetValue(lowLimit != "" ? lowLimit : "---");
                        worksheet.Cell(insertionIndex, 31).SetValue(upLimit != "" ? upLimit : "---");
                        worksheet.Cell(insertionIndex, 32).SetValue(test.Cpk);

                        insertionIndex++;
                    }
                    worksheet.Cell(insertionIndex++, 1).InsertData(new string[] { });
                }

                var style = document.Style;
                style.Font.Bold = true;
                worksheet.Row(1).Style = style;
                worksheet.Columns().AdjustToContents();
                worksheet.RangeUsed().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                worksheet.SheetView.FreezeRows(1);
                worksheet.SheetView.FreezeColumns(1);
                worksheet.SheetView.FreezeColumns(2);
                worksheet.SheetView.FreezeColumns(3);
                worksheet.Column(1).Width = 20;
                worksheet.Column(3).Width = 10;
                worksheet.Column(23).Width = 10;
                document.SaveAs(file);
                string filename = DateTime.Now.ToString("yyyy-MM-dd") + $" {productType} {modelName}.xlsx";
                if (testType != null && !testType.Equals("null"))
                {
                    filename = filename.Replace(".xlsx", $" {testType}.xlsx");
                }
                if (tubeName != null && !tubeName.Equals("null"))
                {
                    filename = filename.Replace(".xlsx", $" {tubeName}.xlsx");
                }
                if (options != null && !options.Equals("null"))
                {
                    filename = filename.Replace(".xlsx", $" {options}.xlsx");
                }
                var resp = new ExcelFileResponse(file.ToArray(), Request, filename);
                file.Dispose();
                return resp;
            }
            catch (DataNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.ToString());
                return InternalServerError();
            }
        }

        public IHttpActionResult GetTubes(string modelName)
        {
            var db = new rfDbEntities();

            var myQuery = (from serialNums in db.tblSerialNumbers
                           join ateOut in db.tblATEOutputs on serialNums.ModelSN equals ateOut.ModelSN
                           where serialNums.ModelName.Equals(modelName)
                           orderby ateOut.TubeName ascending
                           select ateOut).Select(x => x.TubeName).Distinct();

            var tubeList = new List<string>();

            foreach (var output in myQuery)
            {
                if (output != null)
                {
                    tubeList.Add(output.ToString());
                }
            }

            return Ok(tubeList);
        }

    public IHttpActionResult GetModels(string productType)
        {
            List<string> data = null;
            using (var conn = new SqlConnection(GetSqlConnectString()))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ModelName FROM dbo.tblModelNames WHERE ProductType = @name;", conn);
                var nameParam = new SqlParameter("@name", SqlDbType.NVarChar, 100);
                nameParam.Value = productType;
                cmd.Parameters.Add(nameParam);
                cmd.Prepare();
                SqlDataReader sqlResult = cmd.ExecuteReader();

                if (!sqlResult.HasRows)
                {
                    return NotFound();
                }
                else
                {
                    data = ReadAllData(sqlResult, "ModelName");
                    data.Sort();
                }

                cmd.Dispose();
                conn.Close();
            }

            return Ok(data);
        }

        public IHttpActionResult GetProductTypes()
        {
            List<string> data = null;
            using (var conn = new SqlConnection(GetSqlConnectString()))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ProductType FROM dbo.tblProductTypes", conn);
                SqlDataReader sqlResult = cmd.ExecuteReader();

                if (!sqlResult.HasRows)
                {
                    return NotFound();
                }
                else
                {
                    data = ReadAllData(sqlResult, "ProductType");
                    data.Sort();
                }

                cmd.Dispose();
                conn.Close();
            }

            return Ok(data);
        }
    }
}
