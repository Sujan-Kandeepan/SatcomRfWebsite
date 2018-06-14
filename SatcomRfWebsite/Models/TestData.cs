using System.Collections.Generic;
using System;

namespace SatcomRfWebsite.Models
{
    public class TestData
    {
        public string TestName { get; set; }
        public string Channel { get; set; }
        public string Power { get; set; }
        public List<List<string>> AllResults { get; set; } // SN, Result, StartTime, LowLimit, UpLimit
        public string StartTimePlaceHolder { get; set; }
        public string ResultsPlaceHolder { get; set; }
        public string MinResult { get; set; }
        public string MaxResult { get; set; }
        public string AvgResult { get; set; }
        public string StdDev { get; set; }
        public string Unit { get; set; }
        public List<List<string>> AllResultsConv { get; set; } // SN, ResultConv
        public string MinResultConv { get; set; }
        public string MaxResultConv { get; set; }
        public string AvgResultConv { get; set; }
        public string StdDevConv { get; set; }
        public string UnitConv { get; set; }
        public string LowLimitPlaceHolder { get; set; }
        public string UpLimitPlaceHolder { get; set; }
        public string Cpk { get; set; }

        public TestData() { }
    }

    public class TestInfo
    {
        public string TestName { get; set; }
        public string Channel { get; set; }
        public string Power { get; set; }
        public string Units { get; set; }
        public List<List<string>> Results { get; set; }

        public TestInfo(string inTestName, string inChannel, string inPower, string inUnits, List<List<string>> inResults)
        {
            TestName = inTestName;
            Channel = inChannel;
            Power = inPower;
            Units = inUnits;
            Results = inResults;
        }
    }
}