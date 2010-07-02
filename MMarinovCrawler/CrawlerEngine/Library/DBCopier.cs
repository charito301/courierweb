using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static void BulkCopyToActiveDB()
        {
            TruncateActiveDBTables();

            System.Data.DataTable dtFiles = CreateDatatable(new string[] { "ID", "URL", "Title", "ImportantWords", "WeightedWords", "FileType" });
            System.Data.DataTable dtWords = CreateDatatable(new string[] { "ID", "WordName" });
            System.Data.DataTable dtWordsInFiles = CreateDatatable(new string[] { "FileID", "WordID", "Count" });

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
                    dtWordsInFiles.Rows.Add(new object[] { wif.FileID, wif.WordID, wif.Count });
                }
            }

            // Initializing an SqlBulkCopy object
            using (System.Data.SqlClient.SqlBulkCopy sbc = new System.Data.SqlClient.SqlBulkCopy(Preferences.ConnectionStringActive))
            {
                // Copying data to destination
                sbc.DestinationTableName = "Files";
                sbc.WriteToServer(dtFiles);
                sbc.DestinationTableName = "Words";
                sbc.WriteToServer(dtWords);
                sbc.DestinationTableName = "WordsInFiles";
                sbc.WriteToServer(dtWordsInFiles);
                sbc.Close();
            }
        }

        private static System.Data.DataTable CreateDatatable(string[] columnNames)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            foreach (string colName in columnNames)
            {
                dt.Columns.Add(colName);
            }

            return dt;
        }
    }
}
