using System.ComponentModel;

namespace Hinata.Web.Mvc.DataAnnotations
{
    public class ToolTipAttribute : DescriptionAttribute
    {
        public ToolTipAttribute(string description)
            : base(description)
        {
        }
    }
}