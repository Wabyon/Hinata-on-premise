using System.Web.Mvc;
using Hinata.Logging;

namespace Hinata.Filters
{
    public class AccessLogFilter : ActionFilterAttribute
    {
        private readonly IAccessLogger _webAccessLogger = LogManager.GetWebAccessLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _webAccessLogger.Write();
            base.OnActionExecuting(filterContext);
        }
    }
}