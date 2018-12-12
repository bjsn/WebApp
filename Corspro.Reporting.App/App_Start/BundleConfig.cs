using System.Web.Optimization;

namespace Corspro.Reporting.App
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-idleTimeout.js",
                        "~/Scripts/sessvars.js", 			
                        "~/Scripts/jqwidgets/jqxcore.js",
                        "~/Scripts/jqwidgets/jqxdata.js",
                        "~/Scripts/jqwidgets/jqxbuttons.js",
                        "~/Scripts/jqwidgets/jqxscrollbar.js",
                        "~/Scripts/jqwidgets/jqxvalidator.js",
                        "~/Scripts/jqwidgets/jqxmenu.js",
                        "~/Scripts/jqwidgets/jqxgrid.js",
                        "~/Scripts/jqwidgets/jqxgrid.edit.js",
                        "~/Scripts/jqwidgets/jqxgrid.selection.js",
                        "~/Scripts/jqwidgets/jqxgrid.grouping.js",
                        "~/Scripts/jqwidgets/jqxgrid.aggregates.js",
                        "~/Scripts/jqwidgets/jqxgrid.pager.js",
                        "~/Scripts/jqwidgets/jqxgrid.sort.js",
                        "~/Scripts/jqwidgets/jqxgrid.filter.js",
                        "~/Scripts/jqwidgets/jqxwindow.js",
                        "~/Scripts/jqwidgets/jqxpanel.js",
                        "~/Scripts/jqwidgets/jqxlistbox.js",
                        "~/Scripts/jqwidgets/jqxdropdownlist.js",
                        "~/Scripts/jqwidgets/jqxcombobox.js",
                        "~/Scripts/jqwidgets/jqxpasswordinput.js",
                        "~/Scripts/jqwidgets/jqxinput.js",
                        "~/Scripts/jqwidgets/jqxcheckbox.js",
                        "~/Scripts/jqwidgets/jqxnumberinput.js",
                        "~/Scripts/jqwidgets/jqxdatetimeinput.js",
                        "~/Scripts/jqwidgets/jqxcalendar.js",
                        "~/Scripts/jqwidgets/globalization/globalize.js",
                        "~/Scripts/jqwidgets/jqxgrid.columnsresize.js",
                        "~/Scripts/jqwidgets/jqxgrid.columnsreorder.js",
                        "~/Scripts/jqwidgets/jqxdata.export.js",
                        "~/Scripts/jqwidgets/jqxgrid.export.js",
                        "~/Scripts/jqwidgets/jqxgrid.storage.js",
                        "~/Scripts/jqwidgets/jqxtooltip.js",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/generalMenu.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/userwidgets").Include(
                        "~/Scripts/userManagement.js"));

            bundles.Add(new ScriptBundle("~/bundles/cdfwidgets").Include(
                        "~/Scripts/cdfManagement.js"));

            bundles.Add(new ScriptBundle("~/bundles/oppstatuswidgets").Include(
                        "~/Scripts/oppStatusManagement.js"));

            bundles.Add(new ScriptBundle("~/bundles/opportunitywidgets").Include(
                    "~/Scripts/opportunityManagement.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/grid/css").Include(
                        "~/Scripts/jqwidgets/styles/jqx.base.css",
                        "~/Scripts/jqwidgets/styles/jqx.arctic.css",
                        "~/Scripts/jqwidgets/styles/jqx.highcontrast.css",
                        "~/Scripts/jqwidgets/styles/jqx.office.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css", "~/Content/jqx.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}