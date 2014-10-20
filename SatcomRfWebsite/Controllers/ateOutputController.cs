using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SatcomRfWebsite.Models;

namespace SatcomRfWebsite.Controllers
{
    public class ateDataController : Controller
    {
        public SearchModel searchModel = new SearchModel();

        //
        // GET: /ateOutput/
        [HttpGet]
        public ActionResult AteOutput(string prodT = "", string modelN = "", string filter = "")
        {
            var model = new AteDataTopViewModel();

            ViewBag.getResetPath = "ateData/AteOutput";
            ViewBag.getProdType = prodT;
            ViewBag.getModName = modelN;
            ViewBag.getFilter = filter;

            //Do this if product type was selected
            if (prodT != "")
            {
                //limit model names related to product type only.
                //model.modelNames(prodT);
                model.searchModel.modelNames(prodT);

                if (modelN != "")
                {
                    model.searchModel.getTubes(modelN);
                    model.searchModel.parseFilter(filter);

                    model.ateOutModel.generateAteOutput(prodT, modelN, model.searchModel.testType, model.searchModel.tubeName, model.searchModel.options);
                }
            }
            else
            {
                // get all product types and model names
                model.searchModel.allProdTypesModelNames();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection value)
        {

            return View();
        }
    }
}