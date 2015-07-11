using NLog;

namespace Hinata.Logging.Internals
{
    internal static class TraceLogLevelExtensions
    {
        public static LogLevel ToNLogLevel(this TraceLogLevel traceLogLevel)
        {
            if (traceLogLevel == null) return LogLevel.Off;

            if (traceLogLevel.Equals(TraceLogLevel.Trace)) return LogLevel.Trace;
            if (traceLogLevel.Equals(TraceLogLevel.Info)) return LogLevel.Info;
            if (traceLogLevel.Equals(TraceLogLevel.Error)) return LogLevel.Error;

            return LogLevel.Off;
        }
    }
}
