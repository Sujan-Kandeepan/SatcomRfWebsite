using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SatcomRfWebsite.Models
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
    }

    public class ToolBox
    {
        private rfDbEntities db = new rfDbEntities();
        private CommonDataValues commDataValues = new CommonDataValues();

        public String getProdTypeFromSN(string modelSN)
        {
            string productType = "pt";
            var myQuery = (from tblSN in db.tblSerialNumbers
                            join tblMN in db.tblModelNames on tblSN.ModelName equals tblMN.ModelName
                            where tblSN.ModelSN.Equals(modelSN)
                            select new { tblMN.ProductType, tblMN.ModelName }).Take(1);

            foreach (var pt in myQuery)
            {
                productType = pt.ProductType;
            }

            return productType;
        }

        public String getModelNameFromSN(string modelSN)
        {
            string modelName = "mn";
            var myQuery = (from tblSN in db.tblSerialNumbers
                           join tblMN in db.tblModelNames on tblSN.ModelName equals tblMN.ModelName
                           where tblSN.ModelSN.Equals(modelSN)
                           select new { tblMN.ProductType, tblMN.ModelName }).Take(1);

            foreach (var mn in myQuery)
            {
                modelName = mn.ModelName;
            }

            return modelName;
        }

        public bool isModelNameItar(string modelName)
        {
            if (modelName.Equals("TL25XJ") || 
                modelName.Equals("TL25XJ-179") || 
                modelName.Equals("TL25XI-1A") || 
                modelName.Equals("TL25XI-2A") ||

                modelName.Equals("VZX-6984A4") || 
                modelName.Equals("VZM-6993J1") || 
                modelName.Equals("VZM-6993J2") || 
                modelName.Equals("VZM-6993-J3") || 
                modelName.Equals("VZM-6993J4") || 
                modelName.Equals("VZM-6993J5") || 
                modelName.Equals("VZS/X2776L1") || 
                modelName.Equals("VZL-6943J1") || modelName.Equals("VZL-6943J2") || 
                modelName.Equals("VZX-6986J2M") || 
                modelName.Equals("VZX-6986J2E") || 
                modelName.Equals("VZS/C-6963J1") || modelName.Equals("VZS/C-6963J2") || 
                modelName.Equals("VZK-6901J1") || 
                modelName.Equals("VZA-6902J1") || 
                modelName.Equals("VZL-6941K4") || modelName.Equals("VZL-6941K6") || 
                modelName.Equals("VZS-6951K4") || modelName.Equals("VZS-6951K6") || 
                modelName.Equals("VZC-6961K4") || modelName.Equals("VZC-6961K6") || 
                modelName.Equals("VZX-6981K4") || modelName.Equals("VZX-6981K6") || 
                modelName.Equals("VZM-6991K4") || modelName.Equals("VZM-6991K6") || 
                modelName.Equals("VZU-6991K4") || modelName.Equals("VZU-6991K6") || 
                modelName.Equals("VZV-2776K4") || modelName.Equals("VZV-2776K6") || 
                modelName.Equals("VZX-6986J4") || 
                modelName.Equals("VZX-6986J5") || 
                modelName.Equals("VZX-6987V7") ||
                modelName.Equals("VZX6987V7") || 

                modelName.Equals("TL22XI") || 
                modelName.Equals("TL25XI") || 
                modelName.Equals("T01TO") || 
                modelName.Equals("TM04CO") || modelName.Equals("T04CO-C") || 
                modelName.Equals("TM04UO") || modelName.Equals("T04UO-E") || modelName.Equals("T04UO-F") || 
                modelName.Equals("T04XO") || 
                modelName.Equals("VZSC2780C2") || 
                modelName.Equals("VZM2780C2") || 
                modelName.Equals("VZC-6964VM") || 
                modelName.Equals("TM01KO-B") || modelName.Equals("T01KO-B") || 
                modelName.Equals("TM02KO-B") || modelName.Equals("T02KO-B") || 
                modelName.Equals("TL25XJ") || 
                modelName.Equals("TL06TO") || 

                modelName.Equals("TLO5XO") || 
                modelName.Equals("T04TUO") || 
                modelName.Equals("VZK-2790J1") || 
                modelName.Equals("VZA-2790J1") || 
                modelName.Equals("VZX-2782C2") || 
                modelName.Equals("VZX-2783C1") || 
                modelName.Equals("VZL-2780C2") || 
                modelName.Equals("TM05C0") || 
                modelName.Equals("B3K0")) 
            {
                return true;
            }

            return false;
        }

        public class ProductClass
        {
            public bool isTWT { get; set; }
            public bool isKLY { get; set; }
            public bool isSSPA { get; set; }

            public ProductClass(string prodType)
            {
                isTWT = false;
                isKLY = false;
                isSSPA = false;

                switch (prodType)
                {
                    case "GENIV":
                        isKLY = true;
                        break;
                    case "CKPA":
                        isKLY = true;
                        break;
                    case "SSPA":
                        isSSPA = true;
                        break;
                    case "SSPAODU":
                        isSSPA = true;
                        break;
                    default:
                        isTWT = true;
                        break;
                }
            }
        }
    }

    public class TestResults{

        public long id { get; set; }
        public string ModelSN { get; set; }
        public DateTime StartTime { get; set; }
        public string TestName { get; set; }
        public string TestNameNoDigit { get; set; }
        public string Result { get; set; }
        public string Units { get; set; }
        public string LowLimit { get; set; }
        public string UpLimit { get; set; }
        public bool PassFail { get; set; }
        public string Channel { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
        public string P3 { get; set; }
        public string P4 { get; set; }
        public string P5 { get; set; }
        public string P6 { get; set; }
        public string P7 { get; set; }
        public string P8 { get; set; }
        public string P9 { get; set; }

        public TestResults(long i, string sn, DateTime t, string tn, string tnNd, string res, string unit, string llim, string ulim, bool pf, string ch, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9){

            id          = i;
            ModelSN     = sn;
            StartTime   = t;
            TestName    = tn;
            TestNameNoDigit = tnNd;
            Result     = res;
            Units       = unit;
            LowLimit    = llim;
            UpLimit     = ulim;
            PassFail    = pf;
            Channel     = ch;
            P1          = p1;
            P2          = p2;
            P3          = p3;
            P4          = p4;
            P5          = p5;
            P6          = p6;
            P7          = p7;
            P8          = p8;
            P9          = p9;

        }
    }

    public class TestParams
    {

        public long id { get; set; }
        public string TestName { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
        public string P3 { get; set; }
        public string P4 { get; set; }
        public string P5 { get; set; }
        public string P6 { get; set; }
        public string P7 { get; set; }
        public string P8 { get; set; }
        public string P9 { get; set; }

        public TestParams(long i, string tn, string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
        {

            id = i;
            TestName = tn;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            P5 = p5;
            P6 = p6;
            P7 = p7;
            P8 = p8;
            P9 = p9;

        }
    }


    public class CommonDataValues{

        public string klystron { get; set; }

        public CommonDataValues(){
            klystron = "KLY";
        }
    }
}