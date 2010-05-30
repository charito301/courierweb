using System;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace MMarinov.WebCrawler.Indexer
{
    [Serializable]
    public class Word
    {
        #region Private fields: _Text, _FileCollection

        /// <summary>Collection of files the word appears in</summary>
        private System.Collections.Generic.Dictionary<File, int> _FileCollection = new System.Collections.Generic.Dictionary<File, int>();

        /// <summary>The word itself</summary>
        private string _Text;

        #endregion

        /// <summary>
        /// The cataloged word
        /// </summary>
        public string Text
        {
            get { return _Text; }
        }

        /// <summary>
        /// Files that this Word appears in
        /// </summary>
        public System.Collections.Generic.Dictionary<File, int> Files
        {
            get
            {
                return _FileCollection;
            }
        }

        /// <summary>
        /// Empty constructor required for serialization
        /// </summary>
        public Word() { }

        /// <summary>Constructor with first file reference</summary>
        public Word(string text, File infile, int position)
        {
            _Text = text;
            //WordInFile thefile = new WordInFile(filename, position);
            _FileCollection.Add(infile, 1);
        }

        /// <summary>Add a file referencing this word</summary>
        public void Add(File infile, int position)
        {
            if (_FileCollection.ContainsKey(infile))
            {
                _FileCollection[infile] = _FileCollection[infile] + 1; //thefile.Add (position);
            }
            else
            {
                //WordInFile thefile = new WordInFile(filename, position);
                _FileCollection.Add(infile, 1);
            }
        }
    }  
}
