using System;
using System.Linq;
using Csla;
using Csla.Data;

namespace MMarinov.WebCrawler.Library
{
    /// <summary>
    /// File attributes
    /// </summary>
    /// <remarks>
    /// *Beware* ambiguity with System.IO.File - always fully qualify File object references
    /// </remarks>
    [Serializable]
    public class File : Csla.BusinessBase<File>
    {
        #region private & public Fields

        private long _id = 0;
        private Indexer.Document.DoumentTypes _documentType = Indexer.Document.DoumentTypes.HTML;
        private string _Url;
        private string _importantWords;
        private string _weightedContent;

        private string _Title;
        private string _Description;
        private string _keywords;

        public long ID
        {
            get { return _id; }
        }

        public string Keywords
        {
            get { return _keywords; }
            set
            {
                if (_keywords != value)
                { _keywords = value; }
            }
        }

        public string Url
        {
            get { return _Url; }
            set
            {
                if (_Url != value)
                {
                    _Url = value;
                }
            }
        }

        public Indexer.Document.DoumentTypes DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                }
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        public string ImportantWords
        {
            get { return _importantWords; }
            set
            {
                if (_importantWords != value)
                {
                    _importantWords = value;
                }
            }
        }

        /// <summary>
        /// Words from the content, ordered descending by their count
        /// </summary>
        public string WeightedContent
        {
            get { return _weightedContent; }
            set
            {
                if (_weightedContent != value)
                {
                    _weightedContent = value;
                }
            }
        }

        #endregion

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

        #region Constructor

        private File()
        {
            MarkAsChild();
        }

        #endregion

        #region Static Methods

        public static File NewFile(Indexer.Document downloadDocument)
        {
            File file = DataPortal.Create<File>();

            if (downloadDocument is MMarinov.WebCrawler.Indexer.HtmlDocument)
            {
                Indexer.HtmlDocument htmlDoc = (Indexer.HtmlDocument)downloadDocument;

                file.Keywords = htmlDoc.Keywords;

                file.DocumentType = Indexer.Document.DoumentTypes.HTML;
            }
            else if (downloadDocument is Indexer.TextDocument)
            {
                file.DocumentType = Indexer.Document.DoumentTypes.Text;
            }
            else if (downloadDocument is Indexer.PdfDocument)
            {

                file.DocumentType = Indexer.Document.DoumentTypes.PDF;
            }
            else if (downloadDocument is Indexer.Mp3Document)
            {

                file.DocumentType = Indexer.Document.DoumentTypes.Mp3;
            }

            file.Title = downloadDocument.Title;
            file.Url = "http://" + downloadDocument.Uri.Authority.Replace("www.", "").Replace("www2.", "") + downloadDocument.Uri.AbsolutePath;
            file.Description = downloadDocument.Description + downloadDocument.WordsOnly;

            file.SetImportantWords(downloadDocument);

            return file;
        }

        public static File GetFile(int id)
        {
            return DataPortal.Fetch<File>(new Criteria(id));
        }

        public static File GetFile(string url)
        {
            return DataPortal.Fetch<File>(new Criteria(url));
        }

        //public static File GetFile(DALWebCrawler.File data)
        //{
        //    File item = new File();
        //    item.Fetch(data);
        //    return item;
        //}

        #endregion

        #region DataAccess

        #region Criteria
        [Serializable]
        private class Criteria
        {
            public enum FetchTypes
            {
                ID,
                URL
            }

            public FetchTypes FetchType = FetchTypes.ID;

            public int ID = 0;

            public Criteria(int id)
            {
                this.ID = id;
                FetchType = FetchTypes.ID;
            }

            public string URL = "";

            public Criteria(string url)
            {
                this.URL = url;
                FetchType = FetchTypes.URL;
            }

        }
        #endregion

        #region DataPortal Create

        protected override void DataPortal_Create()
        {
            ValidationRules.CheckRules();
        }

        #endregion

        //#region DataPortal Fetch

        //protected override void DataPortal_Fetch(object criteria)
        //{
        //    using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
        //    {
        //        var file = mgr.DataContext.sp_SelectFile(((Criteria)criteria).ID);

        //        Fetch((DALWebCrawler.File)file);

        //        MarkOld();
        //    }
        //}

        //private void Fetch(DALWebCrawler.File data)
        //{
        //    if (data == null)
        //    {
        //        return;
        //    }

        //    _id = data.ID;
        //    _documentType = (Indexer.Document.DoumentTypes)data.FileType;
        //    _importantWords = data.ImportantWords;
        //    _Url = data.URL;
        //    _weightedContent = data.WeightedWords;

        //    MarkOld();
        //}

        //#endregion

        #region DataPortal Insert

        private void Child_Insert()
        {
            if (!IsDirty && !IsNew)
            {
                return;
            }

            DataPortal_Insert();
        }

        private void Child_Update()
        {
            if (!IsDirty)
            {
                return;
            }

            DataPortal_Update();
        }


        protected override void DataPortal_Insert()
        {
            using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
            {
                long? newID = 0;

                mgr.DataContext.sp_InsertFile(_Url, _importantWords, _weightedContent, (byte)_documentType, ref newID);

                _id = newID ?? 0;

                MarkOld();
            }
        }

        #endregion

        #region DataPortal Update

        protected override void DataPortal_Update()
        {
            using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
            {
                mgr.DataContext.sp_UpdateFile(_id, _Url, _importantWords, _weightedContent, (byte)_documentType);
                MarkOld();
            }
        }

        #endregion

        #endregion
    }
}
