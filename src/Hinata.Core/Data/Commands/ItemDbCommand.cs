﻿using System;
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

        public Task<Item[]> GetAllAsync()
        {
            return GetAllAsync(CancellationToken.None);
        }

        public async Task<Item[]> GetAllAsync(CancellationToken cancellationToken)
        {
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(SqlSelectStatement).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }
        }

        public Task<Item[]> GetByIdsAsync(string[] ids, int skip, int take)
        {
            return GetByIdsAsync(ids, skip, take, CancellationToken.None);
        }

        public async Task<Item[]> GetByIdsAsync(string[] ids, int skip, int take, CancellationToken cancellationToken)
        {
            if (ids == null) throw new ArgumentNullException("ids");
            if (!ids.Any()) return new Item[0];

            var idsParam = string.Join(@",", ids);

            var sql = string.Format(@"
WITH SplitPositions
AS (
    SELECT
        StartPosition =  CONVERT(INT, 0),
        EndPosition = CHARINDEX(',', @Ids)

    UNION ALL

    SELECT
        CONVERT(INT,EndPosition + 1),
        CHARINDEX(',',@Ids,EndPosition + 1)
    FROM SplitPositions
    WHERE
        EndPosition > 0
)
,Split
AS (
    SELECT
        Id = SUBSTRING(@Ids, StartPosition, COALESCE(NULLIF(EndPosition, 0), LEN(@Ids) + 1) - StartPosition)
    FROM SplitPositions
)
{0}
WHERE EXISTS (
    SELECT *
    FROM Split
    WHERE
        Split.Id = Items.Id
)
ORDER BY
    _LastModifiedDateTime.[LastModifiedDateTime] DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql, new { Ids = idsParam, Skip = skip, Take = take }).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
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
    PublicType.[PublicType] = 1
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
    PublicType.[PublicType] = 1
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
    PublicType.[PublicType] = 1
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
    Items.[CreateUserId] = @UserId
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
INNER JOIN
    [dbo].[fnItemPublicType]() AS [PublicType]
ON
    PublicType.ItemId = Items.Id
WHERE
    PublicType.PublicType = 1
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<int>(sql).ConfigureAwait(false)).ToArray().FirstOrDefault();
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
INNER JOIN
    [dbo].[fnItemPublicType]() AS [PublicType]
ON
    PublicType.ItemId = Items.Id
WHERE
    PublicType.PublicType = 1
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
    SET [CreateUserId] = @CreateUserId,
        [LastModifyUserId] = @LastModifyUserId,
        [IsPublic] = @IsPublic,
        [Title] = @Title,
        [Body] = @Body,
        [CreatedDateTime] = @CreatedDateTime,
        [LastModifiedDateTime] = @LastModifiedDateTime,
        [PublishSince] = @PublishSince,
        [PublishUntil] = @PublishUntil
    WHERE
        [Id] = @Id
END
ELSE
BEGIN
    INSERT INTO [dbo].[Items] (
        [Id],
        [CreateUserId],
        [LastModifyUserId],
        [IsPublic],
        [Title],
        [Body],
        [CreatedDateTime],
        [LastModifiedDateTime],
        [PublishSince],
        [PublishUntil]
    ) VALUES (
        @Id,
        @CreateUserId,
        @LastModifyUserId,
        @IsPublic,
        @Title,
        @Body,
        @CreatedDateTime,
        @LastModifiedDateTime,
        @PublishSince,
        @PublishUntil
    )
END

