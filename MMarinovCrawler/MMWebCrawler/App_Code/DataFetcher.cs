using System.Linq;

/// <summary>
/// Summary description for DataFetcher
/// </summary>
public class DataFetcher
{
    private static string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionStringActive"].ToString();
    private System.Collections.Generic.List<DALWebCrawlerActive.File> files;
    private static readonly char[] space = new char[] { ' ' };

    public DataFetcher(string query)
    {
        using (DALWebCrawlerActive.WebCrawlerActiveDataContext dataContext = new DALWebCrawlerActive.WebCrawlerActiveDataContext(connectionString))
        {
            DALWebCrawlerActive.Word w = dataContext.Words.SingleOrDefault(ww => ww.WordName == query);
            if (w.WordsInFiles.Count > 0)
            {

            }

            IQueryable<string[]> wordsQuery = from f in dataContext.Files
                                              where (f.ImportantWords + f.WeightedWords).Contains(query)
                                              select (f.ImportantWords + f.WeightedWords).Split(space, System.StringSplitOptions.RemoveEmptyEntries);
       
            foreach (string[] wordsInFile in wordsQuery)
            {

            }
        }
    }

    public int FilesCount
    {
        get { return files.Count(); }
    }

    public System.Collections.Generic.List<DALWebCrawlerActive.File> Files
    {
        get { return files; }
    }
}