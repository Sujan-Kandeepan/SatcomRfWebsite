using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SatcomRfWebsite.Models
{
    public class AteOutputModel
    {
        public IList<SatcomRfWebsite.Models.tblATEOutput> ateOutput { get; set; }

        private rfDbEntities db = new rfDbEntities();
        private ToolBox tools = new ToolBox();

        //constructor
        public AteOutputModel()
        {
            ateOutput = new List<SatcomRfWebsite.Models.tblATEOutput>();
        }

        public void generateAteOutput(string prodType, string modelName, string testType, string tubeName, string options)
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

            ateOutput.Clear();

            //populate ate output data
            foreach (var output in myQuery)
            {
                ateOutput.Add(output);
            }
        }
    }
}