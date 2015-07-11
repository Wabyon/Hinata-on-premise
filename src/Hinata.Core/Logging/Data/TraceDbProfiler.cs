using System;
using System.Data;
using System.Diagnostics;
using StackExchange.Profiling.Data;

namespace Hinata.Logging.Data
{
    public class TraceDbProfiler : IDbProfiler
    {
        private readonly ITraceLogger _logger = LogManager.GetTraceLogger("SQL");
        private Stopwatch _stopwatch;
        private string _commandText;

        public bool IsActive
        {
            get { return true; }
        }

        public void OnError(IDbCommand profiledDbCommand, SqlExecuteType executeType, Exception exception)
        {
        }

        public void ExecuteStart(IDbCommand profiledDbCommand, SqlExecuteType executeType)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void ExecuteFinish(IDbCommand profiledDbCommand, SqlExecuteType executeType, System.Data.Common.DbDataReader reader)
        {
            _commandText = profiledDbCommand.CommandText;
            if (executeType == SqlExecuteType.Reader) return;

            _stopwatch.Stop();
            _logger.Trace(new TraceLogMessage(executeType, _commandText, _stopwatch.ElapsedMilliseconds));
        }

        public void ReaderFinish(IDataReader reader)
        {
            _stopwatch.Stop();
            _logger.Trace(new TraceLogMessage(SqlExecuteType.Reader, _commandText, _stopwatch.ElapsedMilliseconds));
        }
    }
}
