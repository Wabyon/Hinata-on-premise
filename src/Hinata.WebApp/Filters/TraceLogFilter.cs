using System.Diagnostics;
using System.Web.Mvc;
using Hinata.Logging;
using Newtonsoft.Json;

namespace Hinata.Filters
{
    public class TraceLogFilter : ActionFilterAttribute
    {
        private readonly ITraceLogger _traceLogger = LogManager.GetTraceLogger("APP");

        private Stopwatch _stopwatch;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _stopwatch = Stopwatch.StartNew();
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();
            var area = string.Empty;
            if (filterContext.RouteData.DataTokens.ContainsKey("area"))
            {
                area = filterContext.RouteData.DataTokens["area"].ToString();
            }
            var method = filterContext.HttpContext.Request.HttpMethod;
            var key = new {area, controller, action, method};

            _traceLogger.Trace(new TraceLogMessage(0, JsonConvert.SerializeObject(key), _stopwatch.ElapsedMilliseconds));

            base.OnResultExecuted(filterContext);
        }
    }
}