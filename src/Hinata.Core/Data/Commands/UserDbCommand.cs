using System;
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

        public Task<User[]> SearchAsync(string[] searchText)
        {
            return SearchAsync(searchText, CancellationToken.None);
        }

        public async Task<User[]> SearchAsync(string[] searchText, CancellationToken cancellationToken)
        {
            if (searchText == null) throw new ArgumentNullException("searchText");

            const string sql = @"
WITH SplitPositions
AS (
    SELECT
        StartPosition =  CONVERT(INT, 0),
        EndPosition = CHARINDEX(',', @SearchText)

    UNION ALL

    SELECT
        CONVERT(INT,EndPosition + 1),
        CHARINDEX(',',@SearchText,EndPosition + 1)
    FROM SplitPositions
    WHERE
        EndPosition > 0
)
,Split
AS (
    SELECT
        Text = SUBSTRING(@SearchText, StartPosition, COALESCE(NULLIF(EndPosition, 0), LEN(@SearchText) + 1) - StartPosition)
    FROM SplitPositions
)
SELECT
    [Id] = Users.[Id],
    [LogonName],
    [Name] = Users.[UserName],
    [DisplayName],
    [IconUrl]
FROM Users
INNER JOIN ( 
    SELECT Users.Id
    FROM Users
    CROSS APPLY Split
    WHERE
        Users.LogonName LIKE '%' + Split.Text + '%' COLLATE Japanese_CI_AS
    OR  Users.UserName LIKE '%' + Split.Text + '%' COLLATE Japanese_CI_AS
    OR  Users.DisplayName LIKE '%' + Split.Text + '%' COLLATE Japanese_CI_AS
    GROUP BY
        Users.Id
    HAVING COUNT(*) = (SELECT COUNT(*) FROM Split)
) Candidate
ON  Users.Id = Candidate.Id
ORDER BY
    Users.[UserName]
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results =
                    (await cn.QueryAsync<User>(sql, new { SearchText = string.Join(",", searchText) }).ConfigureAwait(false)).ToArray();

                return results.Any() ? results.ToArray() : new User[0];
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
