using System.Web;
using System.Web.Optimization;

namespace SatcomRfWebsite
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/myJsBundle").Include(
                        "~/Scripts/jquery.js",
                        /*"~/Scripts/jquery-{version}.js",*/
                        "~/Scripts/bootstrap*",
                        "~/Scripts/selectize.js",
                        "~/Scripts/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/SatcomStats").Include("~/Scripts/SatcomStats/Index.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/site.css",
                "~/Content/bootstrap.css",
                "~/Content/selectize.css",
                "~/Content/jquery.dataTables.css"));

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}