//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SatcomRfWebsite.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOCCalHeaders
    {
        public long id { get; set; }
        public string AssetNumber { get; set; }
        public long StartFreq { get; set; }
        public long StopFreq { get; set; }
        public int Points { get; set; }
        public long Loss { get; set; }
        public long Power { get; set; }
        public double MaxOffset { get; set; }
        public Nullable<double> Temp { get; set; }
        public Nullable<double> Humidity { get; set; }
        public string Lookback { get; set; }
        public string Operator { get; set; }
        public System.DateTime CalDate { get; set; }
        public System.DateTime ExpireDate { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string EditedBy { get; set; }
    }
}
