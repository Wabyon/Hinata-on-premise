using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Hinata.Data.Models;

namespace Hinata.Data.Commands
{
    public class LikeDbCommand : DbCommand
    {
        public LikeDbCommand(string connectionString)
            : base(connectionString)
        {
        }

        public Task<Like> FindAsync(string id)
        {
            return FindAsync(id, CancellationToken.None);
        }

        public async Task<Like> FindAsync(string id, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    [Likes].[Id],
    [Likes].[ItemId],
    [Likes].[UserId]
FROM [dbo].[Likes] 
WHERE
    [Likes].[Id] = @Id
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                var resultSet = (await cn.QueryAsync<LikeSelectDataModel>(sql, new { Id = id }).ConfigureAwait(false)).ToArray();
                return resultSet.Any() ? resultSet.First().ToEntity() : null;
            }
        }

        public Task<Like[]> GetByItemAsync(Item item)
        {
            return GetByItemAsync(item, CancellationToken.None);
        }

        public async Task<Like[]> GetByItemAsync(Item item, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");

            const string sql = @"
SELECT
    [Likes].[Id],
    [Likes].[ItemId],
    [Likes].[UserId]
FROM [dbo].[Likes] 
WHERE
    [Likes].[ItemId] = @ItemId
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                var resultSet = (await cn.QueryAsync<LikeSelectDataModel>(sql, new { ItemId = item.Id }).ConfigureAwait(false)).ToArray();
                return resultSet.Select(r => r.ToEntity()).ToArray();
            }
        }

        public Task<Like> GetByItemAndUserAsync(Item item, User user)
        {
            return GetByItemAndUserAsync(item, user, CancellationToken.None);
        }

        public async Task<Like> GetByItemAndUserAsync(Item item, User user, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (user == null) throw new ArgumentNullException("user");


            const string sql = @"
SELECT
    [Likes].[Id],
    [Likes].[ItemId],
    [Likes].[UserId]
FROM [dbo].[Likes] 
WHERE
    [Likes].[ItemId] = @ItemId
AND [Likes].[UserId] = @UserId
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                var resultSet = (await cn.QueryAsync<LikeSelectDataModel>(sql, new { ItemId = item.Id, UserID = user.Id }).ConfigureAwait(false)).ToArray();
                return resultSet.Any() ? resultSet.First().ToEntity() : null;
            }
        }

        public Task AddLikeAsync(Like like)
        {
            return AddLikeAsync(like, CancellationToken.None);
        }

        public async Task AddLikeAsync(Like like, CancellationToken cancellationToken)
        {
            if (like == null) throw new ArgumentNullException("like");

            const string sql = @"
INSERT INTO [dbo].[Likes] (
    [Id],
    [ItemId],
    [UserId]
) VALUES (
    @Id,
    @ItemId,
    @UserId
)
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new LikeAddDataModel(like)).ConfigureAwait(false);
            }
        }

        public Task RemoveLikeAsync(string id)
        {
            return RemoveLikeAsync(id, CancellationToken.None);
        }

        public async Task RemoveLikeAsync(string id, CancellationToken cancellationToken)
        {
            var like = await FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (like == null) throw new InvalidOperationException("like is not found.");

            const string sql = @"
DELETE Likes
FROM [dbo].[Likes] Likes
WHERE
    Likes.[Id] = @Id
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
            }
        }
    }
}
