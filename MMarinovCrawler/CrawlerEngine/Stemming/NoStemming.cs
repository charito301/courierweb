using System;

namespace MMarinov.WebCrawler.Stemming
{
    public class NoStemming : IStemming
    {
        public string StemWord(string word)
        {
            return word;
        }
    }
}