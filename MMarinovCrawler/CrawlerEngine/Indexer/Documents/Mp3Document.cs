using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMarinov.WebCrawler.Indexer
{
    public class Mp3Document : Document
    {
        private string _wordsOnly = "";

        private string mp3Title = "";
        private string mp3Artist = "";
        private string mp3Album = "";
        private string mp3Comments = "";

        public string Mp3Title
        {
            get { return mp3Title; }
        }
        public string Mp3Artist
        {
            get { return mp3Artist; }
        }
        public string Mp3Album
        {
            get { return mp3Album; }
        }
        public string Mp3Comments
        {
            get { return mp3Comments; }
        }

        public Mp3Document(Uri location)
            : base(location)
        {
        }


        public override bool GetResponse(System.Net.HttpWebResponse webResponse)
        {
            string filename = System.IO.Path.Combine(Preferences.TempPath, (System.IO.Path.GetFileName(this.Uri.LocalPath)));

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

            HundredMilesSoftware.UltraID3Lib.UltraID3 mp3 = new HundredMilesSoftware.UltraID3Lib.UltraID3();

            try
            {
                mp3.Read(Uri.AbsoluteUri);

                mp3Title = mp3.Title;
                mp3Artist = mp3.Artist;
                mp3Album = mp3.Album;
                mp3Comments = mp3.Comments;

                this.Title = (mp3Title + " " + mp3Artist).Trim();
                this.Description = mp3Album + " " + mp3Comments;
                return !string.IsNullOrEmpty(this.Title);
            }
            catch (Exception e)
            {
                base.DocumentProgressEvent(new Report.ProgressEventArgs(e));
                return false;
            }
        }

        public override void Parse()
        {
            System.Text.StringBuilder strBldr = new System.Text.StringBuilder();

            Array.ForEach<string>(
                base.WordsStringToArray(mp3Title + " " + mp3Artist + " " + mp3Album + " " + mp3Comments),
                word => strBldr.Append(word).Append(" "));

            _wordsOnly = strBldr.ToString();
        }

        public override string WordsOnly
        {
            get
            {
                return _wordsOnly;
            }
        }
    }
}
