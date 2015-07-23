using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SatcomRfWebsite.Models
{
    public class AteOutputTopViewModel
    {
        public AteOutputModel ateOutputModel { get; set; }
        public ToolBox tools { get; set; }

        public AteOutputTopViewModel()
        {
            ateOutputModel = new AteOutputModel();
            tools = new ToolBox();
        }
    }
}