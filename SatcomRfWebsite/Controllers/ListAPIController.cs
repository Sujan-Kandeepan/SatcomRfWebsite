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
        public List<TestData> InternalGetTableData(string modelName, string productType)
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
                    cmd.CommandText = "SELECT TestName,dbo.tblKLYTestResults.StartTime,Result,Units,Channel,LowLimit,UpLimit,P2,Audit,Itar,SsaSN,LinSN,LipaSN,BucSN,BipaSN,BlipaSN FROM dbo.tblKLYTestResults JOIN dbo.tblATEOutput ON dbo.tblKLYTestResults.ModelSN = dbo.tblATEOutput.ModelSN WHERE dbo.tblKLYTestResults.ModelSn = @sn AND NOT Result = 'PASS' AND NOT Result = 'FAIL';";
                }
                else
                {
                    cmd.CommandText = "SELECT TestName,dbo.tblTWTTestResults.StartTime,Result,Units,Channel,LowLimit,UpLimit,P2,Audit,Itar,SsaSN,LinSN,LipaSN,BucSN,BipaSN,BlipaSN FROM dbo.tblTWTTestResults JOIN dbo.tblATEOutput ON dbo.tblTWTTestResults.ModelSN = dbo.tblATEOutput.ModelSN WHERE dbo.tblTWTTestResults.ModelSn = @sn AND NOT Result = 'PASS' AND NOT Result = 'FAIL';";
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
                            new List<ResultData>() { new ResultData(i, tmp2["StartTime"].ToString(), tmp2["Result"].ToString(), "---", tmp2["LowLimit"].ToString(), tmp2["UpLimit"].ToString()) });
                        var key = tmp2["TestName"].ToString() + tmp2["Channel"].ToString() + tmp2["P2"].ToString();

                        if (raw.ContainsKey(key))
                        {
                            raw[key].Results.Add(new ResultData(i, tmp2["StartTime"].ToString(), tmp2["Result"].ToString(), "---", tmp2["LowLimit"].ToString(), tmp2["UpLimit"].ToString()));
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
                    /* --- Listing all fields associated with individual test records ---
                    foreach (var v in raw.ElementAt(i).Value.Results)
                    {
                        System.Diagnostics.Debug.WriteLine(v[0] + " " + v[1] + " " + v[2] + " " + v[3] + " " + v[4]);
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
                    //foreach (String x in raw.ElementAt(i).Value.Results.OrderBy(x => Convert.ToDouble(Regex.Replace(val[1].Replace("Below ", "").Replace("+/-", "").Replace(":1", ""), "[^0-9.E-]", "")))) { System.Diagnostics.Debug.Write("<" + x + "> "); }
                    //System.Diagnostics.Debug.WriteLine("");
                    var //rawtmp = from val in raw.ElementAt(i).Value.Results select new ResultData(val.SerialNumber, val.StartTime, Convert.ToDouble(Regex.Replace(val.Result.Replace("Below ", "").Replace("+/-", "").Replace(":1", ""), "[^0-9.E-]", "")).ToString("G4", CultureInfo.InvariantCulture), val.ResultConv, val.LowLimit, val.UpLimit);
                    rawtmp = raw.ElementAt(i).Value.Results.Select(val => { val.Result = Convert.ToDouble(Regex.Replace(val.Result.Replace("Below ", "").Replace("+/-", "").Replace(":1", ""), "[^0-9.E-]", "")).ToString("G4", CultureInfo.InvariantCulture); return val; });
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
                    if (Convert.ToDouble(tmp.Cpk) == Double.PositiveInfinity)
                    {
                        tmp.Cpk = "---";
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
                                tmp.AllResults[z].ResultConv = c.ToString("G4", CultureInfo.InvariantCulture);
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
                                tmp.AllResults[z].ResultConv = c.ToString("G4", CultureInfo.InvariantCulture);
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
                                tmp.AllResults[z].ResultConv = c.ToString("G4", CultureInfo.InvariantCulture);
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
                                    tmp.AllResults[z].ResultConv = (Convert.ToDouble(tmp.AllResults[z].ResultConv) / 1000).ToString("G4", CultureInfo.InvariantCulture);
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
                                    tmp.AllResults[z].ResultConv = (Convert.ToDouble(tmp.AllResults[z].ResultConv) * 1000).ToString("G4", CultureInfo.InvariantCulture);
                                }
                                tmp.UnitConv = smallW[wIndex < 7 ? wIndex++ : wIndex];
                            }
                            else
                            {
                                x = 8;
                            }
                        }

                        for (int x = 0; x < tmp.AllResults.Count(); x++)
                        {
                            tmp.AllResults[x].ResultConv = (Math.Round(Convert.ToDouble(tmp.AllResults[x].ResultConv), rounding)).ToString("G4", CultureInfo.InvariantCulture);
                        }

                        tmp.MinResultConv = Math.Round(tempMin, rounding).ToString("G4", CultureInfo.InvariantCulture);
                        tmp.MaxResultConv = Math.Round(tempMax, rounding).ToString("G4", CultureInfo.InvariantCulture);
                        tmp.AvgResultConv = Math.Round(tempAvg, rounding).ToString("G4", CultureInfo.InvariantCulture);
                        tmp.StdDevConv = Math.Round(tempStd, rounding).ToString("G4", CultureInfo.InvariantCulture);
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

                conn.Close();
            }

            data.Sort(new Comparison<TestData>(Compare));
            return data;
        }

        public IHttpActionResult GetTableData(string modelName, string productType)
        {
            try
            {
                List<TestData> data = InternalGetTableData(modelName, productType);
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

        public IHttpActionResult GetTableFile(string modelName, string productType)
        {
            try
            {
                List<TestData> data = InternalGetTableData(modelName, productType);
                string[][] headers = new string[1][];
                headers[0] = new string[] { "Testname", "Channel", "Power", "Serial Number", "Start Time", "Result", "Min", "Max", "Average", "Std. Deviation", "Unit", "Result (Conv)", "Min (Conv)", "Max (Conv)", "Average (Conv)", "Std. Deviation (Conv)", "Unit (Conv)", "LowLimit", "UpLimit", "Cpk" };
                var file = new MemoryStream();
                var document = new XLWorkbook();
                var worksheet = document.Worksheets.Add("Table Data");
                worksheet.Cell(1, 1).InsertData(headers);
                //worksheet.Cell(2, 1).InsertData(data);
                var insertionIndex = 2;
                foreach (TestData test in data)
                {
                    worksheet.Cell(insertionIndex, 1).InsertData(new List<TestData>() { test });

                    for (int i = 0; i < test.AllResults.Count(); i++)
                    {
                        var serial = test.AllResults[i].SerialNumber;
                        var result = test.AllResults[i].Result;
                        var resultConv = test.AllResults[i].ResultConv;
                        var startTime = test.AllResults[i].StartTime;
                        var lowLimit = test.AllResults[i].LowLimit;
                        var upLimit = test.AllResults[i].UpLimit;

                        worksheet.Cell(insertionIndex, 4).SetValue(serial != "" ? serial : "---");
                        worksheet.Cell(insertionIndex, 5).SetValue(startTime != "" ? startTime : "---");
                        worksheet.Cell(insertionIndex, 6).SetValue(result != "" ? result : "---");
                        worksheet.Cell(insertionIndex, 12).SetValue(resultConv != "" ? resultConv : "---");
                        worksheet.Cell(insertionIndex, 18).SetValue(lowLimit != "" ? lowLimit : "---");
                        worksheet.Cell(insertionIndex, 19).SetValue(upLimit != "" ? upLimit : "---");
                        worksheet.Cell(insertionIndex, 21).SetValue("");

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
                document.SaveAs(file);
                string filename = DateTime.Now.ToString("yyyy-MM-dd") + $" {productType} {modelName}.xlsx";
                var resp = new ExcelFileResponse(file.ToArray(), Request, filename);
                file.Dispose();
                return resp;
            }
            catch (DataNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
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
