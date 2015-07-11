using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(0)]
    public class CreateCoreTables : Migration
    {
        public override void Up()
        {
            #region Users
            Execute.Sql(@"
CREATE TABLE [dbo].[Users]
(
    [Id] VARCHAR(32) NOT NULL,
    [LogonName] NVARCHAR(256) NOT NULL,
    [UserName] NVARCHAR(20) NULL,
    [DisplayName] NVARCHAR(20) NULL,

    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
");

            Create.Index("IX_Users_LogonName").OnTable("Users").InSchema("dbo").OnColumn("LogonName").Unique();
            Create.Index("IX_Users_UserName").OnTable("Users").InSchema("dbo").OnColumn("UserName");
            #endregion

            #region Items
            Execute.Sql(@"
CREATE TABLE [dbo].[Items]
(
    [Id] VARCHAR(32) NOT NULL,
    [UserId] VARCHAR(32) NOT NULL,
    [Type] TINYINT NOT NULL,
    [IsPublic] BIT NOT NULL DEFAULT(0),
    [Title] NVARCHAR(256) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [CreatedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),
    [LastModifiedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),

    CONSTRAINT [PK_Items] PRIMARY KEY ([Id])
);
");
            Execute.Sql(@"
CREATE TABLE [dbo].[ItemTags]
(
    [ItemId] VARCHAR(32) NOT NULL,
    [Name] NVARCHAR(32) NOT NULL,
    [Version] NVARCHAR(16) NULL,
    [OrderNo] INT NOT NULL DEFAULT(0),

    CONSTRAINT [PK_ItemTags] PRIMARY KEY ([ItemId], [Name]),
    CONSTRAINT [FK_ItemTags_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE
);
");
            #endregion

            #region Drafts
            Execute.Sql(@"
CREATE TABLE [dbo].[Drafts]
(
    [Id] VARCHAR(32) NOT NULL,
    [UserId] VARCHAR(32) NOT NULL,
    [Type] TINYINT NOT NULL,
    [Title] NVARCHAR(256) NULL,
    [Body] NVARCHAR(MAX) NULL,
    [LastModifiedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),

    CONSTRAINT [PK_Drafts] PRIMARY KEY ([Id])
);
");
            Execute.Sql(@"
CREATE TABLE [dbo].[DraftTags]
(
    [DraftId] VARCHAR(32) NOT NULL,
    [Name] NVARCHAR(32) NOT NULL,
    [Version] NVARCHAR(16) NULL,
    [OrderNo] INT NOT NULL DEFAULT(0),

    CONSTRAINT [PK_DraftTags] PRIMARY KEY ([DraftId], [Name]),
    CONSTRAINT [FK_DraftTags_Drafts] FOREIGN KEY ([DraftId]) REFERENCES [dbo].[Drafts] ([Id]) ON DELETE CASCADE
);
");
            #endregion

            #region Comments
            Execute.Sql(@"
CREATE TABLE [dbo].[Comments]
(
    [Id] VARCHAR(32) NOT NULL,
    [UserId] VARCHAR(32) NOT NULL,
    [ItemId] VARCHAR(32) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [CreatedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),
    [LastModifiedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),

    CONSTRAINT [PK_Comments] PRIMARY KEY ([Id])
);
");
            #endregion
        }

        public override void Down()
        {
            Delete.Table("Comments").InSchema("dbo");
            Delete.Table("DraftTags").InSchema("dbo");
            Delete.Table("Drafts").InSchema("dbo");
            Delete.Table("ItemTags").InSchema("dbo");
            Delete.Table("Items").InSchema("dbo");
            Delete.Table("Users").InSchema("dbo");
        }
    }
}
