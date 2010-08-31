using System;

namespace MMarinov.WebCrawler.Stemming
{
    public enum Languages
    {
        None,
        English,
        German,
        Bulgarian
    }

    /// <summary>
    /// Stemmer, implementing the Porter Stemming Algorithm
    /// 
    /// The Stemmer class transforms a word into its root form.  The input
    /// word can be provided a character at time (by calling AddChar()), or at once
    /// by calling one of the various stem(something) methods.
    /// </summary>
    /// <remarks>
    /// http://www.tartarus.org/martin/PorterStemmer/
    /// http://www.tartarus.org/martin/PorterStemmer/csharp2.txt
    /// </remarks>
    public class PorterStemmer : IStemming
    {
        private char[] b;

        /// <summary>
        /// offset into b
        /// </summary>
        private int i;

        /// <summary>
        ///  offset to end of stemmed word
        /// </summary>
        private int i_end;
        private int j;
        private int k;

        /// <summary>
        /// unit of size whereby b is increased
        /// </summary>
        private static int INC = 200;

        public Languages Language = Languages.None;

        public PorterStemmer()
        {
            b = new char[INC];
            i = 0;
            i_end = 0;
        }

        /* Implementation of the .NET interface - added as part of realease 4 (Leif) */
        public string StemWord(string s)
        {
            setTerm(s);

            switch (Language)
            {
                case Languages.None:
                    return s;
                case Languages.English:
                    stemEnglishWord();
                    break;
                default: return s;
            }
            return getTerm();
        }

        /// <summary>
        /// SetTerm and GetTerm have been simply added to ease the interface with other languages. 
        /// They replace the AddChar functions and toString function. This was done because the original functions stored
        /// all stemmed words (and each time a new woprd was added, the buffer would be re-copied each time, 
        /// making it quite slow). Now, The class interface that is provided simply accepts a term and returns its stem, 
        /// instead of storing all stemmed words.
        /// </summary>
        /// <param name="s"></param>
        void setTerm(string s)
        {
            i = s.Length;
            //char[] new_b = new char[wordsCount];
            //for (int c = 0; c < wordsCount; c++)
            //    new_b[c] = s[c];

            //b = new_b;

            b = s.ToCharArray();
        }

        public string getTerm()
        {
            return new String(b, 0, i_end);
        }


        /// <summary>
        /// Add a character to the word being stemmed.  When you are finished
        /// adding characters, you can call stem(void) to stem the word.
        /// </summary>
        /// <param name="ch"></param>
        public void AddChar(char ch)
        {
            if (i == b.Length)
            {
                char[] new_b = new char[i + INC];
                for (int c = 0; c < i; c++)
                    new_b[c] = b[c];
                b = new_b;
            }
            b[i++] = ch;
        }


        /// <summary>
        /// Adds wLen characters to the word being stemmed contained in a portion of a char[] array. 
        /// This is like repeated calls of AddChar(char ch), but faster.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="wLen"></param>
        public void add(char[] w, int wLen)
        {
            if (i + wLen >= b.Length)
            {
                char[] new_b = new char[i + wLen + INC];
                for (int c = 0; c < i; c++)
                    new_b[c] = b[c];
                b = new_b;
            }
            for (int c = 0; c < wLen; c++)
                b[i++] = w[c];
        }


        /// <summary>
        /// Returns the length of the word resulting from the stemming process.
        /// </summary>
        /// <returns></returns>
        public int getResultLength()
        {
            return i_end;
        }

        /// <summary>
        /// Returns a reference to a character buffer containing the results of 
        /// the stemming process.  You also need to consult getResultLength() 
        /// to determine the length of the result.
        /// </summary>
        /// <returns></returns>
        public char[] getResultBuffer()
        {
            return b;
        }

        /* cons(wordsCount) is true <=> b[wordsCount] is a consonant. */
        private bool isConsonantEN(int i)
        {
            switch (b[i])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u': return false;
                case 'y': return (i == 0) ? true : !isConsonantEN(i - 1);
                default: return true;
            }
        }

