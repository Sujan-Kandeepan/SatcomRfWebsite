using System.Collections.Generic;
using System;

namespace SatcomRfWebsite.Models
{
    public class TestData
    {
        public string TestName { get; set; }
        public string Channel { get; set; }
        public string Power { get; set; }
        public List<ResultData> AllResults { get; set; }
        public string StartTimePlaceHolder { get; set; }
        public string ResultsPlaceHolder { get; set; }
        public string MinResult { get; set; }
        public string MaxResult { get; set; }
        public string AvgResult { get; set; }
        public string StdDev { get; set; }
        public string Unit { get; set; }
        public string ResultConvPlaceholder { get; set; }
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
        public List<ResultData> Results { get; set; }

        public TestInfo(string inTestName, string inChannel, string inPower, string inUnits, List<ResultData> inResults)
        {
            TestName = inTestName;
            Channel = inChannel;
            Power = inPower;
            Units = inUnits;
            Results = inResults;
        }
    }

    public class ResultData
    {
        public string SerialNumber { get; set; }
        public string StartTime { get; set; }
        public string Result { get; set; }
        public string ResultConv { get; set; }
        public string LowLimit { get; set; }
        public string UpLimit { get; set; }

        public ResultData(string inSerialNumber, string inStartTime, string inResult, string inResultConv, string inLowLimit, string inUpLimit)
        {
            SerialNumber = inSerialNumber;
            StartTime = inStartTime;
            Result = inResult;
            ResultConv = inResultConv;
            LowLimit = inLowLimit;
            UpLimit = inUpLimit;
        }
    }
}