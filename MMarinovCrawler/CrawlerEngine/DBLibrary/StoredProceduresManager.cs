using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MMarinov.WebCrawler.DBLibrary
{
    public static class StoredProceduresManager
    {
        public static void InitAllStoredProcedures()
        {
            SqlConnection cn = new SqlConnection(Preferences.ConnectionString);
            SqlCommand cm = new SqlCommand();

            cn.Open();

            try
            {
                cm.Connection = cn;
                cm.CommandType = CommandType.Text;

                cm.CommandText = dropIfExists("sp_CopyFromDBToActiveDB") + dropIfExists("sp_InsertFile") + dropIfExists("sp_InsertStatistics") + dropIfExists("sp_InsertWord") + dropIfExists("sp_InsertWordInFile") + dropIfExists("sp_SelectWordsAll") +dropIfExists("sp_TruncateTables");
                cm.ExecuteNonQuery();

                cm.CommandText = sp_CopyFromDBToActiveDB;
                cm.ExecuteNonQuery();
                cm.CommandText = sp_InsertFile;
                cm.ExecuteNonQuery();
                cm.CommandText = sp_InsertStatistics;
                cm.ExecuteNonQuery();
                cm.CommandText = sp_InsertWord;
                cm.ExecuteNonQuery();
                cm.CommandText = sp_InsertWordInFile;
                cm.ExecuteNonQuery();
                cm.CommandText = sp_SelectWordsAll;
                cm.ExecuteNonQuery();
                cm.CommandText = sp_TruncateTables;
                cm.ExecuteNonQuery();
            }
            finally
            {
                cn.Close();
            }

            cn = new SqlConnection(Preferences.ConnectionStringActive);
            cm = new SqlCommand();

            // WebCrawlerActive
            cn.Open();

            try
            {
                cm.Connection = cn;
                cm.CommandType = CommandType.Text;

                cm.CommandText = dropIfExists("sp_TruncateAllTables");
                cm.ExecuteNonQuery();

                cm.CommandText = sp_TruncateAllTables;
                cm.ExecuteNonQuery();
            }
            finally
            {
                cn.Close();
            }
        }

        private static string dropIfExists(string spName)
        {
            return @"
IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('" + spName + @"') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
   DROP PROCEDURE " + spName;
        }

        private static string sp_TruncateAllTables = @"
CREATE PROCEDURE [dbo].[sp_TruncateAllTables]
AS

/*Disable Constraints & Triggers*/
ALTER TABLE Files NOCHECK CONSTRAINT ALL
ALTER TABLE Files DISABLE TRIGGER ALL

ALTER TABLE Words NOCHECK CONSTRAINT ALL
ALTER TABLE Words DISABLE TRIGGER ALL

ALTER TABLE WordsInFiles NOCHECK CONSTRAINT ALL
ALTER TABLE WordsInFiles DISABLE TRIGGER ALL

/*Perform delete operation on all table for cleanup*/
DELETE Files
DELETE Words
DELETE WordsInFiles

/*Enable Constraints & Triggers again*/
ALTER TABLE Files CHECK CONSTRAINT ALL
ALTER TABLE Files ENABLE TRIGGER ALL

ALTER TABLE Words CHECK CONSTRAINT ALL
ALTER TABLE Words ENABLE TRIGGER ALL

ALTER TABLE WordsInFiles CHECK CONSTRAINT ALL
ALTER TABLE WordsInFiles ENABLE TRIGGER ALL

/*Reset Identity on tables with identity column*/
IF OBJECTPROPERTY(OBJECT_ID('Files'), 'TableHasIdentity') = 1 BEGIN DBCC CHECKIDENT ('Files',RESEED,0) END
IF OBJECTPROPERTY(OBJECT_ID('Words'), 'TableHasIdentity') = 1 BEGIN DBCC CHECKIDENT ('Words',RESEED,0) END
IF OBJECTPROPERTY(OBJECT_ID('WordsInFiles'), 'TableHasIdentity') = 1 BEGIN DBCC CHECKIDENT ('WordsInFiles',RESEED,0) END";

        private static string sp_TruncateTables = @"
CREATE PROCEDURE [dbo].[sp_TruncateTables]
AS

/*Disable Constraints & Triggers*/
ALTER TABLE Files NOCHECK CONSTRAINT ALL
ALTER TABLE Files DISABLE TRIGGER ALL

ALTER TABLE WordsInFiles NOCHECK CONSTRAINT ALL
ALTER TABLE WordsInFiles DISABLE TRIGGER ALL

/*Perform delete operation on all table for cleanup*/
DELETE Files
DELETE WordsInFiles

/*Enable Constraints & Triggers again*/
ALTER TABLE Files CHECK CONSTRAINT ALL
ALTER TABLE Files ENABLE TRIGGER ALL

ALTER TABLE WordsInFiles CHECK CONSTRAINT ALL
ALTER TABLE WordsInFiles ENABLE TRIGGER ALL

/*Reset Identity on tables with identity column*/
IF OBJECTPROPERTY(OBJECT_ID('Files'), 'TableHasIdentity') = 1 BEGIN DBCC CHECKIDENT ('Files',RESEED,0) END
IF OBJECTPROPERTY(OBJECT_ID('WordsInFiles'), 'TableHasIdentity') = 1 BEGIN DBCC CHECKIDENT ('WordsInFiles',RESEED,0) END";

        private static string sp_CopyFromDBToActiveDB = @"
CREATE Procedure [dbo].[sp_CopyFromDBToActiveDB]
as

SET IDENTITY_INSERT WebCrawlerActive.dbo.Words ON
insert into WebCrawlerActive.dbo.Words(ID, WordName) 
select ID, WordName from WebCrawler.dbo.Words
SET IDENTITY_INSERT WebCrawlerActive.dbo.Words OFF

SET IDENTITY_INSERT WebCrawlerActive.dbo.Files ON
insert into WebCrawlerActive.dbo.Files(ID, URL, Title, Keywords, Description, FileType) 
select ID, URL, Title, Keywords, Description, FileType from WebCrawler.dbo.Files
SET IDENTITY_INSERT WebCrawlerActive.dbo.Files OFF

SET IDENTITY_INSERT WebCrawlerActive.dbo.WordsInFiles ON
insert into WebCrawlerActive.dbo.WordsInFiles(ID,WordID,FileID,Count) 
select ID, WordID, FileID, Count from WebCrawler.dbo.WordsInFiles
SET IDENTITY_INSERT WebCrawlerActive.dbo.WordsInFiles OFF";

        private static string sp_InsertFile = @"
CREATE PROCEDURE [dbo].[sp_InsertFile]
	@URL nvarchar(2500),
	@Title nvarchar(200),
	@Keywords nvarchar(500),
	@Description nvarchar(500),
	@FileType tinyint
AS

SET NOCOUNT ON

INSERT INTO [dbo].[Files] (
	[URL],
	[Title],
	[Keywords],
	[Description],
	[FileType]
) VALUES (
	@URL,
	@Title,
	@Keywords,
	@Description,
	@FileType
)

SELECT SCOPE_IDENTITY()";

        private static string sp_InsertWord = @"
CREATE PROCEDURE [dbo].[sp_InsertWord]
	@WordName nvarchar(50)
AS

SET NOCOUNT ON

INSERT INTO [dbo].[Words] (
	[WordName]
) VALUES (
	@WordName
)

SELECT SCOPE_IDENTITY()";

        private static string sp_InsertWordInFile = @"
CREATE PROCEDURE [dbo].[sp_InsertWordInFile]
	@WordID bigint,
	@FileID bigint,
	@Count int
AS

SET NOCOUNT ON

INSERT INTO [dbo].[WordsInFiles] (
	[WordID],
	[FileID],
	[Count]
) VALUES (
	@WordID,
	@FileID,
	@Count
)

SELECT SCOPE_IDENTITY()";

        private static string sp_InsertStatistics = @"
CREATE PROCEDURE [dbo].[sp_InsertStatistics]
	@CrawledSuccessfulLinks bigint,
	@CrawledTotalLinks bigint,
	@Duration varchar(50),
	@FoundTotalLinks bigint,
	@FoundValidLinks bigint,
	@ProcessDescription varchar(250),
	@StartDate datetime,
	@Words bigint                   
AS

SET NOCOUNT ON

INSERT INTO [dbo].[Statistics] (
	[CrawledSuccessfulLinks],
	[CrawledTotalLinks],
	[Duration],
	[FoundTotalLinks],
	[FoundValidLinks],
	[ProcessDescription],
	[StartDate],
	[Words]
) VALUES (
	@CrawledSuccessfulLinks,
	@CrawledTotalLinks,
	@Duration,
	@FoundTotalLinks,
	@FoundValidLinks,
	@ProcessDescription,
	@StartDate,
	@Words
)

SELECT SCOPE_IDENTITY()";

        private static string sp_SelectWordsAll = @"
CREATE PROCEDURE [dbo].[sp_SelectWordsAll]
AS

SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ COMMITTED

SELECT
	[ID],
	[WordName]
FROM
	[dbo].[Words]";
    }
}
