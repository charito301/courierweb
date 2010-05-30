using System;
using System.Linq;

namespace MMarinov.WebCrawler.Indexer
{
    /// <summary>
    /// File attributes
    /// </summary>
    /// <remarks>
    /// *Beware* ambiguity with System.IO.File - always fully qualify File object references
    /// </remarks>
    [Serializable]
    public class File
    {
        #region private & public Fields

        private DoumentTypes _documentType = DoumentTypes.HTML;
        private string _Url;
        private string _Title;
        private string _Description;
        private string _importantWords;
        private string _weightedContent;

        private string _keywords;

        public string Url
        {
            get { return _Url; }
        }

        public string Title
        {
            get { return _Title; }
        }

        public string Description
        {
            get { return _Description; }
        }

        public string ImportantWords
        {
            get { return _importantWords; }
        }

        /// <summary>
        /// Words from the content, ordered descending by their count
        /// </summary>
        public string WeightedContent
        {
            get { return _weightedContent; }
        }

        #endregion

        /// <summary>
        /// Constructor requires all File attributes
        /// </summary>
        public File(MMarinov.WebCrawler.Indexer.Document downloadDocument)
        {
            if (downloadDocument is MMarinov.WebCrawler.Indexer.HtmlDocument)
            {
                MMarinov.WebCrawler.Indexer.HtmlDocument htmlDoc = (MMarinov.WebCrawler.Indexer.HtmlDocument)downloadDocument;

                _keywords = htmlDoc.Keywords;

                _documentType = DoumentTypes.HTML;
            }
            else if (downloadDocument is MMarinov.WebCrawler.Indexer.TextDocument)
            {

                _documentType = DoumentTypes.Text;
            }
            else if (downloadDocument is MMarinov.WebCrawler.Indexer.PdfDocument)
            {

                _documentType = DoumentTypes.PDF;
            }
            else if (downloadDocument is MMarinov.WebCrawler.Indexer.Mp3Document)
            {

                _documentType = DoumentTypes.Mp3;
            }

            _Title = downloadDocument.Title;
            _Url = "http://" + downloadDocument.Uri.Authority.Replace("www.", "").Replace("www2.", "") + downloadDocument.Uri.AbsolutePath;
            _Description = downloadDocument.Description + downloadDocument.WordsOnly;

            SetImportantWords(downloadDocument);
        }

        /// <summary>
        /// Set words without change, but ordered - every word occurs once
        /// </summary>
        /// <param name="downloadDocument"></param>
        private void SetImportantWords(MMarinov.WebCrawler.Indexer.Document downloadDocument)
        {
            string url = "";
            try
            {
                Uri uri = new Uri(_Url);
                url = uri.Authority + uri.AbsolutePath;
            }
            catch { }

            _importantWords = (this._keywords + " " + this._Title + " " + url + " " + this._Description).ToLower();

            string[] urlWords = _importantWords.Split(Common.Separators, StringSplitOptions.RemoveEmptyEntries);
            _importantWords = "";

            System.Text.StringBuilder readyWords = new System.Text.StringBuilder();
            foreach (string word in urlWords)
            {
                if (word.Length > 2)
                {
                    readyWords.Append(word).Append(" ");
                }
            }

            _importantWords = WeightWords(readyWords.ToString());
        }

        /// <summary>
        /// Before setting the words pass the throug Stopper and Stemmer!
        /// </summary>
        /// <param name="content"></param>
        public void SetWeightedWords(string content)
        {
            _weightedContent = WeightWords(content);
        }

        private string WeightWords(string words)
        {
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

            string weightedWords = "";

            foreach (string word in orderedWords)
            {
                weightedWords += word + ' ';
            }

            return weightedWords;
        }

        private bool DeleteFile(string filename)
        {
            // delete file
            try
            {
                new System.IO.FileInfo(filename).Delete();
            }
            catch (Exception e)
            {
                Report.Logger.ErrorLog(e);
                return false;
            }

            return true;
        }
    }
}
