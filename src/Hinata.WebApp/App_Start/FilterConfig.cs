using System.Web.Mvc;
using Hinata.Filters;

namespace Hinata
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalHandleErrorAttribute());
        }
    }
}
