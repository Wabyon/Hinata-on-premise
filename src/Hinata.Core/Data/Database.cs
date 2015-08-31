using System.Data.SqlClient;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;

namespace Hinata.Data
{
    public class Database
    {
        public static void Initialize(string connectionString)
        {
            CreateIfNotExists(connectionString);
            MigrateToLatest(connectionString);
        }

        public static void CreateIfNotExists(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;

            connectionStringBuilder.InitialCatalog = "master";
            connectionStringBuilder.ConnectTimeout = 20;

            using (var connection = new SqlConnection(connectionStringBuilder.ToString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(@"
SELECT
    name
FROM master.sys.databases
WHERE
    name = '{0}'
",
                 databaseName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) return;
                    }

                    command.CommandText = string.Format("CREATE DATABASE [{0}] Japanese_CI_AS", databaseName);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }


        private class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public string ProviderSwitches { get; set; }
            public int Timeout { get; set; }
        }

        public static void MigrateToLatest(string connectionString)
        {
            var announcer = new NullAnnouncer();
            var assembly = Assembly.GetExecutingAssembly();

            var migrationContext = new RunnerContext(announcer)
            {
                Namespace = "Hinata.Data.Migrations"
            };

            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();
            var processor = factory.Create(connectionString, announcer, options);
            var runner = new MigrationRunner(assembly, migrationContext, processor);
            runner.MigrateUp(true);
        }

        public static void MigrateDown(string connectionString)
        {
            var announcer = new NullAnnouncer();
            var assembly = Assembly.GetExecutingAssembly();

            var migrationContext = new RunnerContext(announcer)
            {
                Namespace = "Hinata.Data.Migrations"
            };

            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();
            var processor = factory.Create(connectionString, announcer, options);
            var runner = new MigrationRunner(assembly, migrationContext, processor);
            runner.MigrateDown(-1);
        }
    }
}