INSERT INTO [dbo].[ItemRevisions] (
    [ItemId],
    [RevisionNo],
    [CreateUserId],
    [ModifyUserId],
    [IsPublic],
    [Title],
    [Body],
    [Comment],
    [CreatedDateTime],
    [ModifiedDateTime]
) VALUES (
    @Id,
    @RevisionNo,
    @CreateUserId,
    @LastModifyUserId,
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
        ,Author
        ,Editor
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
                    Users.Id = ItemRevisions.CreateUserId
            ) [Author]
            FOR XML AUTO
        ) [Author]
    ) [Author]
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
                    Users.Id = ItemRevisions.ModifyUserId
            ) [Editor]
            FOR XML AUTO
        ) [Editor]
    ) [Editor]
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
    ,Author
    ,Editor
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
        ,Author
        ,Editor
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
                    Users.Id = ItemRevisions.CreateUserId
            ) [Author]
            FOR XML AUTO
        ) [Author]
    ) [Author]
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
                    Users.Id = ItemRevisions.ModifyUserId
            ) [Editor]
            FOR XML AUTO
        ) [Editor]
    ) [Editor]
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
    ,Author
    ,Editor
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

        public Task<Item[]> GetNotIndexedItemsAsync()
        {
            return GetNotIndexedItemsAsync(CancellationToken.None);
        }

        public async Task<Item[]> GetNotIndexedItemsAsync(CancellationToken cancellationToken)
        {
            var sql = string.Format(@"
{0}
WHERE NOT EXISTS (
    SELECT *
    FROM [dbo].[ItemIndexCreatedLogs]
    WHERE
        ItemIndexCreatedLogs.ItemId = Items.Id
    AND ItemIndexCreatedLogs.IndexCreatedDateTime >= Items.LastModifiedDateTime
)
", SqlSelectStatement);

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                return (await cn.QueryAsync<ItemSelectDataModel>(sql).ConfigureAwait(false))
                    .Select(x => x.ToEntity())
                    .ToArray();
            }

        }

        /// <summary></summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Task SaveCollaboratorsAsync(Item item)
        {
            return SaveCollaboratorsAsync(item, CancellationToken.None);
        }

        /// <summary></summary>
        /// <param name="item"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SaveCollaboratorsAsync(Item item, CancellationToken cancellationToken)
        {
            return SaveCollaboratorsAsync(item, item.Collaborators.ToArray(), cancellationToken);
        }

        /// <summary>記事の共同編集者を更新します。</summary>
        /// <param name="item">共同編集者を更新する記事</param>
        /// <param name="collaborators">共同編集者</param>
        /// <returns></returns>
        public Task SaveCollaboratorsAsync(Item item, Collaborator[] collaborators)
        {
            return SaveCollaboratorsAsync(item, collaborators, CancellationToken.None);
        }

        /// <summary>記事の共同編集者を更新します。</summary>
        /// <param name="item">共同編集者を更新する記事</param>
        /// <param name="collaborators">共同編集者</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>        
        public async Task SaveCollaboratorsAsync(Item item, Collaborator[] collaborators, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (collaborators == null) throw new ArgumentNullException("collaborators");

            const string delete = @"
DELETE FROM [dbo].[Collaborators]
WHERE [ItemId] = @ItemId
";

            const string insert = @"
INSERT INTO [dbo].[Collaborators] (
    [ItemId],
    [UserId],
    [RoleType]
) VALUES (
    @ItemId,
    @UserId,
    @RoleType
)
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var tr = cn.BeginTransaction())
                {
                    try
                    {
                        await cn.ExecuteAsync(delete, new {ItemId = item.Id}, tr).ConfigureAwait(false);

                        foreach (var collaborator in collaborators)
                        {
                            await
                                cn.ExecuteAsync(insert, new { ItemId = item.Id, UserId = collaborator.Id, RoleType = collaborator.Role }, tr)
                                    .ConfigureAwait(false);
                        }

                        tr.Commit();

                        item.ClearAllCollaborators();
                        foreach (var collaborator in collaborators)
                        {
                            item.AddCollaborator(collaborator);
                        }
                    }
                    catch
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary></summary>
        /// <param name="item"></param>
        /// <param name="collaborator"></param>
        /// <returns></returns>
        public Task AddCollaboratorAsync(Item item, Collaborator collaborator)
        {
            return AddCollaboratorAsync(item, collaborator, CancellationToken.None);
        }

        /// <summary></summary>
        /// <param name="item"></param>
        /// <param name="collaborator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddCollaboratorAsync(Item item, Collaborator collaborator, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (collaborator == null) throw new ArgumentNullException("collaborator");
            if (item.Collaborators.Contains(collaborator)) throw new InvalidOperationException("target user is already included in collaborators.");

            const string insert = @"
INSERT INTO [dbo].[Collaborators] (
    [ItemId],
    [UserId],
    [RoleType]
) VALUES (
    @ItemId,
    @UserId,
    @RoleType
)
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                await
                    cn.ExecuteAsync(insert, new { ItemId = item.Id, UserId = collaborator.Id, RoleType = collaborator.Role })
                        .ConfigureAwait(false);
            }

            item.AddCollaborator(collaborator);
        }

        /// <summary></summary>
        /// <param name="item"></param>
        /// <param name="collaborator"></param>
        /// <returns></returns>
        public Task RemoveCollaboratorAsync(Item item, Collaborator collaborator)
        {
            return RemoveCollaboratorAsync(item, collaborator, CancellationToken.None);
        }

        /// <summary></summary>
        /// <param name="item"></param>
        /// <param name="collaborator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RemoveCollaboratorAsync(Item item, Collaborator collaborator, CancellationToken cancellationToken)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (collaborator == null) throw new ArgumentNullException("collaborator");
            if (!item.Collaborators.Contains(collaborator)) throw new InvalidOperationException("target user is not included in collaborators.");

            const string delete = @"
