using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace Hinata.Logging.Internals
{
    internal class TraceLogConfigurationFactory
    {
        public static LoggingConfiguration Create(string connectionString, TraceLogLevel traceLogLevel)
        {
            var config = new LoggingConfiguration();

            var databaseTarget = new DatabaseTarget();
            config.AddTarget("database", databaseTarget);

            databaseTarget.ConnectionString = connectionString;
            databaseTarget.CommandText = @"
INSERT INTO [dbo].[TraceLogs] (
    [Logger],
    [Level],
    [ThreadId],
    [MachineName],
    [Message]
) VALUES (
    @Logger,
    @Level,
    @ThreadId,
    @MachineName,
    @Message
);
";
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Logger", new SimpleLayout("${logger}")));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Level", new SimpleLayout("${uppercase:${level}}")));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@ThreadId", new SimpleLayout("${threadid}")));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@MachineName", new SimpleLayout("${machinename}")));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@CallSite", new SimpleLayout("${callsite}")));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@UserName", new SimpleLayout("${identity}")));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Message", new SimpleLayout("${message}")));
            databaseTarget.Parameters.Add(new DatabaseParameterInfo("@StackTrace", new SimpleLayout("${exception:format=tostring}")));

            var loggingRule = new LoggingRule("*", traceLogLevel.ToNLogLevel(), databaseTarget);
            config.LoggingRules.Add(loggingRule);

            return config;
        }
    }
}
