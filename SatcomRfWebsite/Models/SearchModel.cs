using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SatcomRfWebsite.Models
{
    public class SearchModel
    {
        public IList<SatcomRfWebsite.Models.tblProductType> productTypeList { get; set; }
        public IList<SatcomRfWebsite.Models.tblModelName> modelNameList { get; set; }

        public string modelNamesStr { get; set; }
        public string prodTypeStr { get; set; }
        public string testName { get; set; }
        public string testType { get; set; }
        public string tubeName { get; set; }
        public string options { get; set; }
        public List<string> tubeList { get; set; }


        private rfDbEntities db = new rfDbEntities();

        //constructor
        public SearchModel()
        {
            productTypeList = new List<SatcomRfWebsite.Models.tblProductType>();
            modelNameList = new List<SatcomRfWebsite.Models.tblModelName>();

            testName = "";
            testType = "";
            tubeName = "";
            options = "";
            tubeList = new List<string>();

        }

        /*
         * - Generate list of product types and a list of model names
         * - Generate two strings that contains product types and model names.
         *   These strings are structured as an array so they can be used by Java script easily.
         */
        public void allProdTypesModelNames()
        {
            // Add all products, models to the list order by product type and model name respectively
            productTypeList = db.tblProductTypes.OrderBy(x => x.ProductType).ToList();
            modelNameList = db.tblModelNames.OrderBy(x => x.ModelName).ToList();


            string charSeperator = "";

            foreach (var item in modelNameList)
            {
                prodTypeStr += charSeperator + "'" + item.ProductType + "'";
                modelNamesStr += charSeperator + "'" + item.ModelName + "'";
                charSeperator = ",";
            }
        }

        /*
         * - Generate list of product types and a list of model names that correspond to the passed in product type
         * - Generate model names string from product type (passed to the function)
         *   String is structured as an array so it can be used by Java script easily
         */
        public void modelNames(string prodType)
        {
            // Add all products, models to the list
            productTypeList = db.tblProductTypes.OrderBy(x => x.ProductType).ToList();
            modelNameList = db.tblModelNames.Where(x => x.ProductType.Equals(prodType)).OrderBy(x => x.ModelName).ToList();

            string charSeperator = "";

            foreach (var item in modelNameList)
            {

                if (item.ProductType.Equals(prodType))
                {

                    prodTypeStr += charSeperator + "'" + prodType + "'";
                    modelNamesStr += charSeperator + "'" + item.ModelName + "'";
                    charSeperator = ",";
                }
            }
        }

        /*
         * 
         */
        public void parseFilter(string filter)
        {

            Match match = Regex.Match(filter, @"(?<=testName\=)([a-zA-Z0-9]*)(?=\+)");

            if (match.Success)
            {
                testName = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=testType\=)([a-zA-Z0-9]*)(?=\+)");

            if (match.Success)
            {
                testType = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=tubeName\=)([a-zA-Z0-9-]*)(?=\+)");

            if (match.Success)
            {
                tubeName = match.Groups[0].Value;
            }

            match = Regex.Match(filter, @"(?<=opt\=)([a-zA-Z0-9,]*)");

            if (match.Success)
            {
                options = match.Groups[0].Value;
            }
        }

        public List<string> getTubes(string modelN)
        {

            var myQuery = (from serialNums in db.tblSerialNumbers
                           join ateOut in db.tblATEOutputs on serialNums.ModelSN equals ateOut.ModelSN
                           where serialNums.ModelName.Equals(modelN)
                           orderby ateOut.TubeName ascending
                           select ateOut).Select(x => x.TubeName).Distinct();

            tubeList.Clear();

            foreach (var output in myQuery)
            {
                if (output != null)
                {
                    tubeList.Add(output.ToString());
                }
            }

            return tubeList;
        }
    }
}