        /// <summary>
        /// Measures the number of consonant sequences between 0 and j. if c is
        /// a consonant sequence and v a vowel sequence, and <![CDATA[<..>]]> indicates arbitrary presence,
        ///
        /// <![CDATA[<c><v>]]>      gives 0
        /// <![CDATA[<c>vc<v>]]>    gives 1
        /// <![CDATA[<c>vcvc<v>]]>  gives 2
        /// <![CDATA[<c>vcvcvc<v>]]>gives 3
        /// ....
        /// </summary>
        /// <returns></returns>
        private int ConsonantSequencesCount()
        {
            int n = 0;
            int i = 0;
            while (true)
            {
                if (i > j) return n;
                if (!isConsonantEN(i)) break; i++;
            }
            i++;
            while (true)
            {
                while (true)
                {
                    if (i > j) return n;
                    if (isConsonantEN(i)) break;
                    i++;
                }
                i++;
                n++;
                while (true)
                {
                    if (i > j) return n;
                    if (!isConsonantEN(i)) break;
                    i++;
                }
                i++;
            }
        }

        /* vowelinstem() is true <=> 0,...j contains a vowel */
        private bool vowelinstem()
        {
            int i;
            for (i = 0; i <= j; i++)
                if (!isConsonantEN(i))
                    return true;
            return false;
        }

        /// <summary>
        /// DoubleConsonant(j) is true <![CDATA[<==>]]> j,(j-1) contain a double consonant.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool DoubleConsonant(int j)
        {
            if (j < 1)
            {
                return false;
            }

            if (b[j] != b[j - 1])
            {
                return false;
            }

            return isConsonantEN(j);
        }

        /// <summary>
        /// It's true <![CDATA[<==>]]> wordsCount-2,wordsCount-1,wordsCount has the form consonant - vowel - consonant
        /// and also if the second c is not word,x or y. this is used when trying to  restore an e at the end of a short word. 
        /// e.g. cav(e), lov(e), hop(e), crim(e), but snow, box, tray.
        /// </summary>
        /// <param name="wordsCount"></param>
        /// <returns></returns>
        private bool ConsonantVowelConsonant(int wordsCount)
        {
            if (wordsCount < 2 || !isConsonantEN(wordsCount) || isConsonantEN(wordsCount - 1) || !isConsonantEN(wordsCount - 2))
                return false;
            int ch = b[wordsCount];
            if (ch == 'w' || ch == 'x' || ch == 'y')
                return false;
            return true;
        }

        private bool ends(string s)
        {
            int length = s.Length;
            int o = k - length + 1;

            if (o < 0)
            {
                return false;
            }

            char[] sc = s.ToCharArray();

            for (int i = 0; i < length; i++)
            {
                if (b[o + i] != sc[i])
                {
                    return false;
                }
            }

            j = k - length;
            return true;
        }

        /// <summary>
        /// setto(s) sets (j+1),...k to the characters in the string s, readjusting k.
        /// </summary>
        /// <param name="s"></param>
        private void setto(String s)
        {
            int l = s.Length;
            int o = j + 1;
            char[] sc = s.ToCharArray();
            for (int i = 0; i < l; i++)
            {
                b[o + i] = sc[i];
            }
            k = j + l;
        }

        /* r(s) is used further down. */
        private void r(String s)
        {
            if (ConsonantSequencesCount() > 0)
            {
                setto(s);
            }
        }

        /// <summary>
        /// gets rid of plurals and -ed or -ing. e.g.
        ///caresses  ->  caress
        ///ponies    ->  poni
        ///cats      ->  cat

        ///feed      ->  feed
        ///agreed    ->  agree
        ///disabled  ->  disable

        ///matting   ->  mat
        ///mating    ->  mate

        ///meetings  ->  meet
        /// </summary>
        private void step1RemovesPluralEdIng()
        {
            if (b[k] == 's')
            {
                if (ends("sses"))
                    k -= 2;
                else if (ends("ies"))
                    setto("i");
                else if (b[k - 1] != 's')
                    k--;
            }
            if (ends("eed"))
            {
                if (ConsonantSequencesCount() > 0)
                    k--;
            }
            else if ((ends("ed") || ends("ing")) && vowelinstem())
            {
                k = j;
                if (ends("at"))
                    setto("ate");
                else if (ends("bl"))
                    setto("ble");
                else if (ends("iz"))
                    setto("ize");
                else if (DoubleConsonant(k))
                {
                    k--;
                    int ch = b[k];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        k++;
                }
                else if (ConsonantSequencesCount() == 1 && ConsonantVowelConsonant(k)) setto("e");
            }
        }

        /* step2() turns terminal y to wordsCount when there is another vowel in the stem. */
        private void step2()
        {
            if (ends("y") && vowelinstem())
                b[k] = 'i';
        }

