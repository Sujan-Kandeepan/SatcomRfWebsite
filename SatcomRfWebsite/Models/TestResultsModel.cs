using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SatcomRfWebsite.Models
{
    public class TestResultsModel
    {
        public List<TestResults> testResults { get; set; }
        public List<TestParams> testParams { get; set; }

        private rfDbEntities db = new rfDbEntities();
        private ToolBox tools = new ToolBox();


        //constructor
        public TestResultsModel()
        {
            testResults = new List<TestResults>();
            testParams = new List<TestParams>();
        }

        public void generateTestsResults(string prodType, string modelName, string testType, string tubeName, string options)
        {
            if (tools.isModelNameItar(modelName)) { return; }

            string dbTestType = "Production";

            switch (testType)
            {
                case "ProdTest": dbTestType = "Production"; break;
                case "EngTest": dbTestType = "Engineering"; break;
                case "Debug": dbTestType = "Debugging"; break;
            }

            var myQuery = (from ateOut in db.tblATEOutputs
                           join serialNums in db.tblSerialNumbers on ateOut.ModelSN equals serialNums.ModelSN
                           where serialNums.ModelName.Equals(modelName)
                           orderby ateOut.StartTime ascending
                           select ateOut);

            if (testType.Equals("") || testType.Equals("none"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.TestType.Contains("Production") ||
                                                    ateOut.TestType.Contains("Engineering") ||
                                                    ateOut.TestType.Contains("Debugging"));
            }
            else
            {
                myQuery = myQuery.Where(ateOut => ateOut.TestType.Contains(dbTestType));
            }


            if (tubeName.Count() > 3 && !tubeName.Equals("none"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.TubeName.Equals(tubeName));
            }


            if (options.Contains("SsaSN"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.SsaSN.Contains(""));
            }
            if (options.Contains("LinSN"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.LinSN.Contains(""));
            }
            if (options.Contains("LipaSN"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.LipaSN.Contains(""));
            }
            if (options.Contains("BucSN"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.BucSN.Contains(""));
            }
            if (options.Contains("BipaSN"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.BipaSN.Contains(""));
            }
            if (options.Contains("BlipaSN"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.BlipaSN.Contains(""));
            }

            ToolBox.ProductClass prodClass = new ToolBox.ProductClass(prodType);

            //getting params which will be linked with test results
            if (prodClass.isTWT)
            {
                var qParams = db.tblTWTTestParameters.ToList();
                foreach (var param in qParams)
                {
                    testParams.Add(new TestParams(param.id, param.TestName, param.P1, param.P2, param.P3, param.P4, param.P5, param.P6, param.P7, param.P8, param.P9));
                }
            }
            else if (prodClass.isKLY)
            {
                var qParams = db.tblKLYTestParameters.ToList();
                foreach (var param in qParams)
                {
                    testParams.Add(new TestParams(param.id, param.TestName, param.P1, param.P2, param.P3, param.P4, param.P5, param.P6, param.P7, param.P8, param.P9));
                }
            }
            else if (prodClass.isSSPA)
            {
                var qParams = db.tblSSPATestParameters.ToList();
                foreach (var param in qParams)
                {
                    testParams.Add(new TestParams(param.id, param.TestName, param.P1, param.P2, param.P3, param.P4, param.P5, param.P6, param.P7, param.P8, param.P9));
                }
            }

            //populate "testResults" list with all the test for chosen model name
            foreach (var output in myQuery)
            {
                if (prodClass.isTWT)
                {
                    var qTests = db.tblTWTTestResults.Where(x => x.ModelSN.Equals(output.ModelSN) && x.StartTime.Equals(output.StartTime));
                    foreach (var test in qTests)
                    {
                        testResults.Add(new TestResults(test.id, test.ModelSN, test.StartTime, test.TestName, Regex.Replace(test.TestName, @"[\d-]", ""), test.Result, test.Units, test.LowLimit, test.UpLimit, test.PassFail, test.Channel, test.P1, test.P2, test.P3, test.P4, test.P5, test.P6, test.P7, test.P8, test.P9));
                    }
                }
                else if (prodClass.isKLY)
                {
                    var qTests = db.tblKLYTestResults.Where(x => x.ModelSN.Equals(output.ModelSN) && x.StartTime.Equals(output.StartTime));
                    foreach (var test in qTests)
                    {
                        testResults.Add(new TestResults(test.id, test.ModelSN, test.StartTime, test.TestName, Regex.Replace(test.TestName, @"[\d-]", ""), test.Result, test.Units, test.LowLimit, test.UpLimit, test.PassFail, test.Channel, test.P1, test.P2, test.P3, test.P4, test.P5, test.P6, test.P7, test.P8, test.P9));
                    }
                }
                else if (prodClass.isSSPA)
                {
                    var qTests = db.tblSSPATestResults.Where(x => x.ModelSN.Equals(output.ModelSN) && x.StartTime.Equals(output.StartTime));
                    foreach (var test in qTests)
                    {
                        testResults.Add(new TestResults(test.id, test.ModelSN, test.StartTime, test.TestName, Regex.Replace(test.TestName, @"[\d-]", ""), test.Result, test.Units, test.LowLimit, test.UpLimit, test.PassFail, test.Channel, test.P1, test.P2, test.P3, test.P4, test.P5, test.P6, test.P7, test.P8, test.P9));
                    }
                }
            }

            //it is necessary to order this list by "TestName" since we want to group tests together so they can be placed in one table
            testResults = testResults.OrderBy(x => x.TestName).ToList();
        }
    }
}