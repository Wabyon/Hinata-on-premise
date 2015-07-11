using System;
using Hinata.Logging.Internals;

namespace Hinata.Logging
{
    public sealed class LogManager
    {
        private static Type _accessLoggerType;

        public static void RegisterAccessLogger<T>()
            where T : IAccessLogger, new()
        {
            _accessLoggerType = typeof (T);
        }


        public static ITraceLogger GetTraceLogger(string name)
        {
            return new TraceLogger(name);
        }

        public static IAccessLogger GetWebAccessLogger()
        {
            if (_accessLoggerType == null) return new EmptyWebAccessLogger();
            var logger = Activator.CreateInstance(_accessLoggerType) as IAccessLogger;
            return logger ?? new EmptyWebAccessLogger();
        }
    }
}
