using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SatcomRfWebsite.Models
{
    public class CalibrationModel
    {
        public List<CalibrationData> CalRecords;

        private rfDbEntities db = new rfDbEntities();

        public CalibrationModel()
        {
            CalRecords = new List<CalibrationData>();
        }
    }
}