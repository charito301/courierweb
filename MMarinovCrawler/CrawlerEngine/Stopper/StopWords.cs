using System;

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

        public override string StopWord(string word)
        {
            word = base.StopWord(word);

            if ((word != "") && (word.Length <= 4))
            {
                //there will be removed no matter the language of the page 
                //that is because not all pages have set language and we don't want there parasite words
                switch (word)
                {
                    case "над":
                    case "под":
                    case "това":
                    case "тук":
                    case "там":

                    case "the":
                    case "and":
                    case "that":
                    case "this":
                    case "for":
                    case "but":
                    case "with":
                    case "are":
                    case "have":
                    case "was":
                    case "out":
                    case "not":

                    case"als":
                    case "aber":
                    case "auf":
                    case "aus":
                    case "auch":
                    case "bei":
                    case "bis":
                    case "dort":
                    case "doch":
                    case "das":
                    case "des":
                    case "dem":
                    case "den":
                    case "die":
                    case "der":
                    case "ein":
                    case "für":
                    case "hier":
                    case "ist":
                    case "von":
                    case "vor":
                    case "und":
                    case "nur":
                    case "mit":
                    case "nein":
                    case "kein":
                    case"sich":
                    case "über":
                        return "";
                    default: break;
                }
                switch (Language)
                {
                    case Stemming.Languages.Bulgarian:
                        switch (word)
                        {
                            default: break;
                        }
                        break;
                    case Stemming.Languages.English:
                        switch (word)
                        {
                            default: break;
                        }
                        break;
                    case Stemming.Languages.German:
                        switch (word)
                        {
                            default: break;
                        }
                        break;
                    default: break;
                }

            }
            return word;
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
