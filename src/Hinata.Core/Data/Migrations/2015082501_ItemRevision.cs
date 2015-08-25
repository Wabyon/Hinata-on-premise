using System.Linq;
using FluentMigrator;
using Hinata.Data.Commands;

namespace Hinata.Data.Migrations
{
    [Migration(2015082501)]
    public class AddItemChangeLogs : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
CREATE TABLE [dbo].[ItemRevisions] (
    [ItemId] VARCHAR(32) NOT NULL,
    [RevisionNo] INT NOT NULL DEFAULT(0),
    [UserId] VARCHAR(32) NOT NULL,
    [Type] TINYINT NOT NULL,
    [IsPublic] BIT NOT NULL DEFAULT(0),
    [Title] NVARCHAR(256) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [Comment] NVARCHAR(256) NULL,
    [CreatedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),
    [ModifiedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),

    CONSTRAINT [PK_ItemRevisions] PRIMARY KEY ([ItemId], [RevisionNo]),
    CONSTRAINT [FK_ItemRevisions_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE
);
");

            Execute.Sql(@"
CREATE TABLE [dbo].[ItemTagRevisions] (
    [ItemId] VARCHAR(32) NOT NULL,
    [RevisionNo] INT NOT NULL DEFAULT(0),
    [Name] NVARCHAR(32) NOT NULL,
    [Version] NVARCHAR(16) NULL,
    [OrderNo] INT NOT NULL DEFAULT(0),

    CONSTRAINT [PK_ItemTagRevisions] PRIMARY KEY ([ItemId], [RevisionNo], [Name]),
    CONSTRAINT [FK_ItemTagRevisions_ItemRevisions] FOREIGN KEY ([ItemId], [RevisionNo]) REFERENCES [dbo].[ItemRevisions] ([ItemId], [RevisionNo]) ON DELETE CASCADE
);
");

            Execute.Sql(@"
ALTER TABLE [dbo].[Drafts] ADD [Comment] NVARCHAR(256) NULL;
");

            Execute.Sql(@"
INSERT INTO [dbo].[ItemRevisions] (
     [ItemId]
    ,[RevisionNo]
    ,[UserId]
    ,[Type]
    ,[IsPublic]
    ,[Title]
    ,[Body]
    ,[Comment]
    ,[CreatedDateTime]
    ,[ModifiedDateTime]
)
SELECT
     [Id]
    ,[RevisionNo] = 0
    ,[UserId]
    ,[Type]
    ,[IsPublic]
    ,[Title]
    ,[Body]
    ,[Comment] = 'Migrate'
    ,[CreatedDateTime]
    ,[LastModifiedDateTime]
FROM [dbo].[Items];

INSERT INTO [dbo].[ItemTagRevisions] (
     [ItemId]
    ,[RevisionNo]
    ,[Name]
    ,[Version]
    ,[OrderNo]
)
SELECT
     [ItemId]
    ,[RevisionNo] = 0
    ,[Name]
    ,[Version]
    ,[OrderNo]
FROM [dbo].[ItemTags];
");
        }

        public override void Down()
        {
            Execute.Sql(@"
ALTER TABLE [dbo].[Drafts] DROP COLUMN [Comment];
");

            Execute.Sql(@"
DROP TABLE [dbo].[ItemTagRevisions];
");

            Execute.Sql(@"
DROP TABLE [dbo].[ItemRevisions];
");
        }
    }
}
