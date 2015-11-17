using System.Web.Mvc;

namespace Hinata.Web.Mvc
{
    public static class UrlHelpers
    {
        public static string ResizedImage(this UrlHelper urlHelper, string originalUrl, int size)
        {
            return ResizedImage(urlHelper, originalUrl, size, size);
        }
        public static string ResizedImage(this UrlHelper urlHelper, string originalUrl, int width, int height)
        {
            return urlHelper.Action("Resize", "Image", new { width, height }) + "?u=" + originalUrl;
        }
    }
}
