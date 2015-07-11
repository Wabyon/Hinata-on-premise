using System.Configuration;

namespace Hinata
{
    public class GlobalSettings
    {
        public static string DefaultConnectionString { get; private set; }

        public static string BloqsBaseAddress { get; private set; }

        public static string BloqsAccountName { get; private set; }

        public static string BloqsAccessKey { get; private set; }

        public const string NoImageUserIconUrl = @"~/Content/no-image.png";

        static GlobalSettings()
        {
            DefaultConnectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            BloqsBaseAddress = ConfigurationManager.AppSettings["bloqs:baseAddress"];
            BloqsAccountName = ConfigurationManager.AppSettings["bloqs:accoutName"];
            BloqsAccessKey = ConfigurationManager.AppSettings["bloqs:accessKey"];
        }
    }
}