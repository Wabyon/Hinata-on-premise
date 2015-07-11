using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Hinata.Data.Commands
{
    public class UserDbCommand : DbCommand
    {
        public UserDbCommand(string connectionString) : base(connectionString)
        {
        }

        public Task<User> FindAsync(string id)
        {
            return FindAsync(id, CancellationToken.None);
        }

        public async Task<User> FindAsync(string id, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    [Id],
    [LogonName],
    [Name] = [UserName],
    [DisplayName],
    [IconUrl]
FROM [dbo].[Users]
WHERE
    [Id] = @Id
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results =
                    (await cn.QueryAsync<User>(sql, new { Id = id }).ConfigureAwait(false)).ToArray();

                return results.Any() ? results.First() : null;
            }
        }

        public Task<User> FindByLogonNameAsync(string logonName)
        {
            return FindByLogonNameAsync(logonName, CancellationToken.None);
        }

        public async Task<User> FindByLogonNameAsync(string logonName, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    [Id],
    [LogonName],
    [Name] = [UserName],
    [DisplayName],
    [IconUrl]
FROM [dbo].[Users]
WHERE
    [LogonName] = @LogonName
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results =
                    (await cn.QueryAsync<User>(sql, new {LogonName = logonName}).ConfigureAwait(false)).ToArray();

                return results.Any() ? results.First() : null;
            }
        }

        public Task<User> FindByNameAsync(string name)
        {
            return FindByNameAsync(name, CancellationToken.None);
        }

        public async Task<User> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    [Id],
    [LogonName],
    [Name] = [UserName],
    [DisplayName],
    [IconUrl]
FROM [dbo].[Users]
WHERE
    [UserName] = @Name
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results =
                    (await cn.QueryAsync<User>(sql, new { Name = name }).ConfigureAwait(false)).ToArray();

                return results.Any() ? results.First() : null;
            }
        }

        public Task<bool> ExistAsync(string name)
        {
            return ExistAsync(name, CancellationToken.None);
        }

        public async Task<bool> ExistAsync(string name, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT [Id]
FROM [dbo].[Users]
WHERE
    [UserName] = @Name
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results = await cn.QueryAsync(sql, new { Name = name }).ConfigureAwait(false);

                return results.Any();
            }
        }

        public Task SaveAsync(User user)
        {
            return SaveAsync(user, CancellationToken.None);
        }

        public async Task SaveAsync(User user, CancellationToken cancellationToken)
        {
            const string sql = @"
IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @Id)
BEGIN
    UPDATE [dbo].[Users]
    SET [UserName] = @Name,
        [DisplayName] = @DisplayName,
        [IconUrl] = @IconUrl
    WHERE
        [Id] = @Id
END
ELSE
BEGIN
    INSERT INTO [dbo].[Users]
    VALUES (
        @Id,
        @LogonName,
        @Name,
        @DisplayName,
        @IconUrl
    )
END
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, user).ConfigureAwait(false);
            }
        }
    }
}
