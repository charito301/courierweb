using System;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace MMarinov.WebCrawler.Indexer
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
        private ThreadedGenerics.TDictionary<string, Word> _IndexOfWords = new ThreadedGenerics.TDictionary<string, Word>();
        /// <summary>
        /// File collection that will keep link-keywords info
        /// </summary>
        private ThreadedGenerics.TDictionary<string, File> _filesWithKeywords = new ThreadedGenerics.TDictionary<string, File>();

        /// <summary>
        /// Words in the Catalog
        /// </summary>
        /// <remarks>
        /// Added property to allow Serialization to disk
        /// </remarks>
        public ThreadedGenerics.TDictionary<string, Word> Words
        {
            get
            {
                return _IndexOfWords;
            }
        }

        /// <summary>
        /// File collection that will keep link-keywords info
        /// </summary>
        public ThreadedGenerics.TDictionary<string, File> Files
        {
            get
            {
                return _filesWithKeywords;
            }
        }

        /// <summary>
        /// Number of Words in the Catalog
        /// </summary>
        public int Length
        {
            get { return _IndexOfWords.Count; }
        }

        /// <summary>
        /// Constructor - creates the Hashtable for internal data storage.
        /// </summary>
        public Catalog()
        {
            _IndexOfWords = new ThreadedGenerics.TDictionary<string, Word>();
            _filesWithKeywords = new ThreadedGenerics.TDictionary<string, File>();
        }

        /// <summary>
        /// Add a new Word/File pair to the Catalog
        /// </summary>
        public bool AddWordFilePair(string word, File inFile, int position)
        {
            if (word == null)
            {//debug me
            }
            // ### Make sure the Word object is in the index ONCE only
            if (_IndexOfWords.ContainsKey(word))
            {
                _IndexOfWords[word].Add(inFile, position);	// add this file reference to the Word
            }
            else
            {
                _IndexOfWords.Add(word, new Word(word, inFile, position));// create a new Word object
            }
            return true;
        }

        /// <summary>
        ///  Returns all the Files which contain the searchWord
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        public System.Collections.Generic.Dictionary<File, int> Search(string searchWord)
        {
            System.Collections.Generic.Dictionary<File, int> retval = null;
            if (_IndexOfWords.ContainsKey(searchWord))
            {
                retval = _IndexOfWords[searchWord].Files; // return the collection of File objects
            }
            return retval;
        }

        internal void AddHtmlFile(File htmlFile)
        {
            if (!_filesWithKeywords.ContainsKey(htmlFile.Url))
            {
                _filesWithKeywords.Add(htmlFile.Url, htmlFile);
            }
        }

        /// <summary>
        /// Adds results from one web site to the GlobalCatalog.
        /// Merging of the lists is sychronized, so GlobalCatalog is thread-safe.
        /// </summary>
        internal void MergeResultsRange()
        {
            bool containsLink = false;

            //lock (((System.Collections.IDictionary)CrawlingManager.GlobalCatalog.Words).SyncRoot)
            //{
            //    lock (((System.Collections.IDictionary)_IndexOfWords).SyncRoot)
            //    {
            foreach (System.Collections.Generic.KeyValuePair<string, Word> stringWord in _IndexOfWords)
            {
                if (MMarinov.WebCrawler.Indexer.CrawlingManager.GlobalCatalog.Words.ContainsKey(stringWord.Key))
                {
                    //check values - links to add ..
                    foreach (System.Collections.Generic.KeyValuePair<File, int> fileHtml in stringWord.Value.Files)//foreach link that contains the word
                    {
                        containsLink = false;

                        foreach (File fileHtmlGlobal in MMarinov.WebCrawler.Indexer.CrawlingManager.GlobalCatalog.Words[stringWord.Key].Files.Keys)
                        {
                            if (fileHtmlGlobal.Url == fileHtml.Key.Url)
                            {
                                containsLink = true;
                                break;
                            }
                        }

                        if (!containsLink)
                        {
                            MMarinov.WebCrawler.Indexer.CrawlingManager.GlobalCatalog.Words[stringWord.Key].Files.Add(fileHtml.Key, fileHtml.Value);
                        }
                    }
                }
                else
                {
                    MMarinov.WebCrawler.Indexer.CrawlingManager.GlobalCatalog.Words.Add(stringWord.Key, stringWord.Value);
                }
            }
            //    }
            //}

            //lock (((System.Collections.IDictionary)CrawlingManager.GlobalCatalog.Files).SyncRoot)
            //{
            //    lock (((System.Collections.IDictionary)_filesWithKeywords).SyncRoot)
            //    {
            foreach (System.Collections.Generic.KeyValuePair<string, File> stringFile in _filesWithKeywords)
            {
                if (!MMarinov.WebCrawler.Indexer.CrawlingManager.GlobalCatalog.Files.ContainsKey(stringFile.Key))
                {
                    MMarinov.WebCrawler.Indexer.CrawlingManager.GlobalCatalog.Files.Add(stringFile.Key, stringFile.Value);
                }
            }
            //    }
            //}
        }

        /// <summary>
        /// Saves all web pages of a web site into DB
        /// </summary>
        internal void SaveWebSiteFilesToDB()
        {
            //throw new NotImplementedException();
        }
    }
}
