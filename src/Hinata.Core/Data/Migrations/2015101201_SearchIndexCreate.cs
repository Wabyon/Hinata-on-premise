using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2015101201)]
    public class SearchIndexCreate : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
CREATE TABLE [dbo].[ItemIndexCreatedLogs] (
    [ItemId] VARCHAR(32) NOT NULL,
    [IndexCreatedDateTime] DATETIME2(7) NOT NULL,
    [RevisionNo] INT NOT NULL DEFAULT(0),

    CONSTRAINT [PK_ItemIndexCreatedLogs] PRIMARY KEY ([ItemId], [IndexCreatedDateTime])
);
");

            Create.Index("IX_Items_ModifiedDateTime")
                .OnTable("Items")
                .InSchema("dbo")
                .OnColumn("Id")
                .Ascending()
                .OnColumn("LastModifiedDateTime")
                .Ascending();
        }

        public override void Down()
        {
            Execute.Sql(@"
DROP TABLE [dbo].[ItemIndexCreatedLogs];
");

            Delete.Index("IX_Items_ModifiedDateTime")
                .OnTable("Items")
                .InSchema("dbo");
        }
    }
}
