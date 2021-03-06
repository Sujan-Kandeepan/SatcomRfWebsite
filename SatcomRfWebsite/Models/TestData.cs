﻿using System.Collections.Generic;
using System;

namespace SatcomRfWebsite.Models
{
    public class TestData
    {
        public string TestName { get; set; }
        public string Channel { get; set; }
        public string Frequency { get; set; }
        public string Power { get; set; }
        public List<ResultData> AllResults { get; set; }
        public string MinResult { get; set; }
        public string MaxResult { get; set; }
        public string AvgResult { get; set; }
        public string StdDev { get; set; }
        public string Unit { get; set; }
        public string MinResultConv { get; set; }
        public string MaxResultConv { get; set; }
        public string AvgResultConv { get; set; }
        public string StdDevConv { get; set; }
        public string UnitConv { get; set; }
        public string Cpk { get; set; }
        public string ParsableResults { get; set; }

        public TestData() { }
    }

    public class TestInfo
    {
        public string TestName { get; set; }
        public string Channel { get; set; }
        public string Frequency { get; set; }
        public string Power { get; set; }
        public string Units { get; set; }
        public List<ResultData> Results { get; set; }

        public TestInfo(string inTestName, string inChannel, string inFrequency, string inPower, string inUnits, List<ResultData> inResults)
        {
            TestName = inTestName;
            Channel = inChannel;
            Frequency = inFrequency;
            Power = inPower;
            Units = inUnits;
            Results = inResults;
        }
    }

    public class ResultData
    {
        public string SerialNumber { get; set; }
        public string TestType { get; set; }
        public string StartTime { get; set; }
        public string Result { get; set; }
        public string ResultConv { get; set; }
        public string LowLimit { get; set; }
        public string UpLimit { get; set; }
        public string Audit { get; set; }
        public string Itar { get; set; }
        public string LongModelName { get; set; }
        public string TubeSN { get; set; }
        public string TubeName { get; set; }
        public string SsaSN { get; set; }
        public string LinSN { get; set; }
        public string LipaSN { get; set; }
        public string BucSN { get; set; }
        public string BipaSN { get; set; }
        public string BlipaSN { get; set; }

        public ResultData(string inSerialNumber, string inTestType, string inStartTime, string inResult, string inResultConv, 
            string inLowLimit, string inUpLimit, string inAudit, string inItar, string inLongModelname, string inTubeSN, string inTubeName,
            string inSsaSN, string inLinSN, string inLipaSN, string inBucSN, string inBipaSN, string inBlipaSN)
        {
            SerialNumber = inSerialNumber;
            TestType = inTestType;
            StartTime = inStartTime;
            Result = inResult;
            ResultConv = inResultConv;
            LowLimit = inLowLimit;
            UpLimit = inUpLimit;
            Audit = inAudit;
            Itar = inItar;
            LongModelName = inLongModelname;
            TubeSN = inTubeSN;
            TubeName = inTubeName;
            SsaSN = inSsaSN;
            LinSN = inLinSN;
            LipaSN = inLipaSN;
            BucSN = inBucSN;
            BipaSN = inBipaSN;
            BlipaSN = inBlipaSN;
        }

        public override string ToString()
        {
            return String.Join(",", new List<string>() { SerialNumber, TestType, StartTime, Result, ResultConv, LowLimit, UpLimit,
                Audit, Itar, LongModelName, TubeSN, TubeName, SsaSN, LinSN, LipaSN, BucSN, BipaSN, BlipaSN } );
        }
    }
}