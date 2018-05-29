//Note: ClosedXML requires DocumentFormat.OpenXML v:2.5 exactly (as of time of writing)
//TODO: Add comments maybe?
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                    return numlhs - numrhs;
                }
                else
                {
                    return lhs.Channel.CompareTo(rhs.Channel);
                }
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
        public List<TestData> InternalGetTableData(string modelName, string familyName)
        {
            var data = new List<TestData>();
            using (var conn = new SqlConnection(GetSqlConnectString()))
            {
                conn.Open();
                var cmd = new SqlCommand("", conn);
                List<string> sqlResultData = null;

                if (modelName == "all")
                {
                    cmd.CommandText = "SELECT ModelName FROM dbo.tblModelNames WHERE ProductType = @family;";
                    var familyParam = new SqlParameter("@family", SqlDbType.NVarChar, 10);
                    familyParam.Value = familyName;
                    cmd.Parameters.Add(familyParam);
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

                if (familyName.ToUpper().Contains("GENIV"))
                {
                    cmd.CommandText = "SELECT TestName,Result,Units,Channel FROM dbo.tblKLYTestResults WHERE ModelSn = @sn AND NOT Result = 'PASS';";
                }
                else
                {
                    cmd.CommandText = "SELECT TestName,Result,Units,Channel FROM dbo.tblTWTTestResults WHERE ModelSn = @sn AND NOT Result = 'PASS';";
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
                        var tinfo = new TestInfo(tmp2["TestName"].ToString(), tmp2["Channel"].ToString(),
                            tmp2["Units"].ToString(), new string[] { tmp2["Result"].ToString() });
                        var key = tmp2["TestName"].ToString() + tmp2["Channel"].ToString();

                        if (raw.ContainsKey(key))
                        {
                            raw[key].Results.Add(tmp2["Result"].ToString());
                        }
                        else
                        {
                            raw.Add(key, tinfo);
                        }
                    }
                    sqlResult2.Close();
                }

                cmd.Dispose();
                for (var i = 0; i < raw.Count(); i++)
                {
                    if (i == raw.Count() - 1 || i < raw.Count() - 1 && !(raw.ElementAt(i).Value.TestName == raw.ElementAt(i + 1).Value.TestName && raw.ElementAt(i).Value.Channel == raw.ElementAt(i + 1).Value.Channel)) {

                        var longest = raw.ElementAt(i).Value.Results.OrderByDescending(x => x.Length).First();
                        int rounding = 15;
                        if (longest.IndexOf(".") != -1)
                        {
                            rounding = longest.Length - longest.IndexOf(".") - 1;
                        }
                        var tmp = new TestData();
                        var rawtmp = from val in raw.ElementAt(i).Value.Results select Convert.ToDouble(val.Replace(":1", ""));
                        tmp.TestName = raw.ElementAt(i).Value.TestName;
                        tmp.Unit = raw.ElementAt(i).Value.Units;
                        tmp.Channel = (string.IsNullOrEmpty(raw.ElementAt(i).Value.Channel) ? "N/A" : raw.ElementAt(i).Value.Channel);
                        tmp.MinResult = Convert.ToString(rawtmp.Min());
                        tmp.MaxResult = Convert.ToString(rawtmp.Max());
                        tmp.AvgResult = Convert.ToString(Math.Round(rawtmp.Average(), rounding));

                        var tempSum = 0.0;
                        foreach (var item in rawtmp)
                        {
                            tempSum += Math.Pow(item - rawtmp.Average(), 2);
                        }
                        tmp.StdDev = Convert.ToString(Math.Round(Math.Sqrt(tempSum / rawtmp.Count()), rounding));

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
                                tempMin = Math.Pow(10, rawtmp.Min() / 10);
                                tempMax = Math.Pow(10, rawtmp.Max() / 10);
                                tempAvg = Math.Pow(10, rawtmp.Average() / 10);

                                tempSum2 = 0.0;
                                foreach (var item in rawtmp)
                                {
                                    double c = Math.Pow(10, Convert.ToDouble(item) / 10);
                                    tempSum2 += Math.Pow(c - tempAvg, 2);
                                }
                                tempStd = Math.Sqrt(tempSum2 / rawtmp.Count());

                                break;
                            case "dBm":
                                tempMin = Math.Pow(10, (rawtmp.Min() - 30) / 10);
                                tempMax = Math.Pow(10, (rawtmp.Max() - 30) / 10);
                                tempAvg = Math.Pow(10, (rawtmp.Average() - 30) / 10);

                                tempSum2 = 0.0;
                                foreach (var item in rawtmp)
                                {
                                    double c = Math.Pow(10, (Convert.ToDouble(item) - 30) / 10);
                                    tempSum2 += Math.Pow(c - tempAvg, 2);
                                }
                                tempStd = Math.Sqrt(tempSum2 / rawtmp.Count());

                                break;
                            case "deg/dB":
                            case "o/dB":
                                tempMin = 1 / Math.Pow(10, 1 / rawtmp.Min() / 10);
                                tempMax = 1 / Math.Pow(10, 1 / rawtmp.Max() / 10);
                                tempAvg = 1 / Math.Pow(10, 1 / rawtmp.Average() / 10);

                                tempSum2 = 0.0;
                                foreach (var item in rawtmp)
                                {
                                    double c = 1 / Math.Pow(10, 1 / Convert.ToDouble(item) / 10);
                                    tempSum2 += Math.Pow(c - tempAvg, 2);
                                }
                                tempStd = Math.Sqrt(tempSum2 / rawtmp.Count());

                                break;
                        }
                        
                        if (tmp.Unit.Contains("dB"))
                        {
                            tmp.UnitConv = "W";

                            /*wIndex = 0;
                            for (int x = 0; x < 8; i++)
                            {
                                if (tempAvg > 10000)
                                {
                                    tempMin /= 1000;
                                    tempMax /= 1000;
                                    tempAvg /= 1000;
                                    tempStd /= 1000;
                                    tmp.UnitConv = largeW[wIndex < 7 ? wIndex++ : wIndex];
                                }
                                else if (tempMin < 1 || Math.Round(tempMin, rounding) == 0 || Math.Round(tempStd, rounding) == 0)
                                {
                                    tempMin *= 1000;
                                    tempMax *= 1000;
                                    tempAvg *= 1000;
                                    tempStd *= 1000;
                                    tmp.UnitConv = smallW[wIndex < 7 ? wIndex++ : wIndex];
                                }
                                else
                                {
                                    break;
                                }
                            }*/

                            tmp.MinResultConv = Convert.ToString(Math.Round(tempMin, rounding));
                            tmp.MaxResultConv = Convert.ToString(Math.Round(tempMax, rounding));
                            tmp.AvgResultConv = Convert.ToString(Math.Round(tempAvg, rounding));
                            tmp.StdDevConv = Convert.ToString(Math.Round(tempStd, rounding));
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

                        data.Add(tmp);
                    }
                }

                conn.Close();
            }

            data.Sort(new Comparison<TestData>(Compare));
            return data;
        }

        public IHttpActionResult GetTableData(string modelName, string familyName)
        {
            try
            {
                List<TestData> data = InternalGetTableData(modelName, familyName);
                return Ok(data);
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

        public IHttpActionResult GetTableFile(string modelName, string familyName)
        {
            try
            {
                List<TestData> data = InternalGetTableData(modelName, familyName);
                string[][] headers = new string[1][];
                headers[0] = new string[] { "Testname", "Channel #", "Min", "Max", "Average", "Std. Deviation", "Unit", "Min (Conv)", "Max (Conv)", "Average (Conv)", "Std. Deviation (Conv)", "Unit (Conv)" };
                var file = new MemoryStream();
                var document = new XLWorkbook();
                var worksheet = document.Worksheets.Add("Table Data");
                worksheet.Cell(1, 1).InsertData(headers);
                worksheet.Cell(2, 1).InsertData(data);
                var style = document.Style;
                style.Font.Bold = true;
                worksheet.Range(1, 1, 1, 12).Style = style;
                worksheet.Columns().AdjustToContents();
                document.SaveAs(file);
                string filename = DateTime.Now.ToString("yyyy-MM-dd") + $" {familyName} {modelName}.xlsx";
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

        public IHttpActionResult GetModels(string familyName)
        {
            List<string> data = null;
            using (var conn = new SqlConnection(GetSqlConnectString()))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ModelName FROM dbo.tblModelNames WHERE ProductType = @name;", conn);
                var nameParam = new SqlParameter("@name", SqlDbType.NVarChar, 100);
                nameParam.Value = familyName;
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
                }

                cmd.Dispose();
                conn.Close();
            }

            return Ok(data);
        }

        public IHttpActionResult GetFamilies()
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
                }

                cmd.Dispose();
                conn.Close();
            }

            return Ok(data);
        }
    }
}
