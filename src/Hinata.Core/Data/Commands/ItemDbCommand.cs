using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Hinata.Data.Models;

namespace Hinata.Data.Commands
{
    public class ItemDbCommand : DbCommand
    {
        public ItemDbCommand(string connectionString)
            : base(connectionString)
        {
        }

        public Task<Item> FindAsync(string id)
        {
            return FindAsync(id, CancellationToken.None);
        }

        public async Task<Item> FindAsync(string id, CancellationToken cancellationToken)
        {
            var sql = string.Format(@"
{0}
WHERE
    Items.[Id] = @Id
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                var results = (await cn.QueryAsync<ItemSelectDataModel>(sql, new { Id = id }).ConfigureAwait(false)).ToArray();
                return results.Any() ? results.First().ToEntity() : null;
            }
        }

        public Task<Item[]> GetPublicAsync(int skip, int take)
        {
            return GetPublicAsync(skip, take, CancellationToken.None);
        }

        public async Task<Item[]> GetPublicAsync(int skip, int take, CancellationToken cancellationToken)
        {
            var sql = string.Format(@"
{0}
WHERE
    Items.[IsPublic] = 1
ORDER BY
    _LastModifiedDateTime.[LastModifiedDateTime] DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql, new { Skip = skip, Take = take }).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }
        }

        public Task<Item[]> GetPublicAsync(ItemType itemType, int skip, int take)
        {
            return GetPublicAsync(itemType, skip, take, CancellationToken.None);
        }

