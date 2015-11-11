using System.Web.Optimization;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;

namespace Hinata
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js","~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/markedjs").Include("~/Scripts/marked/marked.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/highlightjs").Include("~/Scripts/highlightjs/highlight.pack.js"));
            bundles.Add(new ScriptBundle("~/bundles/jsdiff").Include("~/Scripts/jsdiff/diff.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/hinata").Include("~/Scripts/hinata.js"));
            bundles.Add(new ScriptBundle("~/scripts/view/draft/edit").Include("~/Scripts/draft.edit.js"));
            bundles.Add(new ScriptBundle("~/scripts/view/draft/index").Include("~/Scripts/draft.index.js"));
            bundles.Add(new ScriptBundle("~/scripts/view/item/index").Include("~/Scripts/item.index.js"));
            bundles.Add(new ScriptBundle("~/scripts/view/item/item").Include("~/Scripts/item.item.js"));
            bundles.Add(new ScriptBundle("~/scripts/view/item/editcollaborators").Include("~/Scripts/item.editcollaborators.js"));

            bundles.Add(new StyleBundle("~/Content/highlightjs.css").Include("~/Content/highlightjs/vs.css"));

            var bootstrapStylesBundle = new CustomStyleBundle("~/Content/bootstrap.less") {Orderer = new NullOrderer()};
            bootstrapStylesBundle.Include("~/Content/bootstrap/bootstrap.custom.less");
            bundles.Add(bootstrapStylesBundle);

            var hinataStylesBundle = new CustomStyleBundle("~/Content/hinata.less") { Orderer = new NullOrderer() };
            hinataStylesBundle.Include("~/Content/hinata/hinata.less");
            bundles.Add(hinataStylesBundle);
        }
    }
}
