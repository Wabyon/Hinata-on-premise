using Hinata.Logging.Internals;

namespace Hinata.Logging
{
    public class DefaultTraceLogInitializer
    {
        public static void Initialize(string connectionString, TraceLogLevel traceLogLevel)
        {
            var config = TraceLogConfigurationFactory.Create(connectionString, traceLogLevel);
            NLog.LogManager.Configuration = config;
        }
    }
}
