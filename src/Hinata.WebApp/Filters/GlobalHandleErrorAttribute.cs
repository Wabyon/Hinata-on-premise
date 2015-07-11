using System;
using System.Net;
using System.Web.Mvc;
using Hinata.Logging;

namespace Hinata.Filters
{
    public class GlobalHandleErrorAttribute : HandleErrorAttribute
    {
        private readonly ITraceLogger _traceLogger = LogManager.GetTraceLogger("APP");

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");

            _traceLogger.Error(filterContext.Exception);

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HandleAjaxRequestException(filterContext);
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        private static void HandleAjaxRequestException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled) return;

            filterContext.Result = new JsonResult
            {
                Data = new
                {
                    filterContext.Exception.Message,
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode =
                (int) HttpStatusCode.InternalServerError;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}