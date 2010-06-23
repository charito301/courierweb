using System;
using System.Linq;

namespace MMarinov.WebCrawler.Library
{
    public class FileManipulator
    {
        /// <summary>
        /// Set words without change, but ordered - every word occurs once
        /// </summary>
        /// <param name="downloadDocument"></param>
        public static string SetImportantWords(Indexer.Document downloadDocument)
        {
            System.Text.StringBuilder importantWords = new System.Text.StringBuilder();

            try
            {
                Uri uri = new Uri(downloadDocument.Uri.AbsoluteUri);
                importantWords.AppendLine(Common.GetAuthority(uri)).AppendLine(uri.AbsolutePath);
            }
            catch { }

            importantWords.AppendLine(downloadDocument.Title).AppendLine(downloadDocument.Description);

            if (downloadDocument is Indexer.HtmlDocument)
            {
                importantWords.AppendLine(((Indexer.HtmlDocument)downloadDocument).Keywords);
            }

            string[] urlWords = importantWords.ToString().ToLower().Split(Common.Separators, StringSplitOptions.RemoveEmptyEntries);

            importantWords = new System.Text.StringBuilder();

            foreach (string word in urlWords)
            {
                if (word.Length > 2)
                {
                    importantWords.Append(word).Append(" ");
                }
            }

            return WeightWords(importantWords.ToString());
        }

        /// <summary>
        /// Before setting the words pass the throug Stopper and Stemmer!
        /// </summary>
        /// <param name="content"></param>
        public static string SetWeightedWords(string content)
        {
            return WeightWords(content);
        }

        private static string WeightWords(string words)
        {
            const int maxWordsCount = 50;

            System.Collections.Generic.Dictionary<string, int> weightWords = new System.Collections.Generic.Dictionary<string, int>();

            if (words.Length == 0)
            {
                return "";
            }

            string[] wordsArray = System.Text.RegularExpressions.Regex.Replace(words, Common.MatchEmptySpacesPattern, " ").Split(Common.Separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in wordsArray)
            {
                if (weightWords.ContainsKey(word))
                {
                    weightWords[word]++;
                }
                else
                {
                    weightWords.Add(word, 1);
                }
            }

            // Use LINQ to specify sorting by value.
            System.Linq.IOrderedEnumerable<string> orderedWords = from word in weightWords.Keys
                                                                  orderby weightWords[word] descending
                                                                  select word;

            System.Text.StringBuilder weightedWords = new System.Text.StringBuilder();

            foreach (string word in orderedWords.Take(maxWordsCount))
            {
                weightedWords.Append(word).Append(' ');
            }

            return weightedWords.ToString();
        }

        public static Indexer.Document.DoumentTypes SetFileType(Indexer.Document downloadDocument)
        {
            if (downloadDocument is MMarinov.WebCrawler.Indexer.HtmlDocument)
            {
                return Indexer.Document.DoumentTypes.HTML;
            }
            else if (downloadDocument is Indexer.TextDocument)
            {
                return Indexer.Document.DoumentTypes.Text;
            }
            else if (downloadDocument is Indexer.PdfDocument)
            {
                return Indexer.Document.DoumentTypes.PDF;
            }
            else if (downloadDocument is Indexer.Mp3Document)
            {
                return Indexer.Document.DoumentTypes.Mp3;
            }

            return Indexer.Document.DoumentTypes.HTML;
        }

        public static bool InsertFilesIntoDB(System.Collections.Generic.ICollection<DALWebCrawler.File> files)
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.Files.InsertAllOnSubmit(files);
                dataContext.SubmitChanges();

                return true;
            }
        }
    }
}
