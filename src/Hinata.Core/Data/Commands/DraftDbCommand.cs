using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Hinata.Data.Models;

namespace Hinata.Data.Commands
{
    public class DraftDbCommand : DbCommand
    {
        public DraftDbCommand(string connectionString) : base(connectionString)
        {
        }

        public Task<Draft> FindAsync(string id)
        {
            return FindAsync(id, CancellationToken.None);
        }

        public async Task<Draft> FindAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id is null or empty", "id");

            var sql = string.Format(@"
{0}
WHERE
    Drafts.[Id] = @Id
", SqlSelectStatement);
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results =
                    (await cn.QueryAsync<DraftSelectDataModel>(sql, new {Id = id}).ConfigureAwait(false)).ToArray();

                return results.Any() ? results.First().ToEntity() : null;
            }
        }

        public Task<Draft[]> GetByAuthorAsync(User author)
        {
            return GetByAuthorAsync(author, CancellationToken.None);
        }

        public async Task<Draft[]> GetByAuthorAsync(User author, CancellationToken cancellationToken)
        {
            if (author == null) throw new ArgumentNullException("author");

            var sql = string.Format(@"
{0}
WHERE
    Drafts.[UserId] = @UserId
ORDER BY
    Drafts.[LastModifiedDateTime] DESC
", SqlSelectStatement);
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results =
                    (await cn.QueryAsync<DraftSelectDataModel>(sql, new { UserId = author.Id }).ConfigureAwait(false));

                return results.Select(x => x.ToEntity()).ToArray();
            }
        }

        public Task SaveAsync(Draft draft)
        {
            return SaveAsync(draft, CancellationToken.None);
        }

        public async Task SaveAsync(Draft draft, CancellationToken cancellationToken)
        {
            if (draft == null) throw new ArgumentNullException("draft");

            const string sqlDraft = @"
DELETE FROM [dbo].[DraftTags]
WHERE
    [DraftId] = @Id
;

IF EXISTS (SELECT * FROM [dbo].[Drafts] WHERE [Id] = @Id)
BEGIN
    UPDATE [dbo].[Drafts]
    SET [UserId] = @UserId,
        [Type] = @Type,
        [Title] = @Title,
        [Body] = @Body,
        [Comment] = @Comment,
        [LastModifiedDateTime] = @LastModifiedDateTime
    WHERE
        [Id] = @Id
END
ELSE
BEGIN
    INSERT INTO [dbo].[Drafts] (
        [Id],
        [UserId],
        [Type],
        [Title],
        [Body],
        [Comment],
        [LastModifiedDateTime]
    ) VALUES (
        @Id,
        @UserId,
        @Type,
        @Title,
        @Body,
        @Comment,
        @LastModifiedDateTime
    )
END
";
            const string sqlTags = @"
INSERT INTO [dbo].[DraftTags] (
    [DraftId],
    [Name],
    [Version],
    [OrderNo]
) VALUES (
    @DraftId,
    @Name,
    @Version,
    @OrderNo
)
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var tr = cn.BeginTransaction())
                {
                    try
                    {
                        await cn.ExecuteAsync(sqlDraft, new DraftRegisterDataModel(draft), tr).ConfigureAwait(false);
                        var orderNo = 1;
                        foreach (var tag in draft.ItemTags)
                        {
                            await cn.ExecuteAsync(sqlTags, new {DraftId = draft.Id, tag.Name, tag.Version, OrderNo = orderNo}, tr).ConfigureAwait(false);
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
        }

        public Task DeleteAsync(string id)
        {
            return DeleteAsync(id, CancellationToken.None);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id is null or empty.", "id");

            const string sql = @"
DELETE FROM [dbo].[DraftTags]
WHERE
    [DraftId] = @Id


DELETE FROM [dbo].[Drafts]
WHERE
    [Id] = @Id
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
            }
        }

        public Task DeleteByAuthorAsync(User author)
        {
            return DeleteByAuthorAsync(author, CancellationToken.None);
        }

        public async Task DeleteByAuthorAsync(User author, CancellationToken cancellationToken)
        {
            if (author == null) throw new ArgumentNullException("author");

            const string sql = @"
DELETE DraftTags FROM [dbo].[DraftTags] DraftTags
WHERE EXISTS (
    SELECT *
    FROM [dbo].[Drafts] Drafts
    WHERE
        DraftTags.DraftId = Drafts.Id
    AnD Drafts.[UserId] = @UserId
)

DELETE FROM [dbo].[Drafts]
WHERE
    [UserId] = @UserId
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new {UserId = author.Id}).ConfigureAwait(false);
            }
        }

        private const string SqlSelectStatement = @"
SELECT
    Drafts.[Id],
    [Author].Author,
    Drafts.[Type],
    Drafts.[Title],
    Drafts.[Body],
    Drafts.[Comment],
    Drafts.[LastModifiedDateTime],
    CurrentRevisionNo = CASE WHEN _Revisions.RevisionNo IS NULL THEN -1 ELSE _Revisions.RevisionNo END,
    Tags.Tags,
    [ItemIsPublic] = CONVERT(BIT,ISNULL(Items.IsPublic, 0)),
    [ItemCreatedDateTime] = Items.CreatedDateTime,
    [ItemRevisionCount] = _Revisions.RevisionCount
FROM [dbo].[Drafts] Drafts
LEFT OUTER JOIN [dbo].[Items] Items
ON  Drafts.Id = Items.Id
OUTER APPLY (
    SELECT (
        SELECT * FROM (
            SELECT
                Users.[Id],
                Users.[LogonName],
                [Name] = Users.UserName,
                Users.[DisplayName]
            FROM [dbo].[Users] Users
            WHERE
                Users.Id = Drafts.UserId
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
            FROM [dbo].[DraftTags] Tags
            WHERE
                Tags.DraftId = Drafts.Id
        ) Tag
        ORDER BY
            Tag.OrderNo
         FOR XML AUTO, ROOT('Tags')
    ) Tags
) Tags
OUTER APPLY (
    SELECT
        RevisionCount = COUNT(*),
        RevisionNo = MAX(RevisionNo)
    FROM [dbo].[ItemRevisions] ItemRevisions
    WHERE
        ItemRevisions.ItemId = Items.Id
) _Revisions
";
    }
}
