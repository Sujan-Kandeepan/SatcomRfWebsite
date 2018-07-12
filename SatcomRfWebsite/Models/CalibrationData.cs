using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SatcomRfWebsite.Models
{
    public class CalibrationData
    {
        public string AssetNumber { get; set; }
        public string AddedDate { get; set; }
        public string EditedBy { get; set; }
        public List<CalibrationRecord> Records { get; set; }
    }

    public class OCCalibrationData : CalibrationData
    {
        public string StartFreq { get; set; }
        public string StopFreq { get; set; }
        public string Points { get; set; }
        public string Loss { get; set; }
        public string Power { get; set; }
        public string MaxOffset { get; set; }
        public string Temp { get; set; }
        public string Humidity { get; set; }
        public string Lookback { get; set; }
        public string Operator { get; set; }
        public string ExpireDate { get; set; }
    }

    public class PSCalibrationData : CalibrationData
    {
        public string Series { get; set; }
        public string Serial { get; set; }
        public string RefCal { get; set; }
        public string Certificate { get; set; }
        public string Operator { get; set; }
        public string CalDate { get; set; }
    }

    public class ATCalibrationData : CalibrationData
    {
        public string StartFreq { get; set; }
        public string StopFreq { get; set; }
        public string Points { get; set; }
        public string Loss { get; set; }
        public string Power { get; set; }
        public string MaxOffset { get; set; }
        public string Temp { get; set; }
        public string Humidity { get; set; }
        public string Lookback { get; set; }
        public string Operator { get; set; }
        public string ExpireDate { get; set; }
    }

    public class CalibrationRecord
    {
        public string Frequency { get; set; }
        public string CalFactor { get; set; }
    }
}