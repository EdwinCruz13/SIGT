using System.Web;
using System.Web.Optimization;

namespace SIGT_FULL
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      //bootstrap
                      "~/Scripts/bootstrap.js",

                      //Datatable script
                      "~/Scripts/DataTables/jquery.dataTables.js",
                      "~/Scripts/DataTables/dataTables.bootstrap.js",

                      //jquery
                      //"~/Scripts/jquery-3.3.1.js",
                      "~/Scripts/jquery-1.10.2.js",
                      "~/Scripts/jquery-ui-1.12.1.js",

                      //Formulario ajax
                      "~/Scripts/jquery.unobtrusive-ajax.min.js",
                      "~/Scripts/knockout-3.4.2.js",

                      //Multiselect plugins
                      "~/App/MultipleSelect/multiple-select.js",

                      //jScrollPane
                      "~/App/jScrollPane/jquery.jscrollpane.min.js",
                      "~/App/jScrollPane/jquery.mousewheel.js",

                      //jplayer
                      "~/App/jPlayer/dist/jplayer/jquery.jplayer.min.js",
                      "~/App/jPlayer/dist/add-on/jplayer.playlist.min.js",

                       //HighChart
                       "~/App/highcharts/highcharts.js",
                       "~/App/highcharts/highcharts-more.js",
                       "~/App/highcharts/modules/exporting.js",
                       "~/App/highcharts/modules/export-data.js",
                       "~/App/highcharts/modules/series-label.js",
                       "~/App/highcharts/modules/solid-gauge.js",

                       //text-to-voice
                       "~/App/text-to-voice/speech.js",


                      //SignalR
                      "~/Scripts/jquery.signalR-2.2.2.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //bootstrap
                      "~/Content/bootstrap.css",

                      //DataTable
                      "~/Content/DataTables/css/jquery.dataTables.css",
                      "~/Content/DataTables/css/dataTables.bootstrap.css",

                      //Mis estilos
                      "~/Content/beginings.css",

                      //estilo jquery-UI
                      "~/Content/themes/base/jquery-ui.min.css",

                      
                      //Multiselect skin
                      "~/App/MultipleSelect/multiple-select.css",

                      //jScrollPane
                      "~/App/jScrollPane/jScrollPane.css",


                      //jplayer skin
                      "~/App/jPlayer/dist/skin/blue.monday/css/jplayer.blue.monday.min.css"));






            bundles.Add(new ScriptBundle("~/bundles/Expendedor").Include(
                        //keyboard
                        "~/App/keyboard/js/keyboard.js"));

            bundles.Add(new StyleBundle("~/Content/Expendedor").Include(

                      //keyboard
                      "~/App/keyboard/css/style.css"));

        }



    }

}
