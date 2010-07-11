using System.Linq;
using System.Collections.Generic;
using System.Collections;
using DALWebCrawlerActive;

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
            using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataContext = new DALWebCrawlerActive.WebCrawlerActiveDataContext(connectionString))
            {
                string[] queryWords = query.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                IQueryable<IEnumerable<long>> innerQuery = from w in dataContext.Words
                                                           where queryWords.Contains(w.WordName)
                                                           orderby w.WordsInFiles.Count descending
                                                           select w.WordsInFiles.Select(wif => wif.FileID);//all files that contain the words ordered by count desc

                List<long> fileIDs = new List<long>();
                foreach (IEnumerable<long> allFileIDs in innerQuery)
                {
                    foreach (long fileId in allFileIDs)
                    {
                        if (!fileIDs.Contains(fileId))
                        {
                            fileIDs.Add(fileId);
                        }
                        else
                        {//debug
                        }
                    }
                }

                IOrderedQueryable<WordsInFile> wordsinfileList = from wif in dataContext.WordsInFiles
                                                                 where fileIDs.Contains(wif.FileID)
                                                                 orderby wif.Count descending
                                                                 select wif;

                Dictionary<Word, CountFileList> results = new Dictionary<Word, CountFileList>();
                foreach (WordsInFile wif in wordsinfileList)
                {
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

                return results;

                //            select * from Words 
                //inner join WordsInFiles on WordsInFiles.WordID = Words.ID
                //where Words.ID in (
                //select top 100 WordsInFiles.FileID from Words
                //inner join WordsInFiles on WordsInFiles.WordID = Words.ID
                //where WordName='live' )
                // order by count desc
            }
        }
    }
}