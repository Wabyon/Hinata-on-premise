using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(1)]
    public class CreateLogTables : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
CREATE TABLE [dbo].[TraceLogs]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [LogUtcDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSUTCDATETIME()),
    [Logger] [NVARCHAR](500) NOT NULL,
    [Level] [NVARCHAR](10) NOT NULL,
    [ThreadId] [INT] NOT NULL,
    [MachineName] [NVARCHAR](100) NOT NULL,
    [Message] [NVARCHAR](MAX) NOT NULL,

    CONSTRAINT [PK_TraceLogs] PRIMARY KEY ([Id])
)
");

            Execute.Sql(@"
CREATE TABLE [dbo].[AccessLogs]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [AccessUtcDateTime] DATETIME2(7) NOT NULL DEFAULT(SYSUTCDATETIME()),
    [ServerName] [NVARCHAR](256) NOT NULL,
    [UserName] [NVARCHAR](256) NULL,
    [Url] [NVARCHAR](MAX) NOT NULL,
    [HttpMethod] [NVARCHAR](16) NOT NULL,
    [Path] [NVARCHAR](MAX) NOT NULL,
    [Query] [NVARCHAR](MAX) NULL,
    [Form] [NVARCHAR](MAX) NULL,
    [Controller] [NVARCHAR](256) NULL,
    [Action] [NVARCHAR](256) NULL,
    [UserHostAddress] [NVARCHAR](256) NULL,
    [UserAgent] [NVARCHAR](MAX) NULL,

    CONSTRAINT [PK_AccessLogs] PRIMARY KEY ([Id])
)
");
        }

        public override void Down()
        {
            Delete.Table("TraceLogs").InSchema("dbo");
            Delete.Table("AccessLogs").InSchema("dbo");
        }
    }
}
