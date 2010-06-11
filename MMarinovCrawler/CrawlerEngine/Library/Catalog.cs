using System;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace MMarinov.WebCrawler.Library
{
    /// <summary>
    /// Catalog of Words (and Files)
    /// <summary>
    [Serializable]
    public class Catalog
    {
        /// <summary>
        /// Internal datastore of Words referencing Files
        /// </summary>
        /// <remarks>
        /// Hashtable
        /// key    = STRING representation of the word, 
        /// value  = Word OBJECT (with File collection, etc)
        /// </remarks>
        //private ThreadedGenerics.TDictionary<string, Word> _IndexOfWords = new ThreadedGenerics.TDictionary<string, Word>();
        /// <summary>
        /// File collection that will keep link-keywords info
        /// </summary>
        //private ThreadedGenerics.TDictionary<string, File> _filesWithKeywords = new ThreadedGenerics.TDictionary<string, File>();

        private ThreadedGenerics.TDictionary<string, DALWebCrawler.Word> _wordColl = new ThreadedGenerics.TDictionary<string, DALWebCrawler.Word>();
        private ThreadedGenerics.TDictionary<string, DALWebCrawler.File> _fileColl = new ThreadedGenerics.TDictionary<string, DALWebCrawler.File>();
        private ThreadedGenerics.TList<DALWebCrawler.WordsInFile> _wordInFileColl = new MMarinov.ThreadedGenerics.TList<DALWebCrawler.WordsInFile>();

        /// <summary>
        /// Key(wordName), 
        /// Value(Hashtable:Key(URL)
        ///                 Value(FileCountPair))
        /// </summary>
        private ThreadedGenerics.TDictionary<string, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>> _fileCountPairColl = new ThreadedGenerics.TDictionary<string, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>>();

        #region Public properties

        public ThreadedGenerics.TDictionary<string, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>> FileCountPairs
        {
            get { return _fileCountPairColl; }
        }
        public ThreadedGenerics.TList<DALWebCrawler.WordsInFile> WordsInFiles
        {
            get { return _wordInFileColl; }
        }

        /// <summary>
        /// Words in the Catalog
        /// </summary>
        /// <remarks>
        /// Added property to allow Serialization to disk
        /// </remarks>
        public ThreadedGenerics.TDictionary<string, DALWebCrawler.Word> Words
        {
            get { return _wordColl; }
        }

        /// <summary>
        /// File collection that will keep link-keywords info
        /// </summary>
        public ThreadedGenerics.TDictionary<string, DALWebCrawler.File> Files
        {
            get { return _fileColl; }
        }

        /// <summary>
        /// Number of Words in the Catalog
        /// </summary>
        public int WordsCount
        {
            get { return _wordColl.Count; }
        }

        #endregion

        /// <summary>
        /// Constructor - creates collections for internal data storage.
        /// </summary>
        public Catalog()
        {
            _wordColl = new ThreadedGenerics.TDictionary<string, DALWebCrawler.Word>();
            _fileColl = new ThreadedGenerics.TDictionary<string, DALWebCrawler.File>();
            _wordInFileColl = new MMarinov.ThreadedGenerics.TList<DALWebCrawler.WordsInFile>();
            _fileCountPairColl = new MMarinov.ThreadedGenerics.TDictionary<string, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>>();
        }

        /// <summary>
        /// Add a new Word/File pair to the Catalog
        /// </summary>
        public bool AddWordFilePair(string wordName, DALWebCrawler.File inFile)
        {
            if (_wordColl.ContainsKey(wordName))
            {
                ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> fileCountPairDict = _fileCountPairColl[wordName];

                if (fileCountPairDict.ContainsKey(inFile.URL))
                {
                    fileCountPairDict[inFile.URL] = new WordManipulator.FileCountPair(inFile.URL, fileCountPairDict[inFile.URL].Count + 1);//increase the count
                }
                else
                {
                    fileCountPairDict.Add(inFile.URL, new WordManipulator.FileCountPair(inFile.URL, 1));
                }
            }
            else
            {
                DALWebCrawler.Word newWord = new DALWebCrawler.Word()
                {
                    WordName = wordName
                };

                AddFirstFileCountPair(wordName, inFile);

                _wordColl.Add(wordName, newWord);
            }

            return true;
        }

        private void AddFirstFileCountPair(string wordName, DALWebCrawler.File inFile)
        {
            ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> fileCountPairList = new ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>(1);
            fileCountPairList.Add(inFile.URL, new Library.WordManipulator.FileCountPair(inFile.URL, 1));
            _fileCountPairColl.Add(wordName, fileCountPairList);
        }

        // <summary>
        //  Returns all the Files which contain the searchWord
        // </summary>
        // <param name="searchWord"></param>
        // <returns></returns>
        //public System.Collections.Generic.Dictionary<File, int> Search(string searchWord)
        //{
        //    System.Collections.Generic.Dictionary<File, int> retval = null;
        //    if (_IndexOfWords.ContainsKey(searchWord))
        //    {
        //        retval = _IndexOfWords[searchWord].Files; // return the collection of File objects
        //    }
        //    return retval;
        //}

        internal void AddHtmlFile(DALWebCrawler.File htmlFile)
        {
            if (!_fileColl.ContainsKey(htmlFile.URL))
            {
                _fileColl.Add(htmlFile.URL, htmlFile);
            }
        }

        /// <summary>
        /// Adds results from one web site to the GlobalCatalog.
        /// Merging of the lists is sychronized, so GlobalCatalog is thread-safe.
        /// </summary>
        internal void MergeResultsRange(Catalog theGlobalCatalog)
        {
            //lock (((System.Collections.IDictionary)CrawlingManager.GlobalCatalog.Files).SyncRoot)
            //{
            //    lock (((System.Collections.IDictionary)_filesWithKeywords).SyncRoot)
            //    {
            foreach (string url in _fileColl.Keys)
            {
                if (!theGlobalCatalog.Files.ContainsKey(url))
                {
                    theGlobalCatalog.Files.Add(url, _fileColl[url]);
                }
            }
            //    }
            //}

            //lock (((System.Collections.IDictionary)CrawlingManager.GlobalCatalog.Words).SyncRoot)
            //{
            //    lock (((System.Collections.IDictionary)_IndexOfWords).SyncRoot)
            //    {
            foreach (string wordName in _wordColl.Keys)//local collection
            {
                if (theGlobalCatalog.Words.ContainsKey(wordName))
                {
                    ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> fileCountPairColl = _fileCountPairColl[wordName];//get all pairs for that word

                    foreach (string urlLocal in fileCountPairColl.Keys)
                    {
                        if (!theGlobalCatalog.FileCountPairs[wordName].ContainsKey(urlLocal))
                        {
                            theGlobalCatalog.FileCountPairs[wordName].Add(urlLocal, fileCountPairColl[urlLocal]);
                        }
                    }
                }
                else
                {
                    theGlobalCatalog.Words.Add(wordName, _wordColl[wordName]);

                    theGlobalCatalog.FileCountPairs.Add(wordName, _fileCountPairColl[wordName]);//add all pairs for that word because it's a new one
                }
            }
            //    }
            //}           
        }

    }
}
