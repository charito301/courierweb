using System;
using System.Linq;

namespace MMarinov.WebCrawler.Library
{
    /// <summary>
    /// Catalog of Words (and Files)
    /// <summary>
    [Serializable]
    public class WebsiteCatalog
    {
        #region private properties

        private static ThreadedGenerics.TList<string> _globalFilesList = new MMarinov.ThreadedGenerics.TList<string>();
        private static ThreadedGenerics.TList<string> _globalWordsList = new MMarinov.ThreadedGenerics.TList<string>();
        private static ThreadedGenerics.TDictionary<string, ThreadedGenerics.TList<string>> _globalWordFilePairsList = new ThreadedGenerics.TDictionary<string, MMarinov.ThreadedGenerics.TList<string>>();

        private ThreadedGenerics.TDictionary<string, DALWebCrawler.Word> _wordColl;
        private ThreadedGenerics.TDictionary<string, DALWebCrawler.File> _fileColl;
        private ThreadedGenerics.TDictionary<string, WordManipulator.WordFilesPair> _wordFilesPairHT;

        private ThreadedGenerics.TList<DALWebCrawler.File> _filesToSaveIntoDB;
        private ThreadedGenerics.TList<DALWebCrawler.Word> _wordsToSaveIntoDB;
        private ThreadedGenerics.TDictionary<DALWebCrawler.Word, ThreadedGenerics.TList<WordManipulator.FileCountPair>> _wordsInFilesToSaveIntoDB;

        #endregion

        #region Public properties

        public int GlobalWordsListCount
        {
            get { return _globalWordsList.Count; }
        }