        public async Task<Item[]> GetPublicAsync(ItemType itemType, int skip, int take, CancellationToken cancellationToken)
        {
            var sql = string.Format(@"
{0}
WHERE
    Items.[IsPublic] = 1
AND Items.[Type] = @ItemType
ORDER BY
    _LastModifiedDateTime.[LastModifiedDateTime] DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql, new { ItemType = itemType, Skip = skip, Take = take }).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }
        }

        public Task<Item[]> GetPublicByTagAsync(Tag tag, int skip, int take)
        {
            return GetPublicByTagAsync(tag, skip, take, CancellationToken.None);
        }

        public async Task<Item[]> GetPublicByTagAsync(Tag tag, int skip, int take, CancellationToken cancellationToken)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            var sql = string.Format(@"
{0}
WHERE
    Items.[IsPublic] = 1
AND EXISTS (
    SELECT *
    FROM [dbo].[ItemTags] _Tags
    WHERE
        _Tags.ItemId = Items.Id
    AND _Tags.Name = @TagName
)
ORDER BY
    _LastModifiedDateTime.[LastModifiedDateTime] DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql, new { TagName = tag.Name, Skip = skip, Take = take }).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }
        }

        public Task<Item[]> GetPublicNewerAsync(int skip, int take)
        {
            return GetPublicNewerAsync(skip, take, CancellationToken.None);
        }

        public async Task<Item[]> GetPublicNewerAsync(int skip, int take, CancellationToken cancellationToken)
        {
            var sql = string.Format(@"
{0}
WHERE
    Items.[IsPublic] = 1
ORDER BY
    Items.[CreatedDateTime] DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql, new { Skip = skip, Take = take }).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }
        }

        public Task<Item[]> GetPublicNewerAsync(ItemType itemType, int skip, int take)
        {
            return GetPublicNewerAsync(itemType, skip, take, CancellationToken.None);
        }

        public async Task<Item[]> GetPublicNewerAsync(ItemType itemType, int skip, int take, CancellationToken cancellationToken)
        {
            var sql = string.Format(@"
{0}
WHERE
    Items.[IsPublic] = 1
AND Items.[Type] = @ItemType
ORDER BY
    Items.[CreatedDateTime] DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql, new { ItemType = itemType, Skip = skip, Take = take }).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }
        }

        public Task<Item[]> GetByAuthorAsync(User author)
        {
            return GetByAuthorAsync(author, CancellationToken.None);
        }

        public async Task<Item[]> GetByAuthorAsync(User author, CancellationToken cancellationToken)
        {
            if (author == null) throw new ArgumentNullException("author");

            var sql = string.Format(@"
{0}
WHERE
    Items.[UserId] = @UserId
ORDER BY
    Items.[CreatedDateTime] DESC
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql, new { UserId = author.Id }).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }
        }

        public Task<int> CountPublicAsync()
        {
            return CountPublicAsync(CancellationToken.None);
        }

        public async Task<int> CountPublicAsync(CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    Count = Count(*)
FROM [dbo].[Items]
WHERE
    [IsPublic] = 1
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<int>(sql).ConfigureAwait(false)).ToArray().FirstOrDefault();
            }
        }

        public Task<int> CountPublicAsync(ItemType itemType)
        {
            return CountPublicAsync(itemType, CancellationToken.None);
        }

        public async Task<int> CountPublicAsync(ItemType itemType, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    Count = Count(*)
FROM [dbo].[Items]
WHERE
    [IsPublic] = 1
AND [Type] = @ItemType
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<int>(sql, new { ItemType = itemType }).ConfigureAwait(false)).ToArray().FirstOrDefault();
            }
        }

        public Task<int> CountPublicByTagAsync(Tag tag)
        {
            return CountPublicByTagAsync(tag, CancellationToken.None);
        }

        public async Task<int> CountPublicByTagAsync(Tag tag, CancellationToken cancellationToken)
        {
            if (tag == null) throw new ArgumentNullException("tag");

            const string sql = @"
SELECT
    Count = Count(*)
FROM [dbo].[Items]
WHERE
    [IsPublic] = 1
AND EXISTS (
    SELECT *
    FROM [dbo].[ItemTags] _Tags
    WHERE
        _Tags.ItemId = Items.Id
    AND _Tags.Name = @TagName
)
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<int>(sql, new { TagName = tag.Name }).ConfigureAwait(false)).ToArray().FirstOrDefault();
            }
        }

        public Task SaveAsync(Item item)
        {
            return SaveAsync(item, CancellationToken.None);
        }

        public async Task SaveAsync(Item item, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");

            const string sqlDraft = @"
DELETE FROM [dbo].[ItemTags]
WHERE
    [ItemId] = @Id
;

IF EXISTS (SELECT * FROM [dbo].[Items] WHERE [Id] = @Id)
BEGIN
    UPDATE [dbo].[Items]
    SET [UserId] = @UserId,
        [Type] = @Type,
        [IsPublic] = @IsPublic,
        [Title] = @Title,
        [Body] = @Body,
        [CreatedDateTime] = @CreatedDateTime,
        [LastModifiedDateTime] = @LastModifiedDateTime
    WHERE
        [Id] = @Id
END
ELSE
BEGIN
    INSERT INTO [dbo].[Items] (
        [Id],
        [UserId],
        [Type],
        [IsPublic],
        [Title],
        [Body],
        [CreatedDateTime],
        [LastModifiedDateTime]
    ) VALUES (
        @Id,
        @UserId,
        @Type,
        @IsPublic,
        @Title,
        @Body,
        @CreatedDateTime,
        @LastModifiedDateTime
    )
END

INSERT INTO [dbo].[ItemRevisions] (
    [ItemId],
    [RevisionNo],
    [UserId],
    [Type],
    [IsPublic],
    [Title],
    [Body],
    [Comment],
    [CreatedDateTime],
    [ModifiedDateTime]
) VALUES (
    @Id,
    @RevisionNo,
    @UserId,
    @Type,
    @IsPublic,
    @Title,
    @Body,
    @Comment,
    @CreatedDateTime,
    @LastModifiedDateTime
)

";
            const string sqlTags = @"
INSERT INTO [dbo].[ItemTags] (
    [ItemId],
    [Name],
    [Version],
    [OrderNo]
) VALUES (
    @ItemId,
    @Name,
    @Version,
    @OrderNo
);

INSERT INTO [dbo].[ItemTagRevisions] (
    [ItemId],
    [RevisionNo],
    [Name],
    [Version],
    [OrderNo]
) VALUES (
    @ItemId,
    @RevisionNo,
    @Name,
    @Version,
    @OrderNo
);
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var tr = cn.BeginTransaction())
                {
                    try
                    {
                        await cn.ExecuteAsync(sqlDraft, new ItemRegisterDataModel(item), tr).ConfigureAwait(false);
                        var orderNo = 1;
                        foreach (var tag in item.ItemTags)
                        {
                            await cn.ExecuteAsync(sqlTags, new { ItemId = item.Id, item.RevisionNo ,tag.Name, tag.Version, OrderNo = orderNo }, tr).ConfigureAwait(false);
                            orderNo++;
                        }

                        tr.Commit();
                    }
                    catch
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }

            item.RevisionCount++;
        }

        public Task DeleteAsync(string id)
        {
            return DeleteAsync(id, CancellationToken.None);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            var item = await FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (item == null) throw new InvalidOperationException("item not found.");

            const string sql = @"
DELETE FROM [dbo].[ItemTags]
WHERE
    [ItemId] = @Id
;

DELETE FROM [dbo].[Items]
WHERE
    [Id] = @Id
;
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new { item.Id }).ConfigureAwait(false);
            }
        }

        public Task<ItemRevision> FindRevisionAsync(string itemId, int revisionNo)
        {
            return FindRevisionAsync(itemId, revisionNo, CancellationToken.None);
        }

        public async Task<ItemRevision> FindRevisionAsync(string itemId, int revisionNo, CancellationToken cancellationToken)
        {
            const string sql = @"
WITH Main
AS (
    SELECT
         ItemId
        ,RevisionNo
        ,Comment
        ,ModifiedDateTime
        ,ItemRevisions.Title
        ,Tags = Tags.Tags
        ,ItemRevisions.Body
        ,FirstRevisionNo = MIN(RevisionNo) OVER (PARTITION BY ItemId)
        ,CurrentRevisionNo = MAX(RevisionNo) OVER (PARTITION BY ItemId)
    FROM [dbo].[ItemRevisions] ItemRevisions
    OUTER APPLY (
        SELECT (
            SELECT * FROM (
                SELECT
                    [Name],
                    [Version],
                    [OrderNo]
                FROM [dbo].[ItemTagRevisions] Tags
                WHERE
                    Tags.ItemId = ItemRevisions.ItemId
                AND Tags.RevisionNo = ItemRevisions.RevisionNo
            ) Tag
            ORDER BY
                Tag.OrderNo
             FOR XML AUTO, ROOT('Tags')
        ) Tags
    ) Tags
)
SELECT
     ItemId
    ,RevisionNo
    ,Comment
    ,ModifiedDateTime
    ,Title
    ,Tags
    ,Body
    ,IsFirst = CONVERT(BIT, CASE WHEN RevisionNo = FirstRevisionNo THEN 1 ELSE 0 END)
    ,IsCurrent = CONVERT(BIT, CASE WHEN RevisionNo = CurrentRevisionNo THEN 1 ELSE 0 END)
FROM Main
WHERE
    ItemId = @ItemId
AND RevisionNo = @RevisionNo
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var resultSet = (await cn.QueryAsync<ItemRevisionSelectDataModel>(sql, new { ItemId = itemId, RevisionNo = revisionNo }).ConfigureAwait(false)).FirstOrDefault();

                return resultSet == null ? null : resultSet.ToEntity();
            }
        }

        public Task<ItemRevision[]> GetRevisionsAsync(string itemId)
        {
            return GetRevisionsAsync(itemId, CancellationToken.None);
        }

        public async Task<ItemRevision[]> GetRevisionsAsync(string itemId, CancellationToken cancellationToken)
        {
            const string sql = @"
WITH Main
AS (
    SELECT
         ItemId
        ,RevisionNo
        ,Comment
        ,ModifiedDateTime
        ,ItemRevisions.Title
        ,Tags = Tags.Tags
        ,ItemRevisions.Body
        ,FirstRevisionNo = MIN(RevisionNo) OVER (PARTITION BY ItemId)
        ,CurrentRevisionNo = MAX(RevisionNo) OVER (PARTITION BY ItemId)
    FROM [dbo].[ItemRevisions] ItemRevisions
    OUTER APPLY (
        SELECT (
            SELECT * FROM (
                SELECT
                    [Name],
                    [Version],
                    [OrderNo]
                FROM [dbo].[ItemTagRevisions] Tags
                WHERE
                    Tags.ItemId = ItemRevisions.ItemId
                AND Tags.RevisionNo = ItemRevisions.RevisionNo
            ) Tag
            ORDER BY
                Tag.OrderNo
             FOR XML AUTO, ROOT('Tags')
        ) Tags
    ) Tags
)
SELECT
     ItemId
    ,RevisionNo
    ,Comment
    ,ModifiedDateTime
    ,Title
    ,Tags
    ,Body
    ,IsFirst = CONVERT(BIT, CASE WHEN RevisionNo = FirstRevisionNo THEN 1 ELSE 0 END)
    ,IsCurrent = CONVERT(BIT, CASE WHEN RevisionNo = CurrentRevisionNo THEN 1 ELSE 0 END)
FROM Main
WHERE
    ItemId = @ItemId
ORDER BY
     RevisionNo DESC
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results = await cn.QueryAsync<ItemRevisionSelectDataModel>(sql, new {ItemId = itemId}).ConfigureAwait(false);

                return results.Select(x => x.ToEntity()).ToArray();
            }
        }

        #region SqlSelectStatement
        private const string SqlSelectStatement = @"
