using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2015110401, TransactionBehavior.None)]
    public class DraftsPrimaryKeyAddUserId : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
CREATE TABLE [dbo].[tmp_ms_xx_DraftTags] (
    [DraftId] VARCHAR (32)  NOT NULL,
    [UserId]  VARCHAR (32)  NOT NULL,
    [Name]    NVARCHAR (32) NOT NULL,
    [Version] NVARCHAR (16) NULL,
    [OrderNo] INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_DraftTags] PRIMARY KEY CLUSTERED ([DraftId], [UserId], [Name])
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[DraftTags])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_DraftTags] ([DraftId], [UserId], [Name], [Version], [OrderNo])
        SELECT   DraftTags.[DraftId],
                 Drafts.[UserId],
                 DraftTags.[Name],
                 DraftTags.[Version],
                 DraftTags.[OrderNo]
        FROM     [dbo].[DraftTags] DraftTags
        INNER JOIN [dbo].[Drafts] Drafts
        ON  DraftTags.[DraftId] = Drafts.[Id]
        ORDER BY [DraftId] ASC, [Name] ASC;
    END

DROP TABLE [dbo].[DraftTags];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_DraftTags]', N'DraftTags';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_DraftTags]', N'PK_DraftTags', N'OBJECT';

CREATE TABLE [dbo].[tmp_ms_xx_Drafts] (
    [Id]                   VARCHAR (32)   NOT NULL,
    [UserId]               VARCHAR (32)   NOT NULL,
    [Type]                 TINYINT        NOT NULL,
    [Title]                NVARCHAR (256) NULL,
    [Body]                 NVARCHAR (MAX) NULL,
    [LastModifiedDateTime] DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [Comment]              NVARCHAR (256) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Drafts] PRIMARY KEY CLUSTERED ([Id], [UserId])
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Drafts])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Drafts] ([Id], [UserId], [Type], [Title], [Body], [LastModifiedDateTime], [Comment])
        SELECT   [Id],
                 [UserId],
                 [Type],
                 [Title],
                 [Body],
                 [LastModifiedDateTime],
                 [Comment]
        FROM     [dbo].[Drafts]
        ORDER BY [Id] ASC, [UserId] ASC;
    END

DROP TABLE [dbo].[Drafts];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Drafts]', N'Drafts';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Drafts]', N'PK_Drafts', N'OBJECT';

ALTER TABLE [dbo].[DraftTags] WITH NOCHECK
    ADD CONSTRAINT [FK_DraftTags_Drafts] FOREIGN KEY ([DraftId], [UserId]) REFERENCES [dbo].[Drafts] ([Id], [UserId]) ON DELETE CASCADE;
");
        }

        public override void Down()
        {
            Execute.Sql(@"
DELETE Drafts FROM [dbo].[Drafts] Drafts
WHERE EXISTS (SELECT * FROM [dbo].[Items] Items WHERE Drafts.Id = Items.Id)
AND NOT EXISTS (SELECT * FROM [dbo].[Items] Items WHERE Drafts.Id = Items.Id AND Drafts.UserId = Items.CreateUserId)

CREATE TABLE [dbo].[tmp_ms_xx_DraftTags] (
    [DraftId] VARCHAR (32)  NOT NULL,
    [Name]    NVARCHAR (32) NOT NULL,
    [Version] NVARCHAR (16) NULL,
    [OrderNo] INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_DraftTags] PRIMARY KEY CLUSTERED ([DraftId], [Name])
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[DraftTags])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_DraftTags] ([DraftId], [Name], [Version], [OrderNo])
        SELECT   [DraftId],
                 [Name],
                 [Version],
                 [OrderNo]
        FROM     [dbo].[DraftTags]
        ORDER BY [DraftId] ASC, [Name] ASC;
    END

DROP TABLE [dbo].[DraftTags];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_DraftTags]', N'DraftTags';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_DraftTags]', N'PK_DraftTags', N'OBJECT';

CREATE TABLE [dbo].[tmp_ms_xx_Drafts] (
    [Id]                   VARCHAR (32)   NOT NULL,
    [UserId]               VARCHAR (32)   NOT NULL,
    [Type]                 TINYINT        NOT NULL,
    [Title]                NVARCHAR (256) NULL,
    [Body]                 NVARCHAR (MAX) NULL,
    [LastModifiedDateTime] DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [Comment]              NVARCHAR (256) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_Drafts] PRIMARY KEY CLUSTERED ([Id])
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Drafts])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_Drafts] ([Id], [UserId], [Type], [Title], [Body], [LastModifiedDateTime], [Comment])
        SELECT   [Id],
                 [UserId],
                 [Type],
                 [Title],
                 [Body],
                 [LastModifiedDateTime],
                 [Comment]
        FROM     [dbo].[Drafts]
        ORDER BY [Id] ASC;
    END

DROP TABLE [dbo].[Drafts];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Drafts]', N'Drafts';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_Drafts]', N'PK_Drafts', N'OBJECT';

ALTER TABLE [dbo].[DraftTags] WITH NOCHECK
    ADD CONSTRAINT [FK_DraftTags_Drafts] FOREIGN KEY ([DraftId]) REFERENCES [dbo].[Drafts] ([Id]) ON DELETE CASCADE;
");
        }
    }
}
