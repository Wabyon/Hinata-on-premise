using System.ComponentModel;

namespace Hinata.Web.Mvc.DataAnnotations
{
    public class PlaceHolderAttribute : DescriptionAttribute
    {
        public PlaceHolderAttribute(string description)
            : base(description)
        {
        }
    }
}