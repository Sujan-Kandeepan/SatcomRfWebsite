using System.Collections.Generic;

namespace SatcomRfWebsite.Models
{
    public class TestData
    {
        public string TestName { get; set; }
        public string MinResult { get; set; }
        public string MaxResult { get; set; }
        public string AvgResult { get; set; }
        public string Unit { get; set; }
        public string Channel { get; set; }

        public TestData() { }
    }

    public class TestInfo
    {
        public string TestName { get; set; }
        public string Channel { get; set; }
        public string Units { get; set; }
        public List<string> Results { get; set; }

        public TestInfo(string inTestName, string inChannel, string inUnits, string[] inResults)
        {
            TestName = inTestName;
            Channel = inChannel;
            Units = inUnits;
            Results = new List<string>(inResults);
        }
    }
}