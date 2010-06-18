using System;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Linq;

namespace MMarinov.WebCrawler.Library
{
    /// <summary>
    /// Catalog of Words (and Files)
    /// <summary>
    [Serializable]
    public class Catalog
    {
        private static object syncAccessToDB = new object();

        private ThreadedGenerics.TDictionary<string, DALWebCrawler.Word> _wordColl;
        private ThreadedGenerics.TDictionary<string, DALWebCrawler.File> _fileColl;

        private System.Collections.Generic.List<DALWebCrawler.File> filesToSaveIntoDB;
        private System.Collections.Generic.List<DALWebCrawler.Word> wordsToSaveIntoDB;
        private System.Collections.Generic.Dictionary<DALWebCrawler.Word, System.Collections.Generic.List<WordManipulator.FileCountPair>> wordsInFilesToSaveIntoDB;

        /// <summary>
        /// Key(wordName), 
        /// Value(Hashtable:Key(URL)
        ///                 Value(FileCountPair))
        /// </summary>
        private ThreadedGenerics.TDictionary<string, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>> _fileCountPairColl;

        #region Public properties

        public ThreadedGenerics.TDictionary<string, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>> FileCountPairs
        {
            get { return _fileCountPairColl; }
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
            _fileCountPairColl = new MMarinov.ThreadedGenerics.TDictionary<string, ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>>();

            filesToSaveIntoDB = new System.Collections.Generic.List<DALWebCrawler.File>();
            wordsToSaveIntoDB = new System.Collections.Generic.List<DALWebCrawler.Word>();
            wordsInFilesToSaveIntoDB = new System.Collections.Generic.Dictionary<DALWebCrawler.Word, System.Collections.Generic.List<WordManipulator.FileCountPair>>();
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
                    fileCountPairDict[inFile.URL] = new WordManipulator.FileCountPair(inFile, fileCountPairDict[inFile.URL].Count + 1);//increase the count
                }
                else
                {
                    fileCountPairDict.Add(inFile.URL, new WordManipulator.FileCountPair(inFile, 1));
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
            fileCountPairList.Add(inFile.URL, new Library.WordManipulator.FileCountPair(inFile, 1));
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

        #region Merge with GlobalCatalog

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
            MergeFilesWithGlobalCatalog(theGlobalCatalog);
            //    }
            //}

            //lock (((System.Collections.IDictionary)CrawlingManager.GlobalCatalog.Words).SyncRoot)
            //{
            //    lock (((System.Collections.IDictionary)_IndexOfWords).SyncRoot)
            //    {
            MergeWordsAndPairsWithGlobalCatalog(theGlobalCatalog);
            //    }
            //}           
        }

        private void MergeWordsAndPairsWithGlobalCatalog(Catalog theGlobalCatalog)
        {
            foreach (string wordName in _wordColl.Keys)//local collection
            {
                if (theGlobalCatalog.Words.ContainsKey(wordName))
                {
                    UpdateWordsAndFileToGlobalCatalog(theGlobalCatalog, wordName);
                }
                else
                {
                    InsertNewWordsAndFileToGlobalCatalog(theGlobalCatalog, wordName);
                }
            }
        }

        private void MergeFilesWithGlobalCatalog(Catalog theGlobalCatalog)
        {
            foreach (string url in _fileColl.Keys)
            {
                if (!theGlobalCatalog.Files.ContainsKey(url))
                {
                    theGlobalCatalog.Files.Add(url, _fileColl[url]);
                    filesToSaveIntoDB.Add(_fileColl[url]);
                }
            }
        }

        private void UpdateWordsAndFileToGlobalCatalog(Catalog theGlobalCatalog, string wordName)
        {
            ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> fileCountPairColl = _fileCountPairColl[wordName];//get all pairs for that word

            foreach (string urlLocal in fileCountPairColl.Keys)
            {
                if (!theGlobalCatalog.FileCountPairs[wordName].ContainsKey(urlLocal))
                {
                    theGlobalCatalog.FileCountPairs[wordName].Add(urlLocal, fileCountPairColl[urlLocal]);
                    if (wordsInFilesToSaveIntoDB.ContainsKey(_wordColl[wordName]))
                    {
                        wordsInFilesToSaveIntoDB[_wordColl[wordName]].Add(fileCountPairColl[urlLocal]);
                    }
                    else
                    {
                        wordsInFilesToSaveIntoDB.Add(_wordColl[wordName], new System.Collections.Generic.List<WordManipulator.FileCountPair>() { fileCountPairColl[urlLocal] });
                    }
                }
            }
        }

        private void InsertNewWordsAndFileToGlobalCatalog(Catalog theGlobalCatalog, string wordName)
        {
            theGlobalCatalog.Words.Add(wordName, _wordColl[wordName]);
            wordsToSaveIntoDB.Add(_wordColl[wordName]);

            theGlobalCatalog.FileCountPairs.Add(wordName, _fileCountPairColl[wordName]);//add all pairs for that word because it's a new one
            wordsInFilesToSaveIntoDB.Add(_wordColl[wordName], _fileCountPairColl[wordName].Values.ToList<WordManipulator.FileCountPair>());
        }

        #endregion

        internal void SaveResultsToDB()
        {
            lock (syncAccessToDB)
            {
                if (filesToSaveIntoDB.Count > 0)
                {
                    FileManipulator.InsertFilesIntoDB(filesToSaveIntoDB);
                }

                if (wordsToSaveIntoDB.Count > 0)
                {
                    WordManipulator.InsertWordsIntoDB(wordsToSaveIntoDB);
                }

                if (wordsInFilesToSaveIntoDB.Count > 0)
                {
                    System.Collections.Generic.List<DALWebCrawler.WordsInFile> wordsInFileList = new System.Collections.Generic.List<DALWebCrawler.WordsInFile>();

                    foreach (DALWebCrawler.Word word in wordsInFilesToSaveIntoDB.Keys)
                    {
                        wordsInFilesToSaveIntoDB[word].ForEach(fileCountPair =>
                             wordsInFileList.Add(new DALWebCrawler.WordsInFile()
                             {
                                 WordID = word.ID,
                                 FileID = fileCountPair.File.ID,
                                 Count = fileCountPair.Count
                             })
                        );
                    }

                    if (wordsInFileList.Count > 0)
                    {
                        WordManipulator.InsertWordsInFilesIntoDB(wordsInFileList);
                    }
                }
            }
        }
    }
}
