using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hinata.Data;
using Hinata.Logging;
using Hinata.Markdown;
using JavaScriptEngineSwitcher.V8;

namespace Hinata
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.Initialize(GlobalSettings.DefaultConnectionString);
            DapperConfig.Initialize();
            DefaultTraceLogInitializer.Initialize(GlobalSettings.DefaultConnectionString, TraceLogLevel.Trace);
            LogManager.RegisterAccessLogger<AccessLogger>();
            MappingConfig.CreateMap();
            MarkdownParser.RegisterJsEngineType<V8JsEngine>();
        }
    }
}
