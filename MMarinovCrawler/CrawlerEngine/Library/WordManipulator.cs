using System;
using System.Linq;

namespace MMarinov.WebCrawler.Library
{
    public static class WordManipulator
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

        public struct WordFilesPair
        {
            public DALWebCrawler.Word Word;

            public ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> WordFileCountHT;

            public WordFilesPair(DALWebCrawler.Word word, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> wordFileCountDic)
            {
                Word = word;
                WordFileCountHT = wordFileCountDic;
            }
        }

        public static bool InsertWordsIntoDB(System.Collections.Generic.ICollection<DALWebCrawler.Word> wordsColl)
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                foreach (DALWebCrawler.Word word in wordsColl)
                {
                    long? id = 0;
                    dataContext.sp_InsertWord(word.WordName, ref id);
                    word.ID = id ?? 0;
                }

                //dataContext.Words.InsertAllOnSubmit(wordsColl);
                //dataContext.SubmitChanges();

                return true;
            }
        }

        public static bool InsertWordsInFilesIntoDB(System.Collections.Generic.ICollection<DALWebCrawler.WordsInFile> wordInFileColl)
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                //foreach (DALWebCrawler.WordsInFile wif in wordInFileColl)
                //{
                //    long? id = 0;
                //    dataContext.sp_InsertWordInFile(wif.WordID, wif.FileID, wif.Count);
                //    wif.ID = id ?? 0;
                //}
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
