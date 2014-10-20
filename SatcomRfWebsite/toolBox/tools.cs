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