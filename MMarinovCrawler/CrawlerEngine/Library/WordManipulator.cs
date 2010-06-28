using System;
using System.Linq;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class WordManipulator
    {
        public struct FileCountPair
        {
            public DALWebCrawler.File File;

            public int Count;

            public FileCountPair(DALWebCrawler.File file, int count)
            {
                File = file;
                Count = count;
            }
        }

        public static bool InsertWordsIntoDB(System.Collections.Generic.ICollection<DALWebCrawler.Word> wordsColl)
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.Words.InsertAllOnSubmit(wordsColl);
                dataContext.SubmitChanges();

                return true;
            }
        }

        public static bool InsertWordsInFilesIntoDB(System.Collections.Generic.ICollection<DALWebCrawler.WordsInFile> wordInFileColl)
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.WordsInFiles.InsertAllOnSubmit(wordInFileColl);
                dataContext.SubmitChanges();

                return true;
            }
        }

        public static IQueryable<DALWebCrawler.Word> SelectAllWords()
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                return from w in dataContext.Words
                       select w;
            }
        }

        public static IQueryable<DALWebCrawler.WordsInFile> SelectAllWordsInFiles()
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                return from wif in dataContext.WordsInFiles
                       select wif;
            }
        }
    }
}
