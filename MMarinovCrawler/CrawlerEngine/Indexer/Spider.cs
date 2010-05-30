using System;
using System.Xml.Serialization;
using System.Collections.Specialized;
using MMarinov.WebCrawler.Report;

namespace MMarinov.WebCrawler.Indexer
{
    /// <summary>
    /// The Spider that crawls a website, link by link.
    /// </summary>
    public class Spider
    {
        internal static ThreadedGenerics.TList<string> GlobalURLsToVisit = new ThreadedGenerics.TList<string>();
        internal static ThreadedGenerics.TList<string> GlobalVisitedURLs = new ThreadedGenerics.TList<string>();

        #region Private fields

        //private object _externalLinksSyncObj = new object();
        //private object _visitedLinksSyncObj = new object();
        //private object _GlobalURLsToVisitSyncObj = new object();
        //private object _GlobalVisitedURLsSyncObj = new object();

        private volatile bool _shouldStop = false;
        private System.Threading.Thread[] ThreadArray;
        //private System.Threading.EventWaitHandle _threadWaitHandle = new System.Threading.AutoResetEvent(false);

        private ThreadedGenerics.TList<string> _visitedLinks = new ThreadedGenerics.TList<string>();
        private ThreadedGenerics.TList<string> _externalLinks = new ThreadedGenerics.TList<string>();

        /// <summary></summary>
        private MMarinov.WebCrawler.Indexer.Catalog _Catalog;

        /// <summary>Stemmer to use</summary>
        private static MMarinov.WebCrawler.Stemming.IStemming _Stemmer;

        /// <summary>Stemmer to use</summary>
        private static MMarinov.WebCrawler.Stopper.IStopper _Stopper;

        /// <summary>Loads and acts as 'authorisation' for robot-excluded Urls</summary>
        private RobotsTxt _Robot;

        #endregion

        #region Public events/handlers: SpiderProgressEvent
        /// <summary>
        /// Event Handler to communicate progress and errors back to the calling code
        /// </summary>
        public event SpiderProgressEventHandler SpiderProgressEvent;

        /// <summary>
        /// Only trigger the event if a Handler has been attached.
        /// </summary>
        private void ProgressEvent(ProgressEventArgs pea)
        {
            if (this.SpiderProgressEvent != null)
            {
                SpiderProgressEvent(pea);
            }
        }
        #endregion

