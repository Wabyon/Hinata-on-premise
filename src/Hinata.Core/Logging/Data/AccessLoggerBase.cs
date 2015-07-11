using System;
using System.Data.SqlClient;
using Dapper;

namespace Hinata.Logging.Data
{
    public abstract class AccessLoggerBase : IAccessLogger
    {
        private readonly string _connectionString;
        protected AccessLoggerBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected abstract Func<AccessLog> GetWebAccessLog { get; }

        public void Write()
        {
            SqlConnection cn = null;
            try
            {
                var log = GetWebAccessLog();

                using (cn = new SqlConnection(_connectionString))
                {
                    cn.Open();
                    cn.Execute(@"
INSERT INTO [dbo].[AccessLogs] (
    [ServerName],
    [UserName],
    [Url],
    [HttpMethod],
    [Path],
    [Query],
    [Form],
    [Controller],
    [Action],
    [UserHostAddress],
    [UserAgent]
) VALUES (
    @ServerName,
    @UserName,
    @Url,
    @HttpMethod,
    @Path,
    @Query,
    @Form,
    @Controller,
    @Action,
    @UserHostAddress,
    @UserAgent
)
", log);
                }
            }
            catch
            {
            }
            finally
            {
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }
    }
}