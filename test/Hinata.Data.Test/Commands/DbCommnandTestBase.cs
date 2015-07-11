using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;

namespace Hinata.Data.Commands
{
    [TestFixture]
    public abstract class DbCommnandTestBase
    {
        protected static readonly string ConnectionString = @"Data Source=(localdb)\v11.0;Initial Catalog=HinataTest;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        protected readonly UserDbCommand UserDbCommand = new UserDbCommand(ConnectionString);
        protected readonly ItemDbCommand ItemDbCommand = new ItemDbCommand(ConnectionString);
        protected readonly DraftDbCommand DraftDbCommand = new DraftDbCommand(ConnectionString);
        protected readonly User LogonUser = new User(@"Domain\UsserName"){ Name = "test_user", DisplayName = "テストユーザー"};
        private static bool _isDbInitialized;

        [TestFixtureSetUp]
        public virtual void SetUp()
        {
            if (!_isDbInitialized)
            {
                // テスト初回のみ、すでにDBが存在したらDBを再作成する
                DropAllTable();
                _isDbInitialized = true;
            }

            Database.Initialize(ConnectionString);
            DapperConfig.Initialize();

            UserDbCommand.SaveAsync(LogonUser).Wait();
        }

        private static void DropAllTable()
        {
            var sb = new SqlConnectionStringBuilder(ConnectionString);
            var initialCatalog = sb.InitialCatalog;
            sb.InitialCatalog = "master";
            var cn = new SqlConnection(sb.ToString());
            try
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"select * from sys.databases where name = '{0}'", initialCatalog);
                    cmd.CommandType = CommandType.Text;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Database.MigrateDown(ConnectionString);
                        }
                    }
                }
            }
            catch
            {
                // do nothing.
            }
            finally
            {
                cn.Dispose();
            }
        }
    }
}