        /// <summary>
        ///  maps double suffices to single ones. so -ization ( = -ize plus -ation) maps to -ize etc. 
        ///  note that the string before the suffix must give ConsonantSequencesCount() > 0. 
        /// </summary>
        private void Step3MapsDoubleSuffices()
        {
            if (k == 0)
                return;

            /* For Bug 1 */
            switch (b[k - 1])
            {
                case 'a':
                    if (ends("ational")) { r("ate"); break; }
                    if (ends("tional")) { r("tion"); break; }
                    break;
                case 'c':
                    if (ends("enci")) { r("ence"); break; }
                    if (ends("anci")) { r("ance"); break; }
                    break;
                case 'e':
                    if (ends("izer")) { r("ize"); break; }
                    break;
                case 'l':
                    if (ends("bli")) { r("ble"); break; }
                    if (ends("alli")) { r("al"); break; }
                    if (ends("entli")) { r("ent"); break; }
                    if (ends("eli")) { r("e"); break; }
                    if (ends("ousli")) { r("ous"); break; }
                    break;
                case 'o':
                    if (ends("ization")) { r("ize"); break; }
                    if (ends("ation")) { r("ate"); break; }
                    if (ends("ator")) { r("ate"); break; }
                    break;
                case 's':
                    if (ends("alism")) { r("al"); break; }
                    if (ends("iveness")) { r("ive"); break; }
                    if (ends("fulness")) { r("ful"); break; }
                    if (ends("ousness")) { r("ous"); break; }
                    break;
                case 't':
                    if (ends("aliti")) { r("al"); break; }
                    if (ends("iviti")) { r("ive"); break; }
                    if (ends("biliti")) { r("ble"); break; }
                    break;
                case 'g':
                    if (ends("logi")) { r("log"); break; }
                    break;
                default:
                    break;
            }
        }

        /* step4() deals with -ic-, -full, -ness etc. similar strategy to Step3MapsDoubleSuffices. */
        private void step4()
        {
            switch (b[k])
            {
                case 'e':
                    if (ends("icate")) { r("ic"); break; }
                    if (ends("ative")) { r(""); break; }
                    if (ends("alize")) { r("al"); break; }
                    break;
                case 'i':
                    if (ends("iciti")) { r("ic"); break; }
                    break;
                case 'l':
                    if (ends("ical")) { r("ic"); break; }
                    if (ends("ful")) { r(""); break; }
                    break;
                case 's':
                    if (ends("ness")) { r(""); break; }
                    break;
            }
        }

        /* step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */
        private void step5()
        {
            if (k == 0)
                return;

            /* for Bug 1 */
            switch (b[k - 1])
            {
                case 'a':
                    if (ends("al")) break; return;
                case 'c':
                    if (ends("ance")) break;
                    if (ends("ence")) break; return;
                case 'e':
                    if (ends("er")) break; return;
                case 'i':
                    if (ends("ic")) break; return;
                case 'l':
                    if (ends("able")) break;
                    if (ends("ible")) break; return;
                case 'n':
                    if (ends("ant")) break;
                    if (ends("ement")) break;
                    if (ends("ment")) break;
                    /* element etc. not stripped before the ConsonantSequencesCount */
                    if (ends("ent")) break; return;
                case 'o':
                    if (ends("ion") && j >= 0 && (b[j] == 's' || b[j] == 't')) break;
                    /* j >= 0 fixes Bug 2 */
                    if (ends("ou")) break; return;
                /* takes care of -ous */
                case 's':
                    if (ends("ism")) break; return;
                case 't':
                    if (ends("ate")) break;
                    if (ends("iti")) break; return;
                case 'u':
                    if (ends("ous")) break; return;
                case 'v':
                    if (ends("ive")) break; return;
                case 'z':
                    if (ends("ize")) break; return;
                default:
                    return;
            }
            if (ConsonantSequencesCount() > 1)
                k = j;
        }

        /* step6() removes a final -e if ConsonantSequencesCount() > 1. */
        private void step6()
        {
            j = k;

            if (b[k] == 'e')
            {
                int a = ConsonantSequencesCount();
                if (a > 1 || a == 1 && !ConsonantVowelConsonant(k - 1))
                    k--;
            }
            if (b[k] == 'l' && DoubleConsonant(k) && ConsonantSequencesCount() > 1)
                k--;
        }

        /// <summary>
        /// Stem the word placed into the Stemmer buffer through calls to AddChar(). Returns true 
        /// if the stemming process resulted in a word different from the input.  
        /// </summary>
        public void stemEnglishWord()
        {
            k = i - 1;
            if (k > 1)
            {
                step1RemovesPluralEdIng();
                step2();
                Step3MapsDoubleSuffices();
                step4();
                step5();
                step6();
            }
            i_end = k + 1;
            i = 0;
        }
    }
}