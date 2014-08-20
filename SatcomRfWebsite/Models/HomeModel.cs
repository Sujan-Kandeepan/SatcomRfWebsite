using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SatcomRfWebsite.Models
{
    public class HomeModel
    {
        private ToolBox tools = new ToolBox();
        private rfDbEntities db = new rfDbEntities();

        public String getProductType { get; set; }
        public String getModelName { get; set; }
        public String getModelType { get; set; }
        public String getSerialNum { get; set; }
        public String getLastTestDate { get; set; }

        public IList<List<String>> getLastTestResults { get; set; }

        //Constructor
        public HomeModel(){

            Tuple<string, DateTime> lastModelTested = getLastTestModelSN();

            String tempSN = lastModelTested.Item1;
            DateTime tempDT = lastModelTested.Item2;
            getModelType = "N/A";

            getSerialNum = tempSN;
            getLastTestDate = tempDT.ToString();
            getProductType = tools.getProdTypeFromSN(tempSN);
            getModelName = tools.getModelNameFromSN(tempSN);
        }

        public List<string> convertDbResultToList(string testName, string result, string units, string lowLimit, string upLimit, bool passFail){

            List<string> tempList = new List<string>();

            tempList.Add(testName);
            tempList.Add(result);
            tempList.Add(units);
            tempList.Add(lowLimit);
            tempList.Add(upLimit);

            if(passFail){
                tempList.Add("Pass");
            }else{
                tempList.Add("Fail");
            }

            return new List<string>(tempList);
        }


        //find the last test performed. return SN, DT, TP
        public Tuple<String, DateTime> getLastTestModelSN()
        {
            DateTime lastDateTime = default(DateTime);
            String lastModelSN = "";

            var myQuery = (from ateOut in db.tblATEOutputs
                           orderby ateOut.StartTime descending
                           select new { ateOut.ModelSN, ateOut.StartTime }).Take(1);

            foreach (var twtResult in myQuery)
            {
                lastDateTime = twtResult.StartTime;
                lastModelSN = twtResult.ModelSN;
            }

            return new Tuple<String, DateTime>(lastModelSN, lastDateTime);
        }

        //find todays tests. return ProdType, ModName, TestTime
        public IEnumerable<Tuple<String, String, String, DateTime>> getTodayTests()
        {
            DateTime today = DateTime.Today;
            IList<Tuple<String, String, String, DateTime>> todaysTests = new List<Tuple<String, String, String, DateTime>>();


            var myQuery = from ateOut in db.tblATEOutputs
                          where (ateOut.StartTime.Day == today.Day && ateOut.StartTime.Month == today.Month && ateOut.StartTime.Year == today.Year)
                          orderby ateOut.StartTime descending
                          select new { ateOut.ModelSN, ateOut.StartTime };


            foreach (var ateOutput in myQuery)
            {
                todaysTests.Add(new Tuple<String, String, String, DateTime>(getProductType, getModelName, ateOutput.ModelSN, ateOutput.StartTime));
            }

            return todaysTests;
        }
    }
}