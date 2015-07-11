using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2)]
    public class AddUserIconUrl : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
ALTER TABLE [dbo].[Users] ADD [IconUrl] NVARCHAR(2048) NULL;
");
        }

        public override void Down()
        {
            Execute.Sql(@"
ALTER TABLE [dbo].[Users] DROP COLUMN [IconUrl];
");
        }
    }
}
