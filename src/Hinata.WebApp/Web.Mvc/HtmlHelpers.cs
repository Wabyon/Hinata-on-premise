using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Hinata.Web.Mvc.DataAnnotations;

namespace Hinata.Web.Mvc
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString ToolTipFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var exp = (MemberExpression)expression.Body;
            foreach (Attribute attribute in exp.Expression.Type.GetProperty(exp.Member.Name).GetCustomAttributes(false))
            {
                if (typeof(ToolTipAttribute) == attribute.GetType())
                {
                    return MvcHtmlString.Create(((ToolTipAttribute)attribute).Description);
                }
            }
            return MvcHtmlString.Create("");
        }

        public static MvcHtmlString PlacceHolderFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var exp = (MemberExpression)expression.Body;
            foreach (Attribute attribute in exp.Expression.Type.GetProperty(exp.Member.Name).GetCustomAttributes(false))
            {
                if (typeof(PlaceHolderAttribute) == attribute.GetType())
                {
                    return MvcHtmlString.Create(((PlaceHolderAttribute)attribute).Description);
                }
            }
            return MvcHtmlString.Create("");
        }
    }
}