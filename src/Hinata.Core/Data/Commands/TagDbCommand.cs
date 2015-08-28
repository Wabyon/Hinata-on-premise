using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Hinata.Data.Commands
{
    public class TagDbCommand : DbCommand
    {
        public TagDbCommand(string connectionString) : base(connectionString)
        {
        }

        public Task<Tag> FindAsync(string name)
        {
            return FindAsync(name, CancellationToken.None);
        }

        public async Task<Tag> FindAsync(string name, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
     ItemTags.Name
    ,[AllItemCount] = COUNT(*)
    ,[PublicItemCount] = SUM(CASE WHEN Items.IsPublic = 1 THEN 1 ELSE 0 END)
    ,[PrivateItemCount] = SUM(CASE WHEN Items.IsPublic = 0 THEN 1 ELSE 0 END)
FROM [dbo].[ItemTags] ItemTags
INNER JOIN [dbo].[Items] Items
ON  ItemTags.ItemId = Items.Id
WHERE
    ItemTags.Name = @Name
GROUP BY ItemTags.Name
";
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                return (await cn.QueryAsync<Tag>(sql, new {Name = name}).ConfigureAwait(false)).FirstOrDefault();
            }
        }

        public Task<Tag[]> GetAllAsync()
        {
            return GetAllAsync(CancellationToken.None);
        }

        public async Task<Tag[]> GetAllAsync(CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
     ItemTags.Name
    ,[AllItemCount] = COUNT(*)
    ,[PublicItemCount] = SUM(CASE WHEN Items.IsPublic = 1 THEN 1 ELSE 0 END)
    ,[PrivateItemCount] = SUM(CASE WHEN Items.IsPublic = 0 THEN 1 ELSE 0 END)
FROM [dbo].[ItemTags] ItemTags
INNER JOIN [dbo].[Items] Items
ON  ItemTags.ItemId = Items.Id
GROUP BY ItemTags.Name
";

            using (var cn = CreateConnection())
            {
                await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

                return (await cn.QueryAsync<Tag>(sql).ConfigureAwait(false)).ToArray();
            }
        }
    }
}
