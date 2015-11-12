using FluentMigrator;

namespace Hinata.Data.Migrations
{
    [Migration(2015111201)]
    public class ItemFreeEditable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
ALTER TABLE [dbo].[Items] ADD [IsFreeEditable] BIT DEFAULT (0) NOT NULL
ALTER TABLE [dbo].[ItemRevisions] ADD [IsFreeEditable] BIT DEFAULT (0) NOT NULL
");
        }

        public override void Down()
        {
            Execute.Sql(@"
DECLARE @DefaultConstraintName sysname

DECLARE Cur CURSOR LOCAL FAST_FORWARD
FOR
SELECT sys.default_constraints.name
FROM sys.default_constraints
INNER JOIN sys.columns
ON  sys.default_constraints.parent_column_id= sys.columns.column_id
WHERE 
    sys.default_constraints.parent_object_id = OBJECT_ID(N'[dbo].[ItemRevisions]')
AND sys.columns.name = N'IsFreeEditable'

OPEN Cur
FETCH NEXT FROM Cur
INTO @DefaultConstraintName

WHILE @@FETCH_STATUS = 0
BEGIN
EXEC ('ALTER TABLE [dbo].[ItemRevisions] DROP CONSTRAINT [' + @DefaultConstraintName + ']')

FETCH NEXT FROM Cur
INTO @DefaultConstraintName
END

CLOSE Cur
DEALLOCATE Cur

ALTER TABLE [dbo].[ItemRevisions] DROP COLUMN [IsFreeEditable]
");

            Execute.Sql(@"
DECLARE @DefaultConstraintName sysname

DECLARE Cur CURSOR LOCAL FAST_FORWARD
FOR
SELECT sys.default_constraints.name
FROM sys.default_constraints
INNER JOIN sys.columns
ON  sys.default_constraints.parent_column_id= sys.columns.column_id
WHERE 
    sys.default_constraints.parent_object_id = OBJECT_ID(N'[dbo].[Items]')
AND sys.columns.name = N'IsFreeEditable'

OPEN Cur
FETCH NEXT FROM Cur
INTO @DefaultConstraintName

WHILE @@FETCH_STATUS = 0
BEGIN
EXEC ('ALTER TABLE [dbo].[Items] DROP CONSTRAINT [' + @DefaultConstraintName + ']')

FETCH NEXT FROM Cur
INTO @DefaultConstraintName
END

CLOSE Cur
DEALLOCATE Cur

ALTER TABLE [dbo].[Items] DROP COLUMN [IsFreeEditable]
");
        }
    }
}
