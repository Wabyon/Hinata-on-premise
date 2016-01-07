using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2016010701)]
    public class DeleteColumnItemType : Migration
    {
        public override void Up()
        {
            Delete.Column("Type").FromTable("Drafts");
            Delete.Column("Type").FromTable("ItemRevisions");
            Delete.Column("Type").FromTable("Items");
        }
        public override void Down()
        {
            Create.Column("Type").OnTable("Items").AsByte().Nullable();
            Execute.Sql("UPDATE Items SET Type = 0");
            Alter.Column("Type").OnTable("Items").AsByte().NotNullable();

            Create.Column("Type").OnTable("ItemRevisions").AsByte().Nullable();
            Execute.Sql("UPDATE ItemRevisions SET Type = 0");
            Alter.Column("Type").OnTable("Items").AsByte().NotNullable();

            Create.Column("Type").OnTable("Drafts").AsByte().Nullable();
            Execute.Sql("UPDATE Drafts SET Type = 0");
            Alter.Column("Type").OnTable("Items").AsByte().NotNullable();
        }
    }
}
