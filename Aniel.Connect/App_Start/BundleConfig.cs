using System.Web;
using System.Web.Optimization;

namespace Aniel.Connect
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //**************** Login ******************
            bundles.Add(new StyleBundle("~/Content/css-login").Include(
                      "~/Content/util.css",
                      "~/Content/main.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/bin/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/maskip").Include(
                        "~/bin/Scripts/ipInput.js"));
            

            //**************** Layout ******************
            bundles.Add(new StyleBundle("~/Content/csslayout").Include(
                      "~/bin/Content/bootstrap.min.css",
                      "~/bin/Content/font-awesome.min.css",
                      "~/bin/Content/ionicons.min.css",
                      "~/bin/Content/AdminLTE.min.css",
                      "~/bin/Content/all-skins.min.css",
                      "~/Content/theme.css",
                      "~/bin/Content/icofont.css",
                      "~/bin/Content/material-icons.css",
                      "~/bin/Content/style.css",
                      "~/bin/Content/line-awesome.css"));


            //**************** Layout Limpo ******************
            bundles.Add(new StyleBundle("~/Content/csslayoutlimpo").Include(
                      "~/bin/Content/DataTables/Bootstrap-4-4.1.1/css/bootstrap.min.css",
                      "~/bin/Content/jquery-ui.min.css",
                      "~/bin/Content/line-awesome.css",
                      "~/bin/Content/ionicons.min.css",
                      "~/bin/Content/all-skins.min.css",
                      "~/Content/theme.css",
                      "~/bin/Content/icofont.css",
                      "~/bin/Content/material-icons.css",
                      //"~/bin/Content/fontawesome-free-5.15.1.min.css",
                      "~/bin/Content/font-awesome.min.css",
                      "~/bin/Content/roboto.css",
                      "~/bin/Content/bootstrap-datepicker.css",
                      "~/bin/Content/toast/mk-notifications.min.css",
                      "~/bin/Content/toast/mk-theme-more.css",
                      "~/bin/Content/bootstrap4-toggle.min.css",
                      "~/bin/Content/bootstrap-tagsinput.css",
                      "~/bin/Content/select2/select2.css",
                      "~/bin/Content/connect.css",
                      "~/bin/Content/bootstrap-datepicker.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/jslayoutlimpo").Include(
                        "~/bin/Scripts/jquery-3.3.1.min.js",
                        "~/bin/Scripts/jquery.mask.min.js",
                        "~/bin/Scripts/popper.min.js",
                        "~/bin/Scripts/bootstrap.4.1.1.min.js",
                        "~/bin/Scripts/jquery-ui.min.js",
                        "~/bin/Scripts/all.js",
                        "~/bin/Scripts/sweetalert2@8.js",
                        "~/bin/Content/toast/mk-notifications.js",
                        "~/bin/Scripts/moment.js",
                        "~/bin/Scripts/dinero.min.js",
                        "~/bin/Scripts/connect.js",
                        "~/bin/Scripts/bootstrap4-toggle.min.js",
                        "~/bin/Scripts/bootstrap-tagsinput.js",
                        "~/bin/Scripts/bootstrap-datepicker.min.js",
                        "~/bin/Scripts/bootstrap-datepicker.pt-BR.min.js",
                        "~/bin/Content/select2/select2.min.js",
                        "~/bin/Content/select2/i18n/pt.js"//,
                        //"~/bin/Scripts/apexcharts.min.js"
                        ));

            //**************** DataTable ******************
            bundles.Add(new StyleBundle("~/Content/cssDataTable").Include(
                      "~/bin/Content/DataTables/DataTables-1.10.18/css/dataTables.bootstrap4.min.css",
                      "~/bin/Content/DataTables/FixedHeader-3.1.4/css/fixedHeader.dataTables.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jsDataTable").Include(
                        "~/bin/Content/DataTables/DataTables-1.10.18/js/jquery.dataTables.min.js",
                        "~/bin/Content/DataTables/RowGroup-1.1.0/js/dataTables.rowGroup.min.js",
                        "~/bin/Content/DataTables/pdfmake-0.1.36/pdfmake.min.js",
                        "~/bin/Content/DataTables/pdfmake-0.1.36/vfs_fonts.js",
                        "~/bin/Content/DataTables/Buttons-1.5.4/js/buttons.print.min.js",
                        "~/bin/Content/DataTables/Buttons-1.5.4/js/dataTables.buttons.min.js",
                        "~/bin/Content/DataTables/DataTables-1.10.18/js/dataTables.bootstrap4.min.js",
                        "~/bin/Content/DataTables/Buttons-1.5.4/js/buttons.flash.min.js",
                        "~/bin/Content/DataTables/JSZip-2.5.0/jszip.min.js",
                        "~/bin/Content/DataTables/Buttons-1.5.4/js/buttons.html5.min.js",
                        "~/bin/Content/DataTables/Scroller-1.5.0/js/dataTables.scroller.min.js",
                        "~/bin/Content/DataTables/FixedHeader-3.1.4/js/fixedHeader.dataTables.js",
                        "~/bin/Content/DataTables/dataTables.scrollResize.js"));

            bundles.Add(new ScriptBundle("~/bundles/jsDataTable11").Include(
            "~/bin/Content/DataTables/DataTables-1.11.3/js/jquery.dataTables.min.js",
            "~/bin/Content/DataTables/RowGroup-1.1.0/js/dataTables.rowGroup.min.js",
            "~/bin/Content/DataTables/pdfmake-0.1.36/pdfmake.min.js",
            "~/bin/Content/DataTables/pdfmake-0.1.36/vfs_fonts.js",
            "~/bin/Content/DataTables/Buttons-1.5.4/js/buttons.print.min.js",
            "~/bin/Content/DataTables/Buttons-1.5.4/js/dataTables.buttons.min.js",
            "~/bin/Content/DataTables/DataTables-1.10.18/js/dataTables.bootstrap4.min.js",
            "~/bin/Content/DataTables/Buttons-1.5.4/js/buttons.flash.min.js",
            "~/bin/Content/DataTables/JSZip-2.5.0/jszip.min.js",
            "~/bin/Content/DataTables/Buttons-1.5.4/js/buttons.html5.min.js",
            "~/bin/Content/DataTables/Scroller-1.5.0/js/dataTables.scroller.min.js",
            "~/bin/Content/DataTables/FixedHeader-3.1.4/js/fixedHeader.dataTables.js",
            "~/bin/Content/DataTables/dataTables.scrollResize.js"));

            bundles.Add(new ScriptBundle("~/bundles/jsDataTableSimples").Include(
                        "~/bin/Content/DataTables/DataTables-1.10.18/js/jquery.dataTables.min.js"));

            bundles.Add(new StyleBundle("~/Content/cssTreeview").Include(
                        "~/bin/Content/aimaraJS-master/css/Aimara.css"));

            bundles.Add(new ScriptBundle("~/bundles/jsTreeview").Include(
                        "~/bin/Content/aimaraJS-master/lib/Aimara.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/treejsMaster").Include(
                "~/bin/Content/treejs-master/dist/tree.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/visjsPackage").Include(
                "~/bin/Scripts/vis-network.js",
                "~/bin/Scripts/vis-timeline-graph2d.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/viabilidadejsPackage").Include(
                "~/Scripts/Consulta_Viabilidade.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = CONSTANTES.EnableOptimizations;
        }
    }
}
