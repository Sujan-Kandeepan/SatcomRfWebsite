using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

using SatcomRfWebsite.Models;

namespace SatcomRfWebsite.Controllers
{
    public class ListAPIController : ApiController
    {
        [NonAction]
        private int Compare(TestData lhs, TestData rhs)
        {
            if(lhs.TestName == rhs.TestName && lhs.Channel != "" && rhs.Channel != "")
            {
                return Convert.ToInt32(lhs.Channel) - Convert.ToInt32(rhs.Channel);
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
            while(sql.Read())
            {
                var tmp = (IDataRecord)sql;
                result.Add(tmp[column].ToString());
            }
            return result;
        }

        public IHttpActionResult GetTableData(string modelName, string familyName)
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
                        return NotFound();
                    }
                    
                    List<string> sqlResultModelData = ReadAllData(sqlResultModels, "ModelName");
                    sqlResultModels.Close();
                    cmd.CommandText = "SELECT ModelSN FROM dbo.tblSerialNumbers WHERE ModelName = @name;";
                    var modelParam = new SqlParameter("@name", SqlDbType.NVarChar, 30);
                    cmd.Parameters.Add(modelParam);
                    cmd.Prepare();
                    sqlResultData = new List<string>();

                    foreach(string i in sqlResultModelData)
                    {
                        modelParam.Value = i;
                        SqlDataReader sqlResultTmp = cmd.ExecuteReader();

                        if (!sqlResultTmp.HasRows)
                        {
                            return NotFound();
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
                        return NotFound();
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
                foreach (var i in raw)
                {
                    var tmp = new TestData();
                    var rawtmp = from val in i.Value.Results select Convert.ToDouble(val.Replace(":1",""));
                    tmp.TestName = i.Value.TestName;
                    tmp.Unit = i.Value.Units;
                    tmp.Channel = i.Value.Channel;
                    tmp.MinResult = Convert.ToString(rawtmp.Min());
                    tmp.MaxResult = Convert.ToString(rawtmp.Max());
                    tmp.AvgResult = Convert.ToString(Math.Round(rawtmp.Average(), 2));
                    data.Add(tmp);
                }

                conn.Close();
            }

            data.Sort(new Comparison<TestData>(Compare));
            return Ok(data);
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

                if(!sqlResult.HasRows)
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