        public Spider()
        {
            ThreadArray = new System.Threading.Thread[Preferences.ThreadsCount];

            _shouldStop = false;

            try
            {
                for (int i = 0; i < Preferences.ThreadsCount; i++)
                {
                    ThreadArray[i] = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.BuildCatalog));
                    ThreadArray[i].Name = "SpiderThread " + i;
                    ThreadArray[i].Start((object)i);

                    ProgressEvent(new ProgressEventArgs(EventTypes.Start, i + " : Crawling started in " + DateTime.Now));
                }
            }
            catch (System.Exception e)
            {
                ProgressEvent(new ProgressEventArgs(new System.Exception("Error while creating Threads: ", e)));
            }
        }

        public void KillThreads()
        {
            _shouldStop = true;

            ProgressEvent(new ProgressEventArgs(EventTypes.End, "Killing threads..."));

            System.Threading.Thread.Sleep(1000);

            for (int i = 0; i < Preferences.ThreadsCount; i++)
            {
                if (ThreadArray[i].IsAlive)
                {
                    ThreadArray[i].Abort();
                    ProgressEvent(new ProgressEventArgs(EventTypes.End, "Thread " + i + " killed"));
                }
            }

            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Takes a single Uri (Url) and returns the catalog that is generated
        /// by following all the links from that point.
        /// </summary>
        /// <remarks>
        ///This is the MAIN method of the indexing system.
        /// </remarks>
        public void BuildCatalog(object threadID)
        {
            Uri startPageUri;

            while (!_shouldStop)
            {
                startPageUri = null;

                //lock (_GlobalVisitedURLsSyncObj)
                //{
                //    lock (_GlobalURLsToVisitSyncObj)
                //    {
                if (GlobalURLsToVisit.Count == 0)
                {
                    ProgressEvent(new ProgressEventArgs(EventTypes.End, (int)threadID + " : Empty GlobalURLsToVisit at: " + DateTime.Now));

                    StopThreadsOnEmptyURLsList();

                    continue;
                }

                ProgressEvent(new ProgressEventArgs(EventTypes.Crawling, (int)threadID + " : Get from GlobalURLsToVisit: " + GlobalURLsToVisit[0]));
                GlobalVisitedURLs.Add(GlobalURLsToVisit[0]);

                startPageUri = new Uri(GlobalURLsToVisit[0]);
                GlobalURLsToVisit.RemoveAt(0);
                //    }
                //}

                if (startPageUri != null)
                {
                    ProgressEvent(new ProgressEventArgs(EventTypes.Crawling, (int)threadID + " : Crawling website: " + startPageUri.AbsoluteUri));

                    InitListsAndPreferences();

                    _Robot = new RobotsTxt(startPageUri, Preferences.RobotUserAgent);

                    ProcessUri(startPageUri, 0);

                    MergeExternalGlobalLinks();

                    _Catalog.MergeResultsRange();

                    _Catalog.SaveWebSiteFilesToDB();
                }
                else
                {
                    ProgressEvent(new ProgressEventArgs(EventTypes.End, (int)threadID + " : No more tasks - wait for a signal "));
                }
            }

            ProgressEvent(new ProgressEventArgs(EventTypes.End, (int)threadID + " Crawling finished at: " + DateTime.Now));
        }

        private void InitListsAndPreferences()
        {
            _Catalog = new MMarinov.WebCrawler.Indexer.Catalog();
            _visitedLinks.Clear();
            _externalLinks.Clear();

            // Setup Stop, Go, Stemming
            SetPreferences();
        }

        /// <summary>
        /// Checks if all threads are not working and kills them.
        /// </summary>
        /// <returns></returns>
        private bool StopThreadsOnEmptyURLsList()
        {
            int nonWorkingThreads = 0;

            foreach (System.Threading.Thread thread in ThreadArray)
            {
                if (!thread.IsAlive)
                {
                    nonWorkingThreads++;
                }
            }

            if (nonWorkingThreads == Preferences.ThreadsCount)
            {
                _shouldStop = true;

                KillThreads();
            }

            return _shouldStop;
        }

        private void MergeExternalGlobalLinks()
        {
            //lock (_externalLinksSyncObj)
            //{
            //    lock (_GlobalURLsToVisitSyncObj)
            //    {
            foreach (string link in _externalLinks)
            {
                if (!GlobalVisitedURLs.Contains(link) && !GlobalURLsToVisit.Contains(link))
                {
                    GlobalURLsToVisit.Add(link);
                }
            }
            //    }
            //}
        }

        /// <summary>
        /// Merges the external links for the current website with these, which are found in the local pages
        /// </summary>
        /// <param name="sourceList">The list whose values need to be merged.</param>
        private void MergeExternalLinks(System.Collections.Generic.List<string> sourceList)
        {
            //lock (_externalLinksSyncObj)
            //{
            foreach (string str in sourceList)
            {
                if (!_externalLinks.Contains(str))
                {
                    _externalLinks.Add(str);
                }
            }
            //}
        }

        /// <summary>
        /// Setup Stop and Stemming
        /// </summary>
        private void SetPreferences()
        {
            if (Preferences.StemmingModeEnabled)
            {
                _Stemmer = new Stemming.PorterStemmer();	//Stemming enabled.
            }
            else
            {
                _Stemmer = new Stemming.NoStemming();//Stemming DISabled.
            }

            switch (Preferences.StoppingMode)
            {
                case Stopper.StoppingModes.Short:
                    _Stopper = new Stopper.ShortStopper();//Stop words shorter than 3 chars.
                    break;
                case Stopper.StoppingModes.List:
                    _Stopper = new Stopper.ListStopper();//Stop words from list.
                    break;
                case Stopper.StoppingModes.Off:
                    _Stopper = new Stopper.NoStopping();//Stopping DISabled.
                    break;
                default:
                    _Stopper = new Stopper.ShortStopper();
                    break;
            }
        }

        /// <summary>
        ///GETS THE FIRST DOCUMENT, AND STARTS THE SPIDER! -- create the 'root' document 
        // RECURSIVE CALL TO 'Process()' STARTS HERE
        /// </summary>
        protected int ProcessUri(Uri uri, int level)
        {
            if (level > Preferences.RecursionLimit)
            {
                return Preferences.RecursionLimit;
            }

            int wordcount = 0;
            string url = uri.AbsoluteUri;

            if (_Robot.Allowed(uri) && !_visitedLinks.Contains(url))
            {
                _visitedLinks.Add(url);

                Document downloadDocument = Download(uri);
                if (null != downloadDocument)
                {
                    downloadDocument.DocumentEvent += new Document.DocumentProgressEventHandler(downloadDocument_DocumentEvent);
                    downloadDocument.Parse();

                    MergeExternalLinks(downloadDocument.ExternalLinks);// Adds external links from every page from the site

                    if (downloadDocument.RobotIndexOK)
                    {
                        if (downloadDocument is HtmlDocument)
                        {
                            SetLanguages(((HtmlDocument)downloadDocument).Language);
                        }

                        wordcount = AddToCatalog(downloadDocument);
                    }
                }

                // ### Loop through the 'local' links in the document and parse each of them recursively ###
                if (null != downloadDocument && null != downloadDocument.LocalLinks && downloadDocument.RobotFollowOK)
                { // only if the Robot meta says it's OK
                    foreach (string link in downloadDocument.LocalLinks)
                    {
                        try
                        {
                            ProcessUri(new Uri(downloadDocument.Uri, link), level + 1); // calls THIS method!
                        }
                        catch (Exception ex)
                        {
                            ProgressEvent(new ProgressEventArgs(new Exception(" new Uri(" + downloadDocument.Uri + ", " + link + ") invalid : ", ex)));
                        }
                    }
                } // process local links
            } // robot allowed and not visited
            return level;
        }// ProcessUri

        private void downloadDocument_DocumentEvent(ProgressEventArgs pea)
        {
            ProgressEvent(pea);
        }

        /// <summary>
        /// Set current language of the HTML page to Stemmer and Stopper
        /// </summary>
        /// <param name="lang"></param>
        private void SetLanguages(Stemming.Languages lang)
        {
            if (_Stopper is Stopper.ListStopper)
            {
                ((Stopper.ListStopper)_Stopper).Language = lang;
            }

            if (_Stemmer is Stemming.PorterStemmer)
            {
                ((Stemming.PorterStemmer)_Stemmer).Language = lang;
            }
        }

        /// <summary>
        /// Attempts to download the Uri and (based on it's MimeType) use the DocumentFactory
        /// to get a Document subclass object that is able to parse the downloaded data.
        /// </summary>
        private Document Download(Uri uri)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            System.Net.ServicePointManager.MaxServicePointIdleTime = 2000;

            // Open the requested URL
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri.AbsoluteUri);
            req.UnsafeAuthenticatedConnectionSharing = true;
            req.AllowAutoRedirect = true;
            req.MaximumAutomaticRedirections = 3;
            req.UserAgent = Preferences.UserAgent; //"Mozilla/6.0 (MSIE 6.0; Windows NT 5.1; MMarinov.NET)";
            req.KeepAlive = true;
            req.Method = System.Net.WebRequestMethods.Http.Get;
            req.Timeout = Preferences.RequestTimeout * 1000;

            // Get the stream from the returned web response
            System.Net.HttpWebResponse webResponse = null;
            Document htmldoc = null;

            try
            {
                webResponse = (System.Net.HttpWebResponse)req.GetResponse();
            }
            catch (System.Net.WebException ex)
            {   //remote url not found, 404; remote url forbidden, 403
                ProgressEvent(new ProgressEventArgs(new Exception(uri.AbsoluteUri, ex), true));
            }
            finally
            {
                if (webResponse != null)
                {
                    htmldoc = DocumentFactory.New(uri, webResponse);

                    if (htmldoc != null)
                    {
                        htmldoc.GetResponse(webResponse);
                    }

                    webResponse.Close();
                }
            }

            return htmldoc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <return>Number of words catalogued</return>
        private int AddToCatalog(Document downloadDocument)
        {
            File infile = new File(downloadDocument);

            int i = 0;          // count of words
            string key = "";    // temp variable
            System.Text.StringBuilder formatedWords = new System.Text.StringBuilder();

            foreach (string word in downloadDocument.WordsArray)
            {
                key = word.ToLower();

                // Apply Stemmer (set by preferences)
                key = _Stemmer.StemWord(key);

                // Apply Stopper (set by preferences)
                key = _Stopper.StopWord(key);

                if (key != "")
                {
                    formatedWords.Append(key).Append(" ");
                    _Catalog.AddWordFilePair(key, infile, i);
                    i++;
                }
            } // foreach

            infile.SetWeightedWords(formatedWords.ToString());

            _Catalog.AddHtmlFile(infile);

            return i;
        }
    }
}