        public int GlobalFilesListCount
        {
            get { return _globalFilesList.Count; }
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
        public WebsiteCatalog()
        {
            _wordColl = new ThreadedGenerics.TDictionary<string, DALWebCrawler.Word>();
            _fileColl = new ThreadedGenerics.TDictionary<string, DALWebCrawler.File>();
            _wordFilesPairHT = new MMarinov.ThreadedGenerics.TDictionary<string, WordManipulator.WordFilesPair>();

            _filesToSaveIntoDB = new ThreadedGenerics.TList<DALWebCrawler.File>();
            _wordsToSaveIntoDB = new ThreadedGenerics.TList<DALWebCrawler.Word>();
            _wordsInFilesToSaveIntoDB = new ThreadedGenerics.TDictionary<DALWebCrawler.Word, ThreadedGenerics.TList<WordManipulator.FileCountPair>>();
        }

        #region Add results from spiders

        /// <summary>
        /// Add a new Word/File pair to the Catalog
        /// </summary>
        public bool AddWordFilePair(string wordName, DALWebCrawler.File inFile)
        {
            if (_wordColl.Keys.Contains(wordName))
            {
                WordManipulator.WordFilesPair wordFilesPair = _wordFilesPairHT[wordName];

                if (wordFilesPair.WordFileCountHT.ContainsKey(inFile.URL))
                {
                    wordFilesPair.WordFileCountHT[inFile.URL] = new WordManipulator.FileCountPair(inFile, wordFilesPair.WordFileCountHT[inFile.URL].Count + 1);//increase the count
                }
                else
                {
                    wordFilesPair.WordFileCountHT.Add(inFile.URL, new WordManipulator.FileCountPair(inFile, 1));
                }
            }
            else
            {
                DALWebCrawler.Word newWord = new DALWebCrawler.Word() { WordName = wordName };

                AddFirstFileCountPair(newWord, inFile);

                _wordColl.Add(wordName, newWord);
            }

            return true;
        }

        private void AddFirstFileCountPair(DALWebCrawler.Word newWord, DALWebCrawler.File inFile)
        {
            ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> fileCountPairList = new ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair>(1);
            fileCountPairList.Add(inFile.URL, new Library.WordManipulator.FileCountPair(inFile, 1));
            _wordFilesPairHT.Add(newWord.WordName, new WordManipulator.WordFilesPair(newWord, fileCountPairList));
        }

        internal void AddHtmlFile(DALWebCrawler.File htmlFile)
        {
            if (!_fileColl.ContainsKey(htmlFile.URL))
            {
                _fileColl.Add(htmlFile.URL, htmlFile);
            }
        }
        #endregion

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

        #region Merge with GlobalCatalog

        /// <summary>
        /// Adds results from one web site to the GlobalCatalog.
        /// Merging of the lists is sychronized, so GlobalCatalog is thread-safe.
        /// </summary>
        internal void MergeResultsRange()
        {
            lock (_globalFilesList)
            {
                MergeFilesWithGlobalCatalog();
            }

            lock (_globalWordsList)
            {
                lock (_globalWordFilePairsList)
                {
                    MergeWordsAndPairsWithGlobalCatalog();
                }
            }
        }

        private void MergeWordsAndPairsWithGlobalCatalog()
        {
            foreach (string wordName in _wordColl.Keys)
            {
                if (_globalWordsList.Contains(wordName))
                {
                    UpdateWordsAndPairsToGlobalCatalog(wordName);
                }
                else
                {
                    InsertWordsAndPairsToGlobalCatalog(wordName);
                }
            }
        }

        private void MergeFilesWithGlobalCatalog()
        {
            foreach (string url in _fileColl.Keys)
            {
                if (!_globalFilesList.Contains(url))
                {
                    _globalFilesList.Add(url);
                    _filesToSaveIntoDB.Add(_fileColl[url]);
                }
            }
        }

        private void UpdateWordsAndPairsToGlobalCatalog(string wordName)
        {
            ThreadedGenerics.TDictionary<string, WordManipulator.FileCountPair> fileCountPairsHT = _wordFilesPairHT[wordName].WordFileCountHT;//get all pairs for that word

            foreach (string urlLocal in fileCountPairsHT.Keys)
            {
                if (!_globalWordFilePairsList[wordName].Contains(urlLocal))
                {
                    _globalWordFilePairsList[wordName].Add(urlLocal);

                    DALWebCrawler.Word word = _wordsInFilesToSaveIntoDB.Keys.FirstOrDefault(w => w.WordName == wordName);
                    if (word != null)
                    {
                        _wordsInFilesToSaveIntoDB[word].Add(fileCountPairsHT[urlLocal]);//to save into db coll
                    }
                    else
                    {
                        _wordsInFilesToSaveIntoDB.Add(_wordFilesPairHT[wordName].Word, new ThreadedGenerics.TList<WordManipulator.FileCountPair>() { fileCountPairsHT[urlLocal] });
                    }
                }
            }
        }

        private void InsertWordsAndPairsToGlobalCatalog(string wordName)
        {
            DALWebCrawler.Word newWord = new DALWebCrawler.Word() { WordName = wordName };

            _globalWordsList.Add(wordName);
            _wordsToSaveIntoDB.Add(newWord);

            _globalWordFilePairsList.Add(wordName, new ThreadedGenerics.TList<string>(_wordFilesPairHT[wordName].WordFileCountHT.Keys));//AddChar all pairs for that word because it's a new one
            _wordsInFilesToSaveIntoDB.Add(newWord, new ThreadedGenerics.TList<WordManipulator.FileCountPair>(_wordFilesPairHT[wordName].WordFileCountHT.Values));
        }

        #endregion

        internal void SaveResultsToDB()
        {
            lock (_filesToSaveIntoDB)
            {
                if (_filesToSaveIntoDB.Count > 0)
                {
                    FileManipulator.InsertFilesIntoDB(_filesToSaveIntoDB);
                }
            }

            lock (_globalWordsList)
            {
                lock (_globalWordFilePairsList)
                {
                    if (_wordsToSaveIntoDB.Count > 0)
                    {
                        WordManipulator.InsertWordsIntoDB(_wordsToSaveIntoDB);
                    }

                    if (_wordsInFilesToSaveIntoDB.Count > 0)
                    {
                        System.Collections.Generic.List<DALWebCrawler.WordsInFile> wordsInFileList = new System.Collections.Generic.List<DALWebCrawler.WordsInFile>();

                        foreach (DALWebCrawler.Word word in _wordsInFilesToSaveIntoDB.Keys)
                        {
                            if (word.ID == 0)
                            {
                                continue;
                            }

                            foreach (WordManipulator.FileCountPair fcp in _wordsInFilesToSaveIntoDB[word])
                            {
                                if (fcp.File.ID == 0)
                                {
                                    continue;
                                }

                                wordsInFileList.Add(new DALWebCrawler.WordsInFile()
                               {
                                   WordID = word.ID,
                                   FileID = fcp.File.ID,
                                   Count = fcp.Count
                               });
                            }
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
}
