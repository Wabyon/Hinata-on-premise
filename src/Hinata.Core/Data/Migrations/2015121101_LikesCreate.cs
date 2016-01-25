using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2015121101, TransactionBehavior.None)]
    public class LikesCreate : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
CREATE TABLE [dbo].[Likes](
    [Id] VARCHAR(32) NOT NULL,
    [ItemId] VARCHAR(32) NOT NULL,
    [UserId] VARCHAR(32) NOT NULL,

    CONSTRAINT [PK_Likes] PRIMARY KEY ([ItemId], [UserId])
);
");
        }
        public override void Down()
        {
            Delete.Table("Likes").InSchema("dbo");
        }
    }
}