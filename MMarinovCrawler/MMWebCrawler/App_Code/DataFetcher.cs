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
        public static string ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionStringActive"].ToString();
        private static long tick1;
        private static long tick2;
        private static long tick3;
        private static int _totalLinksFound;
        private static long _shownLinks;

        public class CountFileList
        {
            public int Count = 0;
            public List<File> FilesList = new List<File>();

            public CountFileList(int count, File fileToAdd)
            {
                Count = count;
                FilesList.Add(fileToAdd);
            }

            public CountFileList(int count, List<File> fileList)
            {
                Count = count;
                FilesList = fileList;
            }
        }

        public static Dictionary<Word, CountFileList> FetchResults(string query)
        {
            IQueryable<DALWebCrawlerActive.WordsInFile> wordsInFiles;
            Dictionary<Word, CountFileList> results = new Dictionary<Word, CountFileList>();
            tick1 = DateTime.Now.Ticks;
            _shownLinks = 0;

            try
            {
                string[] queryWords = query.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataContext = new DALWebCrawlerActive.WebCrawlerActiveDataContext(ConnectionString))
                {
                    wordsInFiles = from wif in
                                       dataContext.WordsInFiles.Where(wif2 => (from wif in dataContext.WordsInFiles.Where(wif3 => queryWords.Contains(wif3.Word.WordName))
                                                                               orderby wif.Count descending
                                                                               select wif.FileID).Take(50).Contains(wif2.FileID)).Take(1000)
                                   select wif;

                    _totalLinksFound = wordsInFiles.Count();
                    tick2 = DateTime.Now.Ticks;

                    foreach (WordsInFile wif in wordsInFiles)
                    {
                        if (queryWords.Contains(wif.Word.WordName))
                        {
                            continue; // need improvment, bad loop
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
                long tick4 = DateTime.Now.Ticks;
                double a4 = TimeSpan.FromTicks(tick4 - tick2).TotalSeconds;

                var orderedResults = from r in results.Take(100)
                                     orderby r.Value.Count descending
                                     select r;

                Dictionary<Word, CountFileList> orderedResultsList = new Dictionary<Word, CountFileList>(100);
                foreach (KeyValuePair<Word, CountFileList> kvp in orderedResults)
                {
                    kvp.Value.FilesList = kvp.Value.FilesList.Take(100).ToList();
                    _shownLinks += kvp.Value.FilesList.Count;
                    orderedResultsList.Add(kvp.Key, kvp.Value);
                }

                tick3 = DateTime.Now.Ticks;

                return orderedResultsList;
            }
            catch
            {
                return null;
            }
        }

        public static double FetchTimeInSec
        {
            get
            {
                return TimeSpan.FromTicks(tick2 - tick1).TotalSeconds;
            }
        }

        public static double SortTimeInSec
        {
            get
            {
                return TimeSpan.FromTicks(tick3 - tick2).TotalSeconds;
            }
        }

        public static int TotalLinksFound
        {
            get
            {
                return _totalLinksFound;
            }
        }

        public static long ShownLinks
        {
            get
            {
                return _shownLinks;
            }
        }
    }
}