using System;
using System.Linq;

namespace MMarinov.WebCrawler.Stopper
{
    public enum StoppingModes
    {
        Off,
        Short,
        List
    }

    public class NoStopping : IStopper
    {
        /// <summary>
        /// Basic 'noop' implementation
        /// </summary>
        /// <param name="word">Word to check against the Stop list</param>
        /// <returns>The input word is always returned unchanged</returns>
        public virtual string StopWord(string word)
        {
            return word;
        }
    }
    /// <summary>
    /// The most basic 'stop word' processor, just ignores
    /// any and ALL words of 1 or 2 chars or longer than 50.
    /// </summary>
    /// <remarks>
    /// Examples of words ignored:
    /// a of we in or wordsCount to 
    /// </remarks>
    public class ShortStopper : IStopper
    {
        public virtual string StopWord(string word)
        {
            if (word.Length <= 2 || word.Length > 50)
            {
                return "";
            }
            else
            {
                return word;
            }
        }
    }
    /// <summary>
    /// List of Stop words in a switch statement; 
    /// Note: it only checks words that are 3 or 4 characters long,
    /// as the base() method already excludes 1 and 2 char words.
    /// </summary>
    /// <remarks>
    /// Examples of words ignored:
    /// the and that you this for but with are have was out not
    /// </remarks>
    public class ListStopper : ShortStopper
    {
        public Stemming.Languages Language = Stemming.Languages.None;
        private const string _csvENFilename = "StopWordsEN.csv";
        private const string _csvDEFilename = "StopWordsDE.csv";
        private const string _csvBGFilename = "StopWordsBG.csv";
        private static System.Collections.Generic.List<string> _stopWordsListEN = new System.Collections.Generic.List<string>();
        private static System.Collections.Generic.List<string> _stopWordsListDE = new System.Collections.Generic.List<string>();
        private static System.Collections.Generic.List<string> _stopWordsListBG = new System.Collections.Generic.List<string>();

        public override string StopWord(string word)
        {
            word = base.StopWord(word);

            if ((word != "") && (word.Length <= 4))
            {
                //there will be removed no matter the language of the page 
                //that is because not all pages have set language and we don't want there parasite words
                switch (word)
                {                  
                    case "http":
                    case "com":
                    case "www":
                    case "https":

                    case "над":
                    case "под":
                    case "това":
                    case "тук":
                    case "там":

                        return "";
                    default: break;
                }

                if (_stopWordsListEN.Contains(word) || _stopWordsListDE.Contains(word) || _stopWordsListBG.Contains(word))
                {
                    return "";
                }

                //switch (Language)
                //{
                //    case Stemming.Languages.Bulgarian:
                //        switch (word)
                //        {
                //            default: break;
                //        }
                //        break;
                //    case Stemming.Languages.English:
                //        switch (word)
                //        {
                //            default: break;
                //        }
                //        break;
                //    case Stemming.Languages.German:
                //        switch (word)
                //        {
                //            default: break;
                //        }
                //        break;
                //    default: break;
                //}

            }
            return word;
        }

        internal static void LoadStopLists()
        {
            try
            {
                using (System.IO.StreamReader readFile = new System.IO.StreamReader(Preferences.WorkingPath + "\\" + _csvENFilename))
                {
                    string[] stopWordsEN = readFile.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    _stopWordsListEN = stopWordsEN.ToList<string>();
                }
            }
            catch { }

            try
            {
                using (System.IO.StreamReader readFile = new System.IO.StreamReader(Preferences.WorkingPath + "\\" + _csvDEFilename))
                {
                    string[] stopWordsDE = readFile.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    _stopWordsListDE = stopWordsDE.ToList<string>();
                }
            }
            catch { }
            
            //try
            //{
            //    using (System.IO.StreamReader readFile = new System.IO.StreamReader(Preferences.WorkingPath + "\\" + _csvBGFilename))
            //    {
            //        string[] stopWordsBG = readFile.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //        _stopWordsListBG = stopWordsBG.ToList<string>();
            //    }
            //}
            //catch { }
        }
    }

    /// <summary>
    /// TODO: implement a Stopper that will read in the word list
    /// from a file. 
    /// http://snowball.tartarus.org/algorithms/english/stop.txt
    /// </summary>
    public class FileStopper : IStopper
    {
        /// <summary>
        /// Because this method will use an intelligent list to filter
        /// out stop words, it probably won't need to inherit from the
        /// dodgy implementations above.
        /// </summary>
        public string StopWord(string word)
        {
            throw new NotImplementedException();
        }
    }
}
