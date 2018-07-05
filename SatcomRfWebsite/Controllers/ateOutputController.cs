using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services;
using SatcomRfWebsite.Models;

namespace SatcomRfWebsite.Controllers
{
    public class ateDataController : Controller
    {
        public SearchModel searchModel = new SearchModel();

        //
        // GET: /ateData/AteOutput/...
        [HttpGet]
        public ActionResult AteOutput(string filter = "")
        {
            ViewBag.getResetPath = "ateData/AteOutput";
            var model = new AteOutputTopViewModel();

            //set all the filter parameters: pordType, modelName, ser# ...
            model.ateOutputModel.ParseFilter(filter);

            //generate lists of prodType, modNames, serNums, tubeNames based on filter
            model.ateOutputModel.GenerateLists();

            //generate ate output based on filter
            model.ateOutputModel.GenerateAteOutput();

            return View(model);
        }

        //
        // GET: /ateData/AteOutput/...
        [HttpGet]
        public ActionResult AteOutputDetail(string serNum = "")
        {
            var model = new AteOutputTopViewModel();
            var tools = new ToolBox();

            model.ateOutputModel.serialNum = serNum;

            //make sure product type is set
            model.ateOutputModel.productType = tools.getProdTypeFromSN(serNum);

            if (model.ateOutputModel.GenerateAteOutputDetail())
            {
                return View(model);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection value)
        {
            return View();
        }
    }
}