DELETE FROM [dbo].[Collaborators]
WHERE
    [ItemId] = @ItemId
AND [UserId] = @UserId
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                await
                    cn.ExecuteAsync(delete, new { ItemId = item.Id, UserId = collaborator.Id })
                        .ConfigureAwait(false);
            }

            item.RemoveCollaborator(collaborator);
        }

        #region SqlSelectStatement
        private const string SqlSelectStatement = @"
SELECT
    Items.[Id],
    Items.[IsPublic],
    [Author].Author,
    [Editor].Editor,
    Items.[Title],
    Items.[Body],
    Items.[CreatedDateTime],
    Items.[LastModifiedDateTime],
    Items.[PublishSince],
    Items.[PublishUntil],
    Tags.Tags,
    [ItemIsPublic] = CONVERT(BIT,ISNULL(Items.IsPublic, 0)),
    [ItemCreatedDateTime] = Items.CreatedDateTime,
    _CommentAttributes.[CommentCount],
    _Revisions.[RevisionCount],
    _Revisions.[RevisionNo],
    Collaborators.Collaborators,
    [PublicType].[PublicType]
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
                Users.Id = Items.CreateUserId
        ) [Author]
        FOR XML AUTO
    ) [Author]
) [Author]
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
                Users.Id = Items.LastModifyUserId
        ) [Editor]
        FOR XML AUTO
    ) [Editor]
) [Editor]
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
OUTER APPLY (
    SELECT (
        SELECT * FROM (
            SELECT
                Users.[Id],
                Users.[LogonName],
                [Name] = Users.UserName,
                Users.[DisplayName],
                Users.[IconUrl],
                [Role] = Collaborators.[RoleType]
            FROM [dbo].[Collaborators] Collaborators
            INNER JOIN [dbo].[Users] Users
            ON  Collaborators.UserId = Users.Id
            WHERE
                Collaborators.ItemId = Items.Id
        ) Collaborator
        ORDER BY
            Collaborator.[Name]
         FOR XML AUTO, ROOT('Collaborators')
    ) Collaborators
) [Collaborators]
INNER JOIN
    [dbo].[fnItemPublicType]() AS [PublicType]
ON
    PublicType.ItemId = Items.Id
";
        #endregion
    }
}