SELECT
    Items.[Id],
    Items.[Type],
    Items.[IsPublic],
    [Author].Author,
    Items.[Title],
    Items.[Body],
    Items.[CreatedDateTime],
    Items.[LastModifiedDateTime],
    Tags.Tags,
    [ItemType] = ISNULL(Items.Type, 0),
    [ItemIsPublic] = CONVERT(BIT,ISNULL(Items.IsPublic, 0)),
    [ItemCreatedDateTime] = Items.CreatedDateTime,
    _CommentAttributes.[CommentCount],
    _Revisions.[RevisionCount],
    _Revisions.[RevisionNo]
FROM [dbo].[Items] Items
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
                Users.Id = Items.UserId
        ) [Author]
        FOR XML AUTO
    ) [Author]
) [Author]
OUTER APPLY (
    SELECT (
        SELECT * FROM (
            SELECT
                [Name],
                [Version],
                [OrderNo]
            FROM [dbo].[ItemTags] Tags
            WHERE
                Tags.ItemId = Items.Id
        ) Tag
        ORDER BY
            Tag.OrderNo
         FOR XML AUTO, ROOT('Tags')
    ) Tags
) Tags
OUTER APPLY (
    SELECT
        CommentCount = COUNT(*),
        LastModifiedDateTime = MAX([LastModifiedDateTime])
    FROM [dbo].[Comments] Comments
    WHERE
        Items.Id = Comments.ItemId
) _CommentAttributes
CROSS APPLY (
    SELECT
        LastModifiedDateTime =
            CASE WHEN _CommentAttributes.[LastModifiedDateTime] IS NULL
                    THEN Items.[LastModifiedDateTime]
                 ELSE
                    CASE WHEN _CommentAttributes.[LastModifiedDateTime] > Items.[LastModifiedDateTime]
                            THEN _CommentAttributes.[LastModifiedDateTime]
                         ELSE Items.[LastModifiedDateTime]
                         END
                 END
) _LastModifiedDateTime
OUTER APPLY (
    SELECT
        RevisionCount = COUNT(*),
        RevisionNo = MAX(RevisionNo)
    FROM [dbo].[ItemRevisions] ItemRevisions
    WHERE
        ItemRevisions.ItemId = Items.Id
) _Revisions
";
        #endregion
    }
}
