using System;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class WordManipulator
    {
        public struct FileCountPair
        {
            public string URL;

            public int Count;

            public FileCountPair(string url, int count)
            {
                URL = url;
                Count = count;
            }
        }

        ///// <summary>Add a file referencing this word</summary>
        //public void AddWordInFile(DALWebCrawler.File inFile, ThreadedGenerics.TList<DALWebCrawler.WordsInFile> wordsInFileColl)
        //{
        //    DALWebCrawler.WordsInFile wif = wordsInFileColl.GetWordInFile(_id, inFile.ID);

        //    if (wif != null)
        //    {
        //        wif.Count++; 
        //    }
        //    else
        //    {
        //        wordsInFileColl.Add(DALWebCrawler.WordsInFile.NewWordInFile(_id, inFile.ID, 1));
        //    }
        //}

        public static void InsertWordsIntoDB(System.Collections.Generic.ICollection<DALWebCrawler.Word> wordsColl)
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.Words.InsertAllOnSubmit(wordsColl);
                dataContext.SubmitChanges();
            }
        }

        ///TODO
        public static void InsertWordsInFilesIntoDB(System.Collections.Generic.ICollection<DALWebCrawler.WordsInFile> wordInFileColl)
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                //dataContext.WordsInFiles.InsertAllOnSubmit(wordInFileColl);
                //dataContext.SubmitChanges();
                ///
                /// TODO
                /// get all records from table with words and file ..
            }
        }
    }
}
