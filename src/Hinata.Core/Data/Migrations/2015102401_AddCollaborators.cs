using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2015102401, TransactionBehavior.None)]
    public class AddCollaborators : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTags] DROP CONSTRAINT [FK_ItemTags_Items]
ALTER TABLE [dbo].[ItemRevisions] DROP CONSTRAINT [FK_ItemRevisions_Items]
");

            Execute.Sql(@"
SELECT *
INTO #tmp_Items
FROM [dbo].[Items];

DROP TABLE [dbo].[Items]

CREATE TABLE [dbo].[Items]
(
    [Id] VARCHAR(32) NOT NULL,
    [CreateUserId] VARCHAR(32) NOT NULL,
    [LastModifyUserId] VARCHAR(32) NOT NULL,
    [Type] TINYINT NOT NULL,
    [IsPublic] BIT NOT NULL DEFAULT(0),
    [Title] NVARCHAR(256) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [CreatedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),
    [LastModifiedDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSDATETIME()),

    CONSTRAINT [PK_Items] PRIMARY KEY ([Id])
);

INSERT INTO [dbo].[Items]
SELECT
    [Id],
    [CreateUserId] = [UserId],
    [LastModifyUserId] = [UserId],
    [Type],
    [IsPublic],
    [Title],
    [Body],
    [CreatedDateTime],
    [LastModifiedDateTime]
FROM #tmp_Items
");

            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTags] WITH CHECK ADD CONSTRAINT [FK_ItemTags_Items] FOREIGN KEY([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[ItemTags] CHECK CONSTRAINT [FK_ItemTags_Items];
ALTER TABLE [dbo].[ItemRevisions] WITH CHECK ADD CONSTRAINT [FK_ItemRevisions_Items] FOREIGN KEY([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[ItemRevisions] CHECK CONSTRAINT [FK_ItemRevisions_Items]
");

            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTagRevisions] DROP CONSTRAINT [FK_ItemTagRevisions_ItemRevisions]
");

            Execute.Sql(@"
SELECT *
INTO #tmp_ItemRevisions
FROM [dbo].[ItemRevisions];

DROP TABLE [dbo].[ItemRevisions]

CREATE TABLE [dbo].[ItemRevisions] (
    [ItemId] VARCHAR(32) NOT NULL,
    [RevisionNo] INT NOT NULL DEFAULT(0),
    [CreateUserId] VARCHAR(32) NOT NULL,
    [ModifyUserId] VARCHAR(32) NOT NULL,
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

INSERT INTO [dbo].[ItemRevisions]
SELECT
    [ItemId],
    [RevisionNo],
    [CreateUserId] = [UserId],
    [ModifyUserId] = [UserId],
    [Type],
    [IsPublic],
    [Title],
    [Body],
    [Comment],
    [CreatedDateTime],
    [ModifiedDateTime]
FROM #tmp_ItemRevisions
");

            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTagRevisions] WITH CHECK ADD CONSTRAINT [FK_ItemTagRevisions_ItemRevisions] FOREIGN KEY([ItemId], [RevisionNo]) REFERENCES [dbo].[ItemRevisions] ([ItemId], [RevisionNo]) ON DELETE CASCADE;
ALTER TABLE [dbo].[ItemTagRevisions] CHECK CONSTRAINT [FK_ItemTagRevisions_ItemRevisions];
");

            Execute.Sql(@"
CREATE TABLE [dbo].[Collaborators] (
    [ItemId] VARCHAR(32) NOT NULL,
    [UserId] VARCHAR(32) NOT NULL,
    [RoleType] tinyint NOT NULL,

    CONSTRAINT [PK_Collaborators] PRIMARY KEY ([ItemId], [UserId]),
    CONSTRAINT [FK_Collaborators_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT [FK_Collaborators_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ON DELETE CASCADE ON UPDATE NO ACTION
);
");

        }

        public override void Down()
        {
            Delete.Table("Collaborators").InSchema("dbo");

            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTags] DROP CONSTRAINT [FK_ItemTags_Items]
ALTER TABLE [dbo].[ItemRevisions] DROP CONSTRAINT [FK_ItemRevisions_Items]
");

            Execute.Sql(@"
SELECT *
INTO #tmp_Items
FROM [dbo].[Items];

DROP TABLE [dbo].[Items]

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

INSERT INTO [dbo].[Items]
SELECT
    [Id],
    [UserId] = [CreateUserId],
    [Type],
    [IsPublic],
    [Title],
    [Body],
    [CreatedDateTime],
    [LastModifiedDateTime]
FROM #tmp_Items
");

            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTags] WITH CHECK ADD CONSTRAINT [FK_ItemTags_Items] FOREIGN KEY([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[ItemTags] CHECK CONSTRAINT [FK_ItemTags_Items];
ALTER TABLE [dbo].[ItemRevisions] WITH CHECK ADD  CONSTRAINT [FK_ItemRevisions_Items] FOREIGN KEY([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[ItemRevisions] CHECK CONSTRAINT [FK_ItemRevisions_Items]
");

            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTagRevisions] DROP CONSTRAINT [FK_ItemTagRevisions_ItemRevisions]
");

            Execute.Sql(@"
SELECT *
INTO #tmp_ItemRevisions
FROM [dbo].[ItemRevisions];

DROP TABLE [dbo].[ItemRevisions]

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

INSERT INTO [dbo].[ItemRevisions]
SELECT
    [ItemId],
    [RevisionNo],
    [UserId] = [CreateUserId],
    [Type],
    [IsPublic],
    [Title],
    [Body],
    [Comment],
    [CreatedDateTime],
    [ModifiedDateTime]
FROM #tmp_ItemRevisions
");

            Execute.Sql(@"
ALTER TABLE [dbo].[ItemTagRevisions] WITH CHECK ADD CONSTRAINT [FK_ItemTagRevisions_ItemRevisions] FOREIGN KEY([ItemId], [RevisionNo]) REFERENCES [dbo].[ItemRevisions] ([ItemId], [RevisionNo]) ON DELETE CASCADE;
ALTER TABLE [dbo].[ItemTagRevisions] CHECK CONSTRAINT [FK_ItemTagRevisions_ItemRevisions];
");
        }
    }
}