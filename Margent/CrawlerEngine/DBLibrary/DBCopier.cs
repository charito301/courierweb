using System;
using System.Data;
using System.Data.SqlClient;

namespace MMarinov.WebCrawler.Library
{
    public static class DBCopier
    {
        public static void TruncateDBTables()
        {
            SqlConnection cn = new SqlConnection(Preferences.ConnectionString);
            SqlCommand cm = new SqlCommand();

            cn.Open();

            try
            {
                cm.Connection = cn;
               cm.CommandTimeout = 600;
                cm.CommandType = CommandType.StoredProcedure;

                cm.CommandText = "sp_TruncateTables";
                cm.ExecuteNonQuery();
            }
            finally
            {
                cn.Close();
            }
        }

        private static void TruncateActiveDBTables()
        {
            using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataContext = new DALWebCrawlerActive.WebCrawlerActiveDataContext(Preferences.ConnectionStringActive))
            {
                dataContext.CommandTimeout = 600;
                dataContext.sp_TruncateAllTables();
            }
        }

        public static void CopyDBToActiveDB()
        {
            TruncateActiveDBTables();

            CopyFromDBToActiveDB();
        }

        private static void CopyFromDBToActiveDB()
        {
            SqlConnection cn = new SqlConnection(Preferences.ConnectionString);
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr;

            cn.Open();

            try
            {
                tr = cn.BeginTransaction(IsolationLevel.Serializable);

                try
                {

                    cm.Transaction = tr;
                    cm.Connection = cn;
                    cm.CommandTimeout = 600;
                    cm.CommandType = CommandType.StoredProcedure;

                    cm.CommandText = "sp_CopyFromDBToActiveDB";
                    cm.ExecuteNonQuery();

                    tr.Commit();
                }
                catch (Exception ex)
                {
                    tr.Rollback();//important here
                    throw (ex);
                }
            }
            finally
            {
                cn.Close();
            }
        }

        #region Better way, but not working for now
        /*
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
        }*/
        #endregion
    }
}
