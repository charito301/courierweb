using System;

namespace MMarinov.WebCrawler.Indexer
{
    public class PdfDocument : Document
    {
        private string _All;
        private string _WordsOnly;

        public PdfDocument(Uri location)
            : base(location)
        { }

        /// <summary>
        /// Set 'all' and 'words only' to the same value (no parsing)
        /// </summary>
        public override string AllCode
        {
            get { return _All; }
            set
            {
                _All = value;
                _WordsOnly = _All;
            }
        }

        public override string WordsOnly
        {
            get { return _WordsOnly; }
        }

        public override string[] WordsArray
        {
            get { return base.WordsStringToArray(WordsOnly); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Parse()
        {
            // no parsing (for now). perhaps in future we can regex look for urls (www.xxx.com) and try to link to them...

            System.Text.RegularExpressions.MatchCollection matchLinks = System.Text.RegularExpressions.Regex.Matches(_All, @"http(s)?://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&amp;\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.MatchCollection matchLinks2 = System.Text.RegularExpressions.Regex.Matches(_All, "(http|https)://([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public override bool GetResponse(System.Net.HttpWebResponse webResponse)
        {
            string filename = System.IO.Path.Combine(Preferences.TempPath, (System.IO.Path.GetFileName(this.Uri.LocalPath)));
            this.Title = System.IO.Path.GetFileNameWithoutExtension(filename);
            System.IO.BinaryReader binaryReader = null;
            System.IO.FileStream iofilestream = null;

            try
            {
                binaryReader = new System.IO.BinaryReader(webResponse.GetResponseStream());
                iofilestream = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                const int BUFFER_SIZE = 8192;
                byte[] buf = new byte[BUFFER_SIZE];
                int n = binaryReader.Read(buf, 0, BUFFER_SIZE);

                while (n > 0)
                {
                    iofilestream.Write(buf, 0, n);
                    n = binaryReader.Read(buf, 0, BUFFER_SIZE);
                }

                if (webResponse.ResponseUri != this.Uri)
                {
                    this.Uri = webResponse.ResponseUri; // we *may* have been redirected... and we want the *final* URL

                    base.AddURLtoGlobalVisited(this.Uri);
                }

                _All = ParsePDF(filename);
            }
            catch (Exception e)
            {
                base.DocumentProgressEvent(new Report.ProgressEventArgs(new Exception(Uri.AbsoluteUri, e)));
                return false;
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Close();
                }

                if (iofilestream != null)
                {
                    iofilestream.Close();
                    iofilestream.Dispose();
                }
            }

            return !string.IsNullOrEmpty(this.AllCode);
        }

        private static string ParsePDF(string input)
        {
            org.pdfbox.pdmodel.PDDocument doc = org.pdfbox.pdmodel.PDDocument.load(input);
            return new org.pdfbox.util.PDFTextStripper().getText(doc);
        }
    }
}