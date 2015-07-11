using System.Web.Mvc;
using System.Web.Routing;
using Hinata.Data.Commands;
using Hinata.Filters;

namespace Hinata.Web.Mvc
{
    [TraceLogFilter]
    [AccessLogFilter]
    [Authorize]
    public abstract class WindowsAuthenticationContoller : Controller
    {
        private readonly UserDbCommand _userDbCommand = new UserDbCommand(GlobalSettings.DefaultConnectionString);

        protected User LogonUser { get; private set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            LogonUser = _userDbCommand.FindByLogonNameAsync(User.Identity.Name).Result;
            if (LogonUser != null && string.IsNullOrWhiteSpace(LogonUser.IconUrl)) LogonUser.IconUrl = GlobalSettings.NoImageUserIconUrl;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.LogonUser = LogonUser;
            base.OnActionExecuted(filterContext);
        }
    }
}