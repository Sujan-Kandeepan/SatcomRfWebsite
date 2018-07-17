using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SatcomRfWebsite.Models
{
    public class CalibrationData
    {
        public string AssetNumber { get; set; }
        public DateTime AddedDate { get; set; }
        public string EditedBy { get; set; }
        public List<CalibrationRecord> Records { get; set; }
    }

    public class ATCalibrationData : CalibrationData
    {
        public long StartFreq { get; set; }
        public long StopFreq { get; set; }
        public int Points { get; set; }
        public long Loss { get; set; }
        public long Power { get; set; }
        public double MaxOffset { get; set; }
        public double? Temp { get; set; }
        public double? Humidity { get; set; }
        public string Lookback { get; set; }
        public string Operator { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    public class OCCalibrationData : CalibrationData
    {
        public long StartFreq { get; set; }
        public long StopFreq { get; set; }
        public int Points { get; set; }
        public long Loss { get; set; }
        public long Power { get; set; }
        public double MaxOffset { get; set; }
        public double? Temp { get; set; }
        public double? Humidity { get; set; }
        public string Lookback { get; set; }
        public string Operator { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    public class PSCalibrationData : CalibrationData
    {
        public string Series { get; set; }
        public string Serial { get; set; }
        public string RefCal { get; set; }
        public string Certificate { get; set; }
        public string Operator { get; set; }
        public DateTime CalDate { get; set; }
    }

    public class CalibrationRecord
    {
        public double Frequency { get; set; }
        public double CalFactor { get; set; }
    }
}