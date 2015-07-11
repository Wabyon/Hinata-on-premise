using System;
using NLog;

namespace Hinata.Logging.Internals
{
    internal class TraceLogger : ITraceLogger
    {
        private readonly Logger _logger;
        public TraceLogger(string name)
        {
            _logger = NLog.LogManager.GetLogger(name);
        }

        public void Trace(TraceLogMessage message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception);
        }
        public void Error(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }
        public void Error(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }
    }
}