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

        public Task<Draft> FindAsync(string id, User user)
        {
            return FindAsync(id, user, CancellationToken.None);
        }

        public async Task<Draft> FindAsync(string id, User user, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id is null or empty", "id");
            if (user == null) throw new ArgumentNullException("user");

            var sql = string.Format(@"
{0}
WHERE
    Drafts.[Id] = @Id
AND Drafts.[UserId] = @UserId
", SqlSelectStatement);
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                var results =
                    (await cn.QueryAsync<DraftSelectDataModel>(sql, new {Id = id, UserId = user.Id}).ConfigureAwait(false)).ToArray();

                return results.Any() ? results.First().ToEntity() : null;
            }
        }

        public Task<Draft[]> GetByUserAsync(User user)
        {
            return GetByUserAsync(user, CancellationToken.None);
        }

        public async Task<Draft[]> GetByUserAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException("user");

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
                    (await cn.QueryAsync<DraftSelectDataModel>(sql, new { UserId = user.Id }).ConfigureAwait(false));

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
AND [UserId] = @UserId
;

IF EXISTS (SELECT * FROM [dbo].[Drafts] WHERE [Id] = @Id AND [UserId] = @UserId)
BEGIN
    UPDATE [dbo].[Drafts]
    SET [Type] = @Type,
        [Title] = @Title,
        [Body] = @Body,
        [Comment] = @Comment,
        [LastModifiedDateTime] = @LastModifiedDateTime
    WHERE
        [Id] = @Id
    AND [UserId] = @UserId
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
    [UserId],
    [Name],
    [Version],
    [OrderNo]
) VALUES (
    @DraftId,
    @UserId,
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
                            await cn.ExecuteAsync(sqlTags, new {DraftId = draft.Id, UserId = draft.Editor.Id, tag.Name, tag.Version, OrderNo = orderNo}, tr).ConfigureAwait(false);
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

        public Task DeleteAsync(string id, User user)
        {
            return DeleteAsync(id, user, CancellationToken.None);
        }

        public async Task DeleteAsync(string id, User user, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id is null or empty.", "id");
            if (user == null) throw new ArgumentNullException("user");

            const string sql = @"
DELETE FROM [dbo].[DraftTags]
WHERE
    [DraftId] = @Id
AND [UserId] = @UserId


DELETE FROM [dbo].[Drafts]
WHERE
    [Id] = @Id
AND [UserId] = @UserId
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new { Id = id, UserId = user.Id }).ConfigureAwait(false);
            }
        }

        public Task DeleteByUserAsync(User user)
        {
            return DeleteByUserAsync(user, CancellationToken.None);
        }

        public async Task DeleteByUserAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException("user");

            const string sql = @"
DELETE FROM [dbo].[DraftTags]
WHERE
    [UserId] = @UserId

DELETE FROM [dbo].[Drafts]
WHERE
    [UserId] = @UserId
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
                await cn.ExecuteAsync(sql, new {UserId = user.Id}).ConfigureAwait(false);
            }
        }

        private const string SqlSelectStatement = @"
SELECT
    Drafts.[Id],
    Author = ISNULL([Author].Author, [NewAuthor].Author),
    [Editor].Editor,
    Drafts.[Type],
    Drafts.[Title],
    Drafts.[Body],
    Drafts.[Comment],
    Drafts.[LastModifiedDateTime],
    CurrentRevisionNo = CASE WHEN _Revisions.RevisionNo IS NULL THEN -1 ELSE _Revisions.RevisionNo END,
    Tags.Tags,
    [ItemIsPublic] = CONVERT(BIT,ISNULL(Items.IsPublic, 0)),
    [ItemIsFreeEditable] = CONVERT(BIT,ISNULL(Items.IsFreeEditable, 0)),
    [ItemCreatedDateTime] = Items.CreatedDateTime,
    [ItemRevisionCount] = _Revisions.RevisionCount,
    Collaborators.Collaborators,
    [PublishedBody] = Items.Body
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
        ) [Editor]
        FOR XML AUTO
    ) [Editor]
) [Editor]
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
                Users.[DisplayName]
            FROM [dbo].[Users] Users
            WHERE
                Users.Id = Drafts.UserId
        ) [Author]
        FOR XML AUTO
    ) [Author]
) [NewAuthor]
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
            AND Tags.UserId = Drafts.UserId
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
";
    }
}
