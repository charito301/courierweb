using System;

namespace MMarinov.WebCrawler.Indexer
{
    public abstract class Document
    {
        public enum DoumentTypes
        {
            HTML = 1,
            Text = 2,
            Mp3 = 3,
            PDF = 4
        }

        private Uri _Uri;
        private string _allCode;
        private string _ContentType;
        private string _MimeType = "";
        private string _Title;
        private string _Description = "";

        public static Int64 FoundValidLinks = 0;
        public static Int64 FoundTotalLinks = 0;

        private System.Collections.Generic.List<string> _localLinks;
        private System.Collections.Generic.List<string> _externalLinks;

        public abstract bool GetResponse(System.Net.HttpWebResponse webresponse);
        public abstract void Parse();

        public delegate void DocumentProgressEventHandler(Report.ProgressEventArgs pea);
        public event DocumentProgressEventHandler DocumentEvent;

        protected void DocumentProgressEvent(Report.ProgressEventArgs pea)
        {
            if (this.DocumentEvent != null)
            {
                DocumentEvent(pea);
            }
        }

        public System.Collections.Generic.List<string> LocalLinks
        {
            get
            {
                return _localLinks;
            }
            set
            {
                _localLinks = value;
            }
        }
        public System.Collections.Generic.List<string> ExternalLinks
        {
            get
            {
                return _externalLinks;
            }
            set
            {
                _externalLinks = value;
            }
        }

        public virtual string AllCode
        {
            get { return _allCode; }
            set { _allCode = value; }
        }
        public virtual string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        public virtual string MimeType
        {
            get { return _MimeType; }
            set { _MimeType = value; }
        }

        public abstract string WordsOnly { get; }

        public virtual string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// http://www.ietf.org/rfc/rfc2396.txt
        /// </summary>
        public virtual Uri Uri
        {
            get { return _Uri; }
            set { _Uri = value; }
        }

        public virtual string[] WordsArray
        {
            get { return this.WordsStringToArray(WordsOnly); }
        }

        /// <summary>
        /// Most document types don't have embedded robot information
        /// so they'll always be allowed to be followed 
        /// (assuming there are links to follow)
        /// </summary>
        public virtual bool RobotFollowOK
        {
            get { return true; }
        }
        /// <summary>
        /// Most document types don't have embedded robot information
        /// so they'll always be allowed to be indexed 
        /// (assuming there is content to index)
        /// </summary>
        public virtual bool RobotIndexOK
        {
            get { return true; }
        }

        /// <summary>
        /// Constructor for any document requires the Uri be specified
        /// </summary>
        public Document(Uri uri)
        {
            _Uri = uri;
        }

        /// <summary>
        /// Constructor for any document requires the Uri and MimeType to be specified
        /// </summary>
        public Document(Uri uri, string mimeType)
        {
            _Uri = uri;
            _MimeType = mimeType;
        }

        /// <summary>
        /// COMPRESS ALL WHITESPACE into a single space, seperating words
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        protected string[] WordsStringToArray(string words)
        {
            if (words.Length > 0)
            {
                return System.Text.RegularExpressions.Regex.Replace(words, Common.MatchEmptySpacesPattern, " ").Split(Common.Separators, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[0];
            }
        }

        //protected string GetDescriptionFromWordsOnly(string wordsonly)
        //{
        //    string description = "";
        //    if (wordsonly.Length > Preferences.SummaryCharacters)
        //    {
        //        description = wordsonly.Substring(0, Preferences.SummaryCharacters);
        //    }
        //    else
        //    {
        //        description = WordsOnly;
        //    }

        //    return System.Text.RegularExpressions.Regex.Replace(description, @"\s+", " ").Trim();
        //}

        /// <summary>
        /// Is the value of the href pointing to a web page?
        /// </summary>
        /// <param name="foundHref">The value of the href that needs to be interogated.</param>
        /// <returns>Boolen </returns>
        public static bool IsAWebPage(string foundHref)
        {
            if (foundHref.Length < 2 || foundHref.IndexOf("javascript:") > -1 || foundHref.IndexOf("mailto:") > -1 || foundHref.StartsWith("#") || foundHref.StartsWith("file://") || foundHref.StartsWith(@"\\") || foundHref.StartsWith("ftp://"))
            {
                return false;
            }

            if (!Uri.IsWellFormedUriString(foundHref, UriKind.RelativeOrAbsolute))
            {
                return false;
            }

            string extension = "";

            try
            {
                Uri uri = new Uri(foundHref);
                if (uri.Segments.Length == 0)
                {
                }
                string lastSegment = uri.Segments[uri.Segments.Length - 1];

                if (lastSegment.Contains("."))
                {
                    extension = lastSegment.Substring(lastSegment.LastIndexOf(".") + 1, lastSegment.Length - lastSegment.LastIndexOf(".") - 1);
                }
                else
                {
                    return true;
                }
            }
            catch//relative url
            {
                if (foundHref.Contains("."))
                {
                    extension = foundHref.Substring(foundHref.LastIndexOf(".") + 1, foundHref.Length - foundHref.LastIndexOf(".") - 1);
                }
                else
                {
                    return true;
                }
            }

            switch (extension)
            {
                case "htm":
                case "html":
                case "shtml":
                case "dhtml":
                case "xhtml":
                case "asp":
                case "aspx":
                case "cgi":
                case "php":
                case "jsf":
                case "jsp":
                case "pl":
                case "txt":
                    return true;
                case "mp3":
                case "pdf":
                    return !Preferences.IndexOnlyHTMLDocuments;
                default:
                    return false;
            }
        }

        /// <summary>
        /// False if the link exists, so the website must be skipped
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>False if the link exists</returns>
        protected bool AddURLtoGlobalVisited(Uri uri)
        {
            lock (Spider.GlobalVisitedURLs)
            {
                string link = Common.GetHttpAuthority(uri);
                if (!Spider.GlobalVisitedURLs.Contains(link))
                {
                    Spider.GlobalVisitedURLs.Add(link);

                    DocumentProgressEvent(new Report.ProgressEventArgs(Report.EventTypes.Start, "Crawling website(redirected):" + link));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected bool DeleteFile(string filename)
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