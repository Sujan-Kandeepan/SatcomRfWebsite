using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SatcomRfWebsite.Models
{
    public class AteDataTopViewModel
    {
        public SearchModel searchModel { get; set; }
        public TestResultsModel testResModel { get; set; }
        public AteOutputModel ateOutModel { get; set; }

        public AteDataTopViewModel()
        {
            searchModel = new SearchModel();
            testResModel = new TestResultsModel();
            ateOutModel = new AteOutputModel();
        }
    }
}