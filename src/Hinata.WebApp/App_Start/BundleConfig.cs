using System.Web.Optimization;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;

namespace Hinata
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/hinata").Include(
                "~/Scripts/hinata.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/view/draft/edit").Include(
                "~/lib/marked/marked.js",
                "~/lib/highlightjs/highlight.pack.js",
                "~/Scripts/draft.edit.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/view/draft/index").Include(
                "~/lib/marked/marked.js",
                "~/lib/highlightjs/highlight.pack.js",
                "~/Scripts/draft.index.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/view/item/index").Include(
                "~/Scripts/item.index.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/view/item/item").Include(
                "~/lib/marked/marked.js",
                "~/lib/highlightjs/highlight.pack.js",
                "~/Scripts/item.item.js"
                ));

            var bootstrapStylesBundle = new CustomStyleBundle("~/Content/bootstrap.less") {Orderer = new NullOrderer()};
            bootstrapStylesBundle.Include("~/Content/bootstrap/bootstrap.custom.less");
            bundles.Add(bootstrapStylesBundle);

            var hinataStylesBundle = new CustomStyleBundle("~/Content/hinata.less") { Orderer = new NullOrderer() };
            hinataStylesBundle.Include("~/Content/hinata/hinata.less");
            bundles.Add(hinataStylesBundle);

            //bundles.Add(new StyleBundle("~/Content/css").Include("~/lib/github-markdown-css/github-markdown.css"));
        }
    }
}
