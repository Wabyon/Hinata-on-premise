using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Hinata.Data.Models;

namespace Hinata.Data.Commands
{
    public class CommentDbCommand : DbCommand
    {
        public CommentDbCommand(string connectionString) : base(connectionString)
        {
        }

        public Task<Comment> FindAsync(string id)
        {
            return FindAsync(id, CancellationToken.None);
        }

        public async Task<Comment> FindAsync(string id, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    Comments.[Id],
    Comments.[ItemId],
    [User].[User],
    Comments.[Body],
    Comments.[CreatedDateTime],
    Comments.[LastModifiedDateTime]
FROM [dbo].[Comments] Comments
OUTER APPLY (
    SELECT (
        SELECT * FROM (
            SELECT
                Users.[Id],
                Users.[LogonName],
                [Name] = Users.UserName,
                Users.[DisplayName],
                Users.[IconUrl]
            FROM [dbo].[Users] Users
            WHERE
                Users.Id = Comments.UserId
        ) [User]
        FOR XML AUTO
    ) [User]
) [User]
WHERE
    Comments.[Id] = @Id
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                var results = (await cn.QueryAsync<CommentSelectDataModel>(sql, new { Id = id }).ConfigureAwait(false)).ToArray();
                return results.Any() ? results.First().ToEntity() : null;
            }
        }

        public Task<Comment[]> GetByItemAsync(Item item)
        {
            return GetByItemAsync(item, CancellationToken.None);
        }

        public async Task<Comment[]> GetByItemAsync(Item item, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");

            const string sql = @"
SELECT
    Comments.[Id],
    Comments.[ItemId],
    [User].[User],
    Comments.[Body],
    Comments.[CreatedDateTime],
    Comments.[LastModifiedDateTime]
FROM [dbo].[Comments] Comments
OUTER APPLY (
    SELECT (
        SELECT * FROM (
            SELECT
                Users.[Id],
                Users.[LogonName],
                [Name] = Users.UserName,
                Users.[DisplayName],
                Users.[IconUrl]
            FROM [dbo].[Users] Users
            WHERE
                Users.Id = Comments.UserId
        ) [User]
        FOR XML AUTO
    ) [User]
) [User]
WHERE
    Comments.[ItemId] = @ItemId
ORDER BY
    Comments.[CreatedDateTime]
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                var results =
                    (await cn.QueryAsync<CommentSelectDataModel>(sql, new {ItemId = item.Id}).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();

                return results;
            }
        }

        public Task SaveAsync(Comment comment)
        {
            return SaveAsync(comment, CancellationToken.None);
        }

        public async Task SaveAsync(Comment comment, CancellationToken cancellationToken)
        {
            if (comment == null) throw new ArgumentNullException("comment");

            const string sql = @"
IF EXISTS (SELECT * FROM [dbo].[Comments] WHERE [Id] = @Id)
BEGIN
    UPDATE Comments
    SET Comments.[UserId] = @UserId,
        Comments.[ItemId] = @ItemId,
        Comments.[Body] = @Body,
        Comments.[CreatedDateTime] = @CreatedDateTime,
        Comments.[LastModifiedDateTime] = @LastModifiedDateTime
    FROM [dbo].[Comments] Comments
    WHERE
        Comments.[Id] = @Id
END
ELSE
BEGIN
    INSERT INTO [dbo].[Comments] (
        [Id],
        [UserId],
        [ItemId],
        [Body],
        [CreatedDateTime],
        [LastModifiedDateTime]
    ) VALUES (
        @Id,
        @UserId,
        @ItemId,
        @Body,
        @CreatedDateTime,
        @LastModifiedDateTime
    )
END
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new CommentRegisterDataModel(comment)).ConfigureAwait(false);
            }
        }

        public Task DeleteAsync(string id)
        {
            return DeleteAsync(id, CancellationToken.None);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            var item = await FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (item == null) throw new InvalidOperationException("item is not found.");

            const string sql = @"
DELETE Comments
FROM [dbo].[Comments] Comments
WHERE
    Comments.[Id] = @Id
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new {Id = id}).ConfigureAwait(false);
            }
        }
    }
}
