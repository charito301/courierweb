using System.Linq;

namespace MMarinov.WebCrawler.Library
{
    public static class DBCopier
    {
        public static void TruncateDBTables()
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.sp_TruncateAllTables();
            }
        }

        private static void TruncateActiveDBTables()
        {
            using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataContext = new DALWebCrawlerActive.WebCrawlerActiveDataContext(Preferences.ConnectionStringActive))
            {
                dataContext.sp_TruncateAllTables();
            }
        }

        public static void CopyDBToActiveDB()
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.sp_TruncateAllTables();
                dataContext.sp_CopyFromDBToActiveDB();
            }
        }

        #region Better way, but not working for now
        public static void BulkCopyToActiveDB()
        {
            TruncateActiveDBTables();

            System.Data.DataTable dtFiles = CreateDatatable("Files", new string[] { "ID", "URL", "Title", "ImportantWords", "WeightedWords", "FileType" });
            System.Data.DataTable dtWords = CreateDatatable("Words", new string[] { "ID", "WordName" });
            System.Data.DataTable dtWordsInFiles = CreateDatatable("WordsInFiles", new string[] { "ID", "FileID", "WordID", "Count" });

            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                IQueryable<DALWebCrawler.File> allFiles = from f in dataContext.Files select f;
                IQueryable<DALWebCrawler.Word> allWords = from w in dataContext.Words select w;
                IQueryable<DALWebCrawler.WordsInFile> allWordsInFiles = from wif in dataContext.WordsInFiles select wif;

                foreach (DALWebCrawler.File file in allFiles)
                {
                    dtFiles.Rows.Add(new object[] { file.ID, file.URL, file.Title, file.ImportantWords, file.WeightedWords, file.FileType });
                }

                foreach (DALWebCrawler.Word word in allWords)
                {
                    dtWords.Rows.Add(new object[] { word.ID, word.WordName });
                }

                foreach (DALWebCrawler.WordsInFile wif in allWordsInFiles)
                {
                    dtWordsInFiles.Rows.Add(new object[] { wif.ID, wif.FileID, wif.WordID, wif.Count });
                }
            }

            CopyTable(dtFiles);
            CopyTable(dtWords);
            CopyTable(dtWordsInFiles);
        }

        private static void CopyTable(System.Data.DataTable dtDBtableData)
        {
            // Initializing an SqlBulkCopy object
            using (System.Data.SqlClient.SqlBulkCopy sqlBulkCopy = new System.Data.SqlClient.SqlBulkCopy(Preferences.ConnectionStringActive, System.Data.SqlClient.SqlBulkCopyOptions.TableLock | System.Data.SqlClient.SqlBulkCopyOptions.FireTriggers | System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity))
            {
                // Copying data to destination
                sqlBulkCopy.DestinationTableName = dtDBtableData.TableName;
                sqlBulkCopy.WriteToServer(dtDBtableData);

                sqlBulkCopy.Close();
            }
        }

        private static System.Data.DataTable CreateDatatable(string tableName, string[] columnNames)
        {
            System.Data.DataTable dt = new System.Data.DataTable(tableName);

            foreach (string colName in columnNames)
            {
                dt.Columns.Add(colName);
            }

            return dt;
        } 
        #endregion
    }
}
