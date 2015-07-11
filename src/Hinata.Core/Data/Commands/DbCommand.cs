using System;
using System.Data.Common;
using Hinata.Logging.Data;
using StackExchange.Profiling.Data;

namespace Hinata.Data.Commands
{
    public abstract class DbCommand
    {
        private readonly string _connectionString;
        protected DbCommand(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is null or white space", "connectionString");

            _connectionString = connectionString;
        }

        public DbConnection CreateConnection()
        {

            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            var cn = factory.CreateConnection();
            if (cn == null) throw new InvalidOperationException();

            cn.ConnectionString = _connectionString;
            return new ProfiledDbConnection(cn, new TraceDbProfiler());
        }
    }
}
