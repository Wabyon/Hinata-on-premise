using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2015112601)]
    public class PublicationScheduling : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
ALTER TABLE [dbo].[Items] ADD [PublishSince] DATETIME2(7) NULL;
ALTER TABLE [dbo].[Items] ADD [PublishUntil] DATETIME2(7) NULL;
GO

CREATE FUNCTION [dbo].[fnItemPublicType] ()
RETURNS TABLE
AS
RETURN
(
    SELECT
        [ItemId] = [Items].[Id],
        [PublicType] = CASE WHEN Items.IsPublic = 0 THEN 0
                            WHEN Items.PublishSince IS NOT NULL AND Items.PublishSince > GETDATE() THEN 0
                            WHEN Items.PublishUntil IS NOT NULL AND Items.PublishUntil < GETDATE() THEN 0
                            ELSE 1
                       END
    FROM [dbo].[Items] AS [Items]
);
GO
");
        }
        public override void Down()
        {
            Execute.Sql(@"
ALTER TABLE [dbo].[Items] DROP COLUMN [PublishSince];
ALTER TABLE [dbo].[Items] DROP COLUMN [PublishUntil];
DROP FUNCTION [dbo].[fnItemPublicType];
");
        }
    }
}
