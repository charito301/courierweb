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

        private object _GlobalURLsToVisitSyncObj = new object();
        private object _GlobalVisitedURLsSyncObj = new object();

        private int _spiderIndex = 0;
        private System.Threading.Thread thread;
        // private System.Threading.EventWaitHandle _threadWaitHandle = new System.Threading.AutoResetEvent(false);

        private ThreadedGenerics.TList<string> _visitedLinks = new ThreadedGenerics.TList<string>();
        private ThreadedGenerics.TList<string> _externalLinks = new ThreadedGenerics.TList<string>();

        private static Int64 _totalCrawledLinks = 0;
        private static Int64 _totalSuccessfulLinks = 0;

        /// <summary></summary>
        private Library.Catalog _Catalog;

        /// <summary>Stemmer to use</summary>
        private static Stemming.IStemming _Stemmer;

        /// <summary>Stemmer to use</summary>
        private static Stopper.IStopper _Stopper;

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

        public Spider(int spiderIndex)
        {
            _spiderIndex = spiderIndex;
        }

        #region Thread methods

        public void StartThread()
        {
            try
            {
                thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.BuildCatalog));
                thread.Name = "[Spider " + _spiderIndex + "]";
                thread.Start((object)_spiderIndex);

                ProgressEvent(new ProgressEventArgs(EventTypes.Start, thread.Name + " started in " + DateTime.Now));
            }
            catch (System.Exception e)
            {
                ProgressEvent(new ProgressEventArgs(new System.Exception("Error while creating " + thread.Name, e)));
            }
        }

        public void KillThread()
        {
            CrawlingManager.ShouldStopThreads = true;

            ProgressEvent(new ProgressEventArgs(EventTypes.End, "Killing spider ... " + thread.Name));

            if (thread.IsAlive)
            {
                try
                {
                    thread.Abort();

                    ProgressEvent(new ProgressEventArgs(new Exception(thread.Name + " killed.")));
                }
                catch (System.Threading.ThreadAbortException e)
                {
                    ProgressEvent(new ProgressEventArgs(EventTypes.End, thread.Name + " couldn't stop! :" + e.Message));
                }
            }
        }

        public void WakeWaitingThead()
        {
            if (thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
            {
                thread.Interrupt();
            }
        } 

        #endregion

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

            while (!CrawlingManager.ShouldStopThreads)
            {
                startPageUri = null;

                lock (GlobalURLsToVisit)
                {
                    lock (GlobalVisitedURLs)
                    {
                        if (GlobalURLsToVisit.Count == 0)
                        {
                            if (!StopThreadsOnEmptyURLsList())
                            {
                                thread.Join();
                            }
                        }
                        else
                        {
                            ProgressEvent(new ProgressEventArgs(EventTypes.WakeJoinedThreads, thread.Name + ": !!! Wake waiting threads at: " + DateTime.Now));

                            //ProgressEvent(new ProgressEventArgs(EventTypes.Start, thread.Name + " : Get from GlobalURLsToVisit: " + GlobalURLsToVisit[0]));
                            GlobalVisitedURLs.Add(GlobalURLsToVisit[0]);

                            startPageUri = new Uri(GlobalURLsToVisit[0]);
                            GlobalURLsToVisit.RemoveAt(0);
                        }
                    }
                }

                if (startPageUri != null)
                {
                    ProgressEvent(new ProgressEventArgs(EventTypes.Start, thread.Name + ": Crawling website: " + startPageUri.AbsoluteUri));

                    InitListsAndPreferences();

                    _Robot = new RobotsTxt(startPageUri, Preferences.RobotUserAgent);

                    ProcessUri(startPageUri, 0);

                    MergeExternalGlobalLinks();

                    _Catalog.MergeResultsRange(CrawlingManager.GlobalCatalog);

                    _Catalog.SaveResultsToDB();

                    ProgressEvent(new ProgressEventArgs(EventTypes.End, thread.Name + ": GlobalCatalog => Words(" + CrawlingManager.GlobalCatalog.Words.Count + ") Files(" + CrawlingManager.GlobalCatalog.Files.Count + ")"));
                }
                else
                {
                    ProgressEvent(new ProgressEventArgs(EventTypes.End, thread.Name + ": !!! No more tasks - wait for a signal "));
                }
            }

            ProgressEvent(new ProgressEventArgs(EventTypes.End, thread.Name + ": !!! Crawling finished at: " + DateTime.Now));
        }

        private void InitListsAndPreferences()
        {
            _Catalog = new Library.Catalog();
            _visitedLinks.Clear();
            _externalLinks.Clear();

            // Setup Stop, Go, Stemming
            SetPreferences();
        }

        /// <summary>
        /// Checks if all threads are not working and kills them.
        /// </summary>
        /// <returns>True if Should Stop Threads</returns>
        private bool StopThreadsOnEmptyURLsList()
        {
            CrawlingManager.WaitingThreadsCount++;

            ProgressEvent(new ProgressEventArgs(EventTypes.EmptyVisitedURLs, thread.Name + " : !!! Empty GlobalURLsToVisit at: " + DateTime.Now));

            if (CrawlingManager.WaitingThreadsCount == Preferences.ThreadsCount)
            {
                KillThread();
            }

            return CrawlingManager.ShouldStopThreads;
        }

        private void MergeExternalGlobalLinks()
        {
            foreach (string link in _externalLinks)
            {
                if (!GlobalVisitedURLs.Contains(link) && !GlobalURLsToVisit.Contains(link))
                {
                    GlobalURLsToVisit.Add(link);
                }
            }
        }

        /// <summary>
        /// Merges the external links for the current website with these, which are found in the local pages
        /// </summary>
        /// <param name="sourceList">The list whose values need to be merged.</param>
        private void MergeExternalLinks(System.Collections.Generic.List<string> sourceList)
        {
            foreach (string str in sourceList)
            {
                if (!_externalLinks.Contains(str))
                {
                    _externalLinks.Add(str);
                }
            }
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
        ///GETS THE FIRST DOCUMENT, AND STARTS THE SPIDER!
        // RECURSIVE CALL
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
                _totalCrawledLinks++;

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

                        ProgressEvent(new ProgressEventArgs(EventTypes.Crawling, thread.Name + " " + ++_totalSuccessfulLinks + ": " + url + " [" + wordcount + " words]"));
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
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri.AbsoluteUri);
            webRequest.UnsafeAuthenticatedConnectionSharing = true;
            webRequest.AllowAutoRedirect = true;
            webRequest.MaximumAutomaticRedirections = 3;
            webRequest.UserAgent = Preferences.UserAgent; //"Mozilla/6.0 (MSIE 6.0; Windows NT 5.1; MMarinov.NET)";
            webRequest.KeepAlive = true;
            webRequest.Method = System.Net.WebRequestMethods.Http.Get;
            webRequest.Timeout = Preferences.RequestTimeout * 1000;

            // Get the stream from the returned web response
            System.Net.HttpWebResponse webResponse = null;
            Document htmldoc = null;

            try
            {
                webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();
            }
            catch (System.Net.WebException ex)
            {   //remote url not found, 404; remote url forbidden, 403
                ProgressEvent(new ProgressEventArgs(new Exception(uri.AbsoluteUri, ex), ex.Status));
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
            DALWebCrawler.File infile = new DALWebCrawler.File()
            {
                FileType = (byte)Library.FileManipulator.SetFileType(downloadDocument),
                ImportantWords = Library.FileManipulator.SetImportantWords(downloadDocument),
                URL = Common.GetHttpAuthority(downloadDocument.Uri) + downloadDocument.Uri.AbsolutePath//do wordsCount need that again here ??
            };

            int wordsCount = 0;          
            string key = "";    // temp variable
            System.Text.StringBuilder formatedWords = new System.Text.StringBuilder();

            foreach (string word in downloadDocument.WordsArray)
            {
                key = word.ToLower();

                // Apply Stemming and stopping (set by preferences)
                key = _Stemmer.StemWord(key);
                key = _Stopper.StopWord(key);

                if (key != "")
                {
                    formatedWords.AppendLine(key);

                    _Catalog.AddWordFilePair(key, infile);

                    wordsCount++;
                }
            } // foreach

            infile.WeightedWords = Library.FileManipulator.SetWeightedWords(formatedWords.ToString());

            _Catalog.AddHtmlFile(infile);

            return wordsCount;
        }
    }
}