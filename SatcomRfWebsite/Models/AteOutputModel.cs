﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace SatcomRfWebsite.Models
{
    public class AteOutputModel
    {
        public IList<SatcomRfWebsite.Models.tblATEOutput> ateOutput { get; set; }
        public IList<ToolBox.AteOutputCustom> ateOutputList { get; set; }
        public IList<SatcomRfWebsite.Models.tblProductType> productTypeList { get; set; }
        public IList<SatcomRfWebsite.Models.tblModelName> modelNameList { get; set; }
        public IList<SatcomRfWebsite.Models.tblSerialNumber> serialNumList { get; set; }

        public List<TestResults> testResults { get; set; }
        public List<TestParams> testParams { get; set; }

        public string prodTypeStrList { get; set; }
        public string modelNameStrList { get; set; }
        public string serialNumStrList { get; set; }

        public string filter { get; set; }
        public string productType { get; set; }
        public string modelName { get; set; }
        public string serialNum { get; set; }
        public string testType { get; set; }
        public string tubeName { get; set; }
        public string options { get; set; }
        public List<string> prodList { get; set; }
        public List<string> modelList { get; set; }
        public List<string> serList { get; set; }
        public List<string> tubeList { get; set; }
        

        private rfDbEntities db = new rfDbEntities();
        private ToolBox tools = new ToolBox();

        //constructor
        public AteOutputModel()
        {
            ateOutput = new List<SatcomRfWebsite.Models.tblATEOutput>();
            ateOutputList = new List<ToolBox.AteOutputCustom>();
            productTypeList = new List<SatcomRfWebsite.Models.tblProductType>();
            modelNameList = new List<SatcomRfWebsite.Models.tblModelName>();
            serialNumList = new List<SatcomRfWebsite.Models.tblSerialNumber>();

            testResults = new List<TestResults>();
            testParams = new List<TestParams>();

            prodTypeStrList = "";
            modelNameStrList = "";
            serialNumStrList = "";

            filter = "";
            productType = "";
            modelName = "";
            serialNum = "";
            testType = "";
            tubeName = "";
            options = "";
            prodList = new List<string>();
            modelList = new List<string>();
            serList = new List<string>();
            tubeList = new List<string>();
            
        }

        public void GenerateAteOutput()
        {
            // if nothing is selected don't perform any queries
            if ((productType == "" || productType == "na") && 
                (modelName == "" || modelName == "na") && 
                (serialNum == "" || serialNum == "na") &&
                (testType == "" || testType == "na") && 
                (tubeName == "" || tubeName == "na") && 
                (options == "" || options == "na"))
            {
                return;
            }

            string dbTestType = "Production";

            switch (testType)
            {
                case "ProdTest": dbTestType = "Production"; break;
                case "EngTest": dbTestType = "Engineering"; break;
                case "Debug": dbTestType = "Debugging"; break;
            }

            var myQuery = (from ateOut in db.tblATEOutputs
                           join serNums in db.tblSerialNumbers on ateOut.ModelSN equals serNums.ModelSN
                           join modNames in db.tblModelNames on serNums.ModelName equals modNames.ModelName
                           orderby ateOut.StartTime ascending
                           select new { modNames.ModelName,
                                        modNames.ProductType,
                                        ateOut.ModelSN,
                                        ateOut.StartTime,
                                        ateOut.TestType,
                                        ateOut.Audit,
                                        ateOut.LongModelName,
                                        ateOut.TubeName,
                                        ateOut.SsaSN,
                                        ateOut.LinSN,
                                        ateOut.LipaSN,
                                        ateOut.BucSN,
                                        ateOut.BipaSN,
                                        ateOut.BlipaSN } );

            var counter = myQuery.Count();

            if (productType.Length > 3)
            {
                myQuery = myQuery.Where(modName => modName.ProductType.Equals(productType));
            }
            counter = myQuery.Count();
            if (modelName.Length > 3)
            {
                myQuery = myQuery.Where(modName => modName.ModelName.Equals(modelName));
            }
            counter = myQuery.Count();
            if (serialNum.Length > 3)
            {
                myQuery = myQuery.Where(ateOut => ateOut.ModelSN.Equals(serialNum));
            }
            counter = myQuery.Count();
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
            counter = myQuery.Count();
            if (tubeName.Length > 3 && !tubeName.Equals("none"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.TubeName.Equals(tubeName));
            }
            counter = myQuery.Count();
            if (options.Contains("Audit"))
            {
                myQuery = myQuery.Where(ateOut => ateOut.Audit == true);
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

            ateOutputList.Clear();
            counter = myQuery.Count();

            //populate ate output data
            foreach (var output in myQuery)
            {
                ateOutputList.Add(new ToolBox.AteOutputCustom(output.ModelName, output.ProductType, output.ModelSN, output.StartTime, output.TestType, output.Audit,
                                                                output.LongModelName, output.TubeName, output.SsaSN, output.LinSN, output.LipaSN, output.BucSN,
                                                                    output.BipaSN, output.BlipaSN));
            }
        }

        /*
         * - Generate list of product types, model names, serial nums and tube names that correspond to parsed filter
         */
        public void GenerateLists()
        {
            var myQuery = (from prodTypes in db.tblProductTypes
                            join modNames in db.tblModelNames on prodTypes.ProductType equals modNames.ProductType
                            join serNums in db.tblSerialNumbers on modNames.ModelName equals serNums.ModelName
                            join ateOut in db.tblATEOutputs on serNums.ModelSN equals ateOut.ModelSN
                            orderby modNames.ModelName ascending
                            select new { modNames.ProductType,
                                         modNames.ModelName,
                                         ateOut.ModelSN,
                                         ateOut.TubeName });

            if (this.productType != "" && this.productType != "na")
            {
                myQuery = myQuery.Where(p => p.ProductType.Equals(this.productType));
            }
            
            if (this.modelName != "" && this.modelName != "na")
            {
                myQuery = myQuery.Where(m => m.ModelName.Equals(this.modelName));
            }

            if (this.serialNum != "" && this.serialNum != "na")
            {
                myQuery = myQuery.Where(s => s.ModelSN.Equals(this.serialNum));
            }

            if (this.tubeName != "" && this.tubeName != "na")
            {
                myQuery = myQuery.Where(t => t.TubeName.Equals(this.tubeName));
            }

            string charSeperator = "";
            prodTypeStrList = "";
            modelNameStrList = "";
            serialNumStrList = "";

            foreach (var item in myQuery)
            {
                var list = prodList;
                AddToList(ref list, item.ProductType);
                prodList = list;

                list = modelList;
                AddToList(ref list, item.ModelName);
                modelList = list;

                list = serList;
                AddToList(ref list, item.ModelSN);
                serList = list;

                list = tubeList;
                AddToList(ref list, item.TubeName);
                tubeList = list;

                prodTypeStrList += charSeperator + "'" + item.ProductType + "'";
                modelNameStrList += charSeperator + "'" + item.ModelName + "'";
                serialNumStrList += charSeperator + "'" + item.ModelSN + "'";
                charSeperator = ",";
            }
        }

        private void AddToList(ref List<string> myList, string item){

            myList.Sort();
            int index = myList.BinarySearch(item);

            if (index < 0)
            {
                //From MSDN site:
                //Taking the bitwise complement (the ~ operator in C# and Visual C++, Xor -1 in Visual Basic) of this negative number 
                // produces the index of the first element in the list that is larger than the search string, and inserting at this location 
                // preserves the sort order.
                myList.Insert(~index, item);
            }
        }

        /*
         * 
         */
        public void ParseFilter(string filter)
        {
            //check for correct filter format
            Match match = Regex.Match(filter, @"(pT\=)([a-zA-Z0-9]*)(%mN\=)([a-zA-Z0-9-]*)(%ser\=)([a-zA-Z0-9_-]*)(%testType\=)([a-zA-Z0-9]*)(%tubeName\=)([a-zA-Z0-9-]*)(%opt\=)([a-zA-Z0-9,]*)");
            if (!match.Success)
            {
                filter = "";
                return;
            }

            match = Regex.Match(filter, @"(?<=pT\=)([a-zA-Z0-9]*)");

            if (match.Success)
            {
                productType = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=mN\=)([a-zA-Z0-9-]*)");

            if (match.Success)
            {
                modelName = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=ser\=)([a-zA-Z0-9_-]*)");

            if (match.Success)
            {
                serialNum = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=testType\=)([a-zA-Z]*)");

            if (match.Success)
            {
                testType = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=tubeName\=)([a-zA-Z0-9-]*)");

            if (match.Success)
            {
                tubeName = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=opt\=)([a-zA-Z0-9,]*)");

            if (match.Success)
            {
                options = match.Groups[0].Value;
            }
        }

        public bool GenerateAteOutputDetail()
        {
            //check for valid serialNum
            if (this.serialNum == "" || !tools.isSerialNumberInDB(this.serialNum))
            {
                return false;
            }

            ateOutput = db.tblATEOutputs.Where(x => x.ModelSN.Equals(this.serialNum)).OrderBy(x => x.StartTime).ToList();

            ToolBox.ProductClass prodClass = new ToolBox.ProductClass(this.productType);

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

            //populate "testResults" list with all the test for chosen serial

            if (prodClass.isTWT)
            {
                var qTests = db.tblTWTTestResults.Where(x => x.ModelSN.Equals(this.serialNum));
                foreach (var test in qTests)
                {
                    testResults.Add(new TestResults(test.id, test.ModelSN, test.StartTime, test.TestName, Regex.Replace(test.TestName, @"[\d-]", ""), test.Result, test.Units, test.LowLimit, test.UpLimit, test.PassFail, test.Channel, test.P1, test.P2, test.P3, test.P4, test.P5, test.P6, test.P7, test.P8, test.P9));
                }
            }
            else if (prodClass.isKLY)
            {
                var qTests = db.tblKLYTestResults.Where(x => x.ModelSN.Equals(this.serialNum));
                foreach (var test in qTests)
                {
                    testResults.Add(new TestResults(test.id, test.ModelSN, test.StartTime, test.TestName, Regex.Replace(test.TestName, @"[\d-]", ""), test.Result, test.Units, test.LowLimit, test.UpLimit, test.PassFail, test.Channel, test.P1, test.P2, test.P3, test.P4, test.P5, test.P6, test.P7, test.P8, test.P9));
                }
            }
            else if (prodClass.isSSPA)
            {
                var qTests = db.tblSSPATestResults.Where(x => x.ModelSN.Equals(this.serialNum));
                foreach (var test in qTests)
                {
                    testResults.Add(new TestResults(test.id, test.ModelSN, test.StartTime, test.TestName, Regex.Replace(test.TestName, @"[\d-]", ""), test.Result, test.Units, test.LowLimit, test.UpLimit, test.PassFail, test.Channel, test.P1, test.P2, test.P3, test.P4, test.P5, test.P6, test.P7, test.P8, test.P9));
                }
            }

            //it is necessary to order this list by "TestName" since we want to group tests together so they can be placed in one table
            testResults = testResults.OrderBy(x => x.TestName).ToList();

            return true;
        }
    }
}