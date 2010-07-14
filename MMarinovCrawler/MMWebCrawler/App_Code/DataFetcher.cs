using System.Linq;
using System.Collections.Generic;
using System.Collections;
using DALWebCrawlerActive;
using System;

namespace Margent
{
    /// <summary>
    /// Summary description for DataFetcher
    /// </summary>
    public static class DataFetcher
    {
        private static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionStringActive"].ToString();

        public class CountFileList
        {
            public int Count = 0;
            public List<File> FilesList = new List<File>();

            public CountFileList(int count, File fileToAdd)
            {
                Count = count;
                FilesList.Add(fileToAdd);
            }
        }

        public static Dictionary<Word, CountFileList> FetchResults(string query)
        {
            IQueryable<DALWebCrawlerActive.WordsInFile> wordsInFiles;
            Dictionary<Word, CountFileList> results = new Dictionary<Word, CountFileList>();

            using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataContext = new DALWebCrawlerActive.WebCrawlerActiveDataContext(connectionString))
            {
                long ticks1 = DateTime.Now.Ticks;
                string[] queryWords = query.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                wordsInFiles = from wif in
                                   dataContext.WordsInFiles.Where(wif2 => (from wif in dataContext.WordsInFiles.Where(wif3 => queryWords.Contains(wif3.Word.WordName))
                                                                           select wif.FileID).Contains(wif2.FileID))
                               select wif;

                foreach (WordsInFile wif in wordsInFiles)
                {
                    if (wif.Word.WordName == query)
                    {
                        continue; // need improvment
                    }

                    if (results.Keys.Contains(wif.Word))
                    {
                        CountFileList cfl = results[wif.Word];
                        cfl.Count += wif.Count;
                        cfl.FilesList.Add(wif.File);
                    }
                    else
                    {
                        results.Add(wif.Word, new CountFileList(wif.Count, wif.File));
                    }
                }
            }

            var orderedResults = from r in results.Take(200)
                                 orderby r.Value.Count descending
                                 select r;

            Dictionary<Word, CountFileList> orderedResultsList = new Dictionary<Word, CountFileList>(200);
            foreach (KeyValuePair<Word, CountFileList> kvp in orderedResults)
            {
                orderedResultsList.Add(kvp.Key, kvp.Value);
            }
            return orderedResultsList;
        }
    }
}