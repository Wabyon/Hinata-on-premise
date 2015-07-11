using System;

namespace Hinata.Logging
{
    public interface ITraceLogger
    {
        void Trace(TraceLogMessage message);

        void Trace(string message);

        void Trace(string message, params object[] args);

        void Info(string message);

        void Info(string message, params object[] args);

        void Error(Exception exception);

        void Error(Exception exception, string message);

        void Error(Exception exception, string message, params object[] args);
    }
}