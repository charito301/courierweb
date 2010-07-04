using System.Linq;

/// <summary>
/// Summary description for DataFetcher
/// </summary>
public class DataFetcher
{
    private static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionStringActive"].ToString();
    public DataFetcher(string query)
    {
        System.Collections.Generic.List<DALWebCrawlerActive.File> files;


        using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataContext = new DALWebCrawlerActive.WebCrawlerActiveDataContext(connectionString))
        {
            IQueryable<DALWebCrawlerActive.File> aa = from f in dataContext.Files
                                                      join wif in dataContext.WordsInFiles on f.ID equals wif.FileID
                                                      join w in dataContext.Words on wif.WordID equals w.ID
                                                      where w.WordName == query
                                                      select f;

            files = aa.ToList();
            //var asd = from w in dataContext.Words
            //          where w.WordName == query
            //          select w;
        }

        int count = files.Count();
    }
}