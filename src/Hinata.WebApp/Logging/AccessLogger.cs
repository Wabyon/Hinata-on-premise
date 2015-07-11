using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hinata.Logging.Data;
using Newtonsoft.Json;

namespace Hinata.Logging
{
    public class AccessLogger : AccessLoggerBase
    {
        public AccessLogger() : base(GlobalSettings.DefaultConnectionString)
        {
        }

        protected override Func<AccessLog> GetWebAccessLog
        {
            get
            {
                return () =>
                {
                    var request = HttpContext.Current.Request;

                    string actionName = null;
                    string controllerName = null;
                    var directRouteActions = request.RequestContext.RouteData.DataTokens["MS_DirectRouteActions"];
                    ActionDescriptor[] actionDescriptors = null;
                    var actions = directRouteActions as ActionDescriptor[];
                    if (actions != null)
                    {
                        actionDescriptors = actions;
                    }
                    if (actionDescriptors != null && actionDescriptors.Any())
                    {
                        var action = actionDescriptors[0];
                        actionName = action.ActionName;
                        controllerName = action.ControllerDescriptor.ControllerName;
                    }

                    var form = ToDictionary(request.Form);
                    form.Remove("__RequestVerificationToken");
                    form.Remove("Password");

                    var log = new AccessLog
                    {
                        UserName = (request.LogonUserIdentity == null) ? "" : request.LogonUserIdentity.Name,
                        ServerName = HttpContext.Current.Server.MachineName,
                        Url = request.Url.ToString(),
                        HttpMethod = request.HttpMethod,
                        Path = request.Path,
                        Query = JsonConvert.SerializeObject(ToDictionary(request.QueryString)),
                        Form = JsonConvert.SerializeObject(form),
                        Controller = controllerName,
                        Action = actionName,
                        UserAgent = request.UserAgent,
                        UserHostAddress = request.UserHostAddress
                    };

                    return log;
                };
            }
        }
        public static IDictionary<string, string> ToDictionary(NameValueCollection nameValueCollection)
        {
            return nameValueCollection.AllKeys.ToDictionary(key => key, key => nameValueCollection[key]);
        }
    }
}