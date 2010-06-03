using System;
using System.Collections.Generic;
using System.Text;

namespace MMarinov.WebCrawler.Indexer
{
    /// <summary>
    /// ASCII Text Document (text/plain)
    /// </summary>
    public class TextDocument : Document
    {
        private string _All;
        private string _WordsOnly;

        public override string WordsOnly
        {
            get { return _WordsOnly; }
        }
        public override string[] WordsArray
        {
            get { return base.WordsStringToArray(WordsOnly); }
        }
        /// <summary>
        /// Set 'all' and 'words only' to the same value (no parsing)
        /// </summary>
        public override string AllCode
        {
            get { return _All; }
            set
            {
                _All = value;
                _WordsOnly = value;
            }
        }

        #region Constructor requires Uri
        public TextDocument(Uri location)
            : base(location)
        { }

        #endregion

        public override void Parse()
        {
            _WordsOnly = _All;

        }

        public override bool GetResponse(System.Net.HttpWebResponse webResponse)
        {
            System.IO.StreamReader stream = null;

            try
            {
                stream = new System.IO.StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.ASCII);
                {
                    if (webResponse.ResponseUri != this.Uri)
                    {
                        this.Uri = webResponse.ResponseUri; // we *may* have been redirected... and we want the *final* URL

                        base.AddURLtoGlobalVisited(this.Uri);
                    }

                    _All = stream.ReadToEnd();
                    this.Title = System.IO.Path.GetFileNameWithoutExtension(this.Uri.AbsoluteUri);

                    return true;
                }
            }
            catch (Exception e)
            {
                base.DocumentProgressEvent(new Report.ProgressEventArgs(new Exception(Uri.AbsoluteUri, e)));
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
    }
}
