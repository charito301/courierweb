using System;

namespace MMarinov.WebCrawler.Indexer
{
    public abstract class Document
    {
        public enum DocumentTypes
        {
            HTML = 1,
            Text = 2,
            Mp3 = 3,
            PDF = 4
        }

        private Uri _Uri;
        private string _allCode;
        private string _ContentType;
        private string _MimeType = "";
        private string _Title;
        private string _Description = "";
        private byte _fileType = 0;
        private string _keywords = "";

        public static Int64 FoundValidLinks = 0;
        public static Int64 FoundTotalLinks = 0;

        public static double DownloadSpeed = 0;

        protected void SetDownloadSpeed(double speed)
        {
            DownloadSpeed = speed;
        }

        private System.Collections.Generic.List<string> _localLinks;
        private System.Collections.Generic.List<string> _externalLinks;

        public abstract bool GetResponse(System.Net.HttpWebResponse webresponse);
        public abstract void Parse();

        public delegate void DocumentProgressEventHandler(Report.ProgressEventArgs pea);
        public event DocumentProgressEventHandler DocumentEvent;

        protected void DocumentProgressEvent(Report.ProgressEventArgs pea)
        {
            if (this.DocumentEvent != null)
            {
                DocumentEvent(pea);
            }
        }

        public System.Collections.Generic.List<string> LocalLinks
        {
            get
            {
                return _localLinks;
            }
            set
            {
                _localLinks = value;
            }
        }
        public System.Collections.Generic.List<string> ExternalLinks
        {
            get
            {
                return _externalLinks;
            }
            set
            {
                _externalLinks = value;
            }
        }

        public virtual string AllCode
        {
            get { return _allCode; }
            set { _allCode = value; }
        }

        public virtual string MimeType
        {
            get { return _MimeType; }
            set { _MimeType = value; }
        }

        public virtual string Keywords
        {
            get { return _keywords; }
            set { _keywords = value; }
        }

        public virtual byte FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
        }

        public abstract string WordsOnly { get; }

        public virtual string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// http://www.ietf.org/rfc/rfc2396.txt
        /// </summary>
        public virtual Uri Uri
        {
            get { return _Uri; }
            set { _Uri = value; }
        }

        public virtual string[] WordsArray
        {
            get { return this.WordsStringToArray(WordsOnly); }
        }

        /// <summary>
        /// Most document types don't have embedded robot information
        /// so they'll always be allowed to be followed 
        /// (assuming there are links to follow)
        /// </summary>
        public virtual bool RobotFollowOK
        {
            get { return true; }
        }
        /// <summary>
        /// Most document types don't have embedded robot information
        /// so they'll always be allowed to be indexed 
        /// (assuming there is content to index)
        /// </summary>
        public virtual bool RobotIndexOK
        {
            get { return true; }
        }

        /// <summary>
        /// Constructor for any document requires the Uri be specified
        /// </summary>
        public Document(Uri uri)
        {
            _Uri = uri;
        }

        /// <summary>
        /// Constructor for any document requires the Uri and MimeType to be specified
        /// </summary>
        public Document(Uri uri, string mimeType)
        {
            _Uri = uri;
            _MimeType = mimeType;
        }

        /// <summary>
        /// COMPRESS ALL WHITESPACE into a single space, seperating words
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        protected string[] WordsStringToArray(string words)
        {
            if (words.Length > 0)
            {
                return System.Text.RegularExpressions.Regex.Replace(ISOtoASCII(words), Common.MatchEmptySpacesPattern, " ").Split(Common.Separators, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Is the value of the href pointing to a web page?
        /// </summary>
        /// <param name="foundHref">The value of the href that needs to be interogated.</param>
        /// <returns>Boolen </returns>
        public static bool IsAWebPage(string foundHref)
        {
            if (foundHref.Length < 2 || foundHref.IndexOf("javascript:") > -1 || foundHref.IndexOf("mailto:") > -1 || foundHref.StartsWith("#") || foundHref.StartsWith("file://") || foundHref.StartsWith(@"\\") || foundHref.StartsWith("ftp://"))
            {
                return false;
            }

            if (!Uri.IsWellFormedUriString(foundHref, UriKind.RelativeOrAbsolute))
            {
                return false;
            }

            string extension = "";

            try
            {
                Uri uri = new Uri(foundHref);
                if (uri.Segments.Length == 0)
                {
                }
                string lastSegment = uri.Segments[uri.Segments.Length - 1];

                if (lastSegment.Contains("."))
                {
                    extension = lastSegment.Substring(lastSegment.LastIndexOf(".") + 1, lastSegment.Length - lastSegment.LastIndexOf(".") - 1);
                }
                else
                {
                    return true;
                }
            }
            catch//relative url
            {
                if (foundHref.Contains("."))
                {
                    extension = foundHref.Substring(foundHref.LastIndexOf(".") + 1, foundHref.Length - foundHref.LastIndexOf(".") - 1);
                }
                else
                {
                    return true;
                }
            }

            switch (extension)
            {
                case "htm":
                case "html":
                case "shtml":
                case "dhtml":
                case "xhtml":
                case "asp":
                case "aspx":
                case "cgi":
                case "php":
                case "jsf":
                case "jsp":
                case "pl":
                case "txt":
                    return true;
                case "mp3":
                case "pdf":
                    return !Preferences.IndexOnlyHTMLDocuments;
                default:
                    return false;
            }
        }

        /// <summary>
        /// False if the link exists, so the website must be skipped
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>False if the link exists</returns>
        protected bool AddURLtoGlobalVisited(Uri uri)
        {
            lock (Spider.GlobalVisitedURLs)
            {
                string link = Common.GetHttpAuthority(uri);
                if (!Spider.GlobalVisitedURLs.Contains(link))
                {
                    Spider.GlobalVisitedURLs.Add(link);

                    DocumentProgressEvent(new Report.ProgressEventArgs(Report.EventTypes.Start, "Crawling website(redirected):" + link));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected bool DeleteFile(string filename)
        {
            // delete file
            try
            {
                new System.IO.FileInfo(filename).Delete();
            }
            catch (Exception e)
            {
                Report.Logger.ErrorLog(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Convert ISO Characters to ASCII
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        protected string ISOtoASCII(string strText)
        {
            string[] strFrom = new string[135];
            string[] strTo = new string[135];

            strFrom[1] = "&Agrave;"; strTo[1] = "À"; //capital A, grave accent       
            strFrom[2] = "&agrave;"; strTo[2] = "à"; //small a, grave accent        
            strFrom[3] = "&Aacute;"; strTo[3] = "Á"; //capital A, acute accent      
            strFrom[4] = "&aacute;"; strTo[4] = "á"; //small a, acute accent        
            strFrom[5] = "&Acirc;"; strTo[5] = "Â"; //capital A, circumflex        
            strFrom[6] = "&acirc;"; strTo[6] = "â"; //small a, circumflex          
            strFrom[7] = "&Atilde;"; strTo[7] = "Ã"; //capital A, tilde             
            strFrom[8] = "&atilde;"; strTo[8] = "ã"; //small a, tilde               
            strFrom[9] = "&Auml;"; strTo[9] = "Ä"; //capital A, diæresis/umlaut   
            strFrom[10] = "&auml;"; strTo[10] = "ä"; //small a, diæresis/umlaut     
            strFrom[11] = "&Aring;"; strTo[11] = "Å"; //capital A, ring              
            strFrom[12] = "&aring;"; strTo[12] = "å"; //small a, ring                
            strFrom[13] = "&AElig;"; strTo[13] = "Æ"; //capital AE ligature          
            strFrom[14] = "&aelig;"; strTo[14] = "æ"; //small ae ligature            
            strFrom[15] = "&Ccedil;"; strTo[15] = "Ç"; //capital C, cedilla           
            strFrom[16] = "&ccedil;"; strTo[16] = "ç"; //small c, cedilla             
            strFrom[17] = "&Egrave;"; strTo[17] = "È"; //capital E, grave accent      
            strFrom[18] = "&egrave;"; strTo[18] = "è"; //small e, grave accent        
            strFrom[19] = "&Eacute;"; strTo[19] = "É"; //capital E, acute accent      
            strFrom[20] = "&eacute;"; strTo[20] = "é"; //small e, acute accent        
            strFrom[21] = "&Ecirc;"; strTo[21] = "Ê"; //capital E, circumflex        
            strFrom[22] = "&ecirc;"; strTo[22] = "ê"; //small e, circumflex          
            strFrom[23] = "&Euml;"; strTo[23] = "Ë"; //capital E, diæresis/umlaut   
            strFrom[24] = "&euml;"; strTo[24] = "ë"; //small e, diæresis/umlaut     
            strFrom[25] = "&Igrave;"; strTo[25] = "Ì"; //capital I, grave accent      
            strFrom[26] = "&igrave;"; strTo[26] = "ì"; //small wordsCount, grave accent        
            strFrom[27] = "&Iacute;"; strTo[27] = "Í"; //capital I, acute accent      
            strFrom[28] = "&iacute;"; strTo[28] = "í"; //small wordsCount, acute accent        
            strFrom[29] = "&Icirc;"; strTo[29] = "Î"; //capital I, circumflex        
            strFrom[30] = "&icirc;"; strTo[30] = "î"; //small wordsCount, circumflex          
            strFrom[31] = "&Iuml;"; strTo[31] = "Ï"; //capital I, diæresis/umlaut   
            strFrom[32] = "&iuml;"; strTo[32] = "ï"; //small wordsCount, diæresis/umlaut  
            strFrom[33] = "&ETH;"; strTo[33] = "Ð"; //capital Eth, Icelandic
            strFrom[34] = "&eth;"; strTo[34] = "ð"; //small eth, Icelandic
            strFrom[35] = "&Ntilde;"; strTo[35] = "Ñ"; //capital N, tilde        
            strFrom[36] = "&ntilde;"; strTo[36] = "ñ"; //small n, tilde               
            strFrom[37] = "&Ograve;"; strTo[37] = "Ò"; //capital O, grave accent      
            strFrom[38] = "&ograve;"; strTo[38] = "ò"; //small o, grave accent             
            strFrom[39] = "&Oacute;"; strTo[39] = "Ó"; //capital O, acute accent      
            strFrom[40] = "&oacute;"; strTo[40] = "ó"; //small o, acute accent        
            strFrom[41] = "&Ocirc;"; strTo[41] = "Ô"; //capital O, circumflex   
            strFrom[42] = "&ocirc;"; strTo[42] = "ô"; //small o, circumflex            
            strFrom[43] = "&Otilde;"; strTo[43] = "Õ"; //capital O, tilde             
            strFrom[44] = "&otilde;"; strTo[44] = "õ"; //small o, tilde               
            strFrom[45] = "&Ouml;"; strTo[45] = "Ö"; //capital O, diæresis/umlaut 
            strFrom[46] = "&ouml;"; strTo[46] = "ö"; //small o, diæresis/umlaut   
            strFrom[47] = "&Oslash;"; strTo[47] = "Ø"; //capital O, slash                   
            strFrom[48] = "&oslash;"; strTo[48] = "ø"; //small o, slash          
            strFrom[49] = "&Ugrave;"; strTo[49] = "Ù"; //capital U, grave accent           
            strFrom[50] = "&ugrave;"; strTo[50] = "ù"; //small u, grave accent        
            strFrom[51] = "&Uacute;"; strTo[51] = "Ú"; //capital U, acute accent      
            strFrom[52] = "&uacute;"; strTo[52] = "ú"; //small u, acute accent        
            strFrom[53] = "&Ucirc;"; strTo[53] = "Û"; //capital U, circumflex          
            strFrom[54] = "&ucirc;"; strTo[54] = "û"; //small u, circumflex            
            strFrom[55] = "&Uuml;"; strTo[55] = "Ü"; //capital U, diæresis/umlaut 
            strFrom[56] = "&uuml;"; strTo[56] = "ü"; //small u, diæresis/umlaut      
            strFrom[57] = "&Yacute;"; strTo[57] = "Ý"; //capital Y, acute accent      
            strFrom[58] = "&yacute;"; strTo[58] = "ý"; //small y, acute accent        
            strFrom[59] = "&THORN;"; strTo[59] = "Þ"; //capital Thorn, Icelandic       
            strFrom[60] = "&thorn;"; strTo[60] = "þ"; //small thorn, Icelandic         
            strFrom[61] = "&szlig;"; strTo[61] = "ß"; //small sharp s, German sz           
            strFrom[62] = "&yuml;"; strTo[62] = "ÿ"; //small y, diæresis/umlaut 
            strFrom[63] = "&nbsp;"; strTo[63] = " "; //non-breaking space          
            strFrom[64] = "&iexcl;"; strTo[64] = "¡"; //inverted exclamation mark   
            strFrom[65] = "&cent;"; strTo[65] = "¢"; //cent sign                   
            strFrom[66] = "&pound;"; strTo[66] = "£"; //pound sign                  
            strFrom[67] = "&curren;"; strTo[67] = "¤"; //general currency sign       
            strFrom[68] = "&yen;"; strTo[68] = "¥"; //yen sign                    
            strFrom[69] = "&brvbar;"; strTo[69] = "¦"; //broken [vertical] bar       
            strFrom[70] = "&sect;"; strTo[70] = "§"; //section sign                
            strFrom[71] = "&uml;"; strTo[71] = "¨"; //umlaut/dieresis             
            strFrom[72] = "&copy;"; strTo[72] = "©"; //copyright sign              
            strFrom[73] = "&ordf;"; strTo[73] = "ª"; //ordinal indicator, fem      
            strFrom[74] = "&laquo;"; strTo[74] = "«"; //angle quotation mark, left  
            strFrom[75] = "&not;"; strTo[75] = "¬"; //not sign                    
            strFrom[76] = "&shy;"; strTo[76] = "­"; //soft hyphen                 
            strFrom[77] = "&reg;"; strTo[77] = "®"; //registered sign             
            strFrom[78] = "&macr;"; strTo[78] = "¯"; //macron                      
            strFrom[79] = "&deg;"; strTo[79] = "°"; //degree sign                 
            strFrom[80] = "&#160;"; strTo[80] = " "; //non-breaking space          
            strFrom[81] = "&#161;"; strTo[81] = "¡"; //inverted exclamation mark   
            strFrom[82] = "&#162;"; strTo[82] = "¢"; //cent sign                   
            strFrom[83] = "&#163;"; strTo[83] = "£"; //pound sign                  
            strFrom[84] = "&#164;"; strTo[84] = "¤"; //general currency sign       
            strFrom[85] = "&#165;"; strTo[85] = "¥"; //yen sign                    
            strFrom[86] = "&#166;"; strTo[86] = "¦"; //broken [vertical] bar       
            strFrom[87] = "&#167;"; strTo[87] = "§"; //section sign                
            strFrom[88] = "&#168;"; strTo[88] = "¨"; //umlaut/dieresis             
            strFrom[89] = "&#169;"; strTo[89] = "©"; //copyright sign              
            strFrom[90] = "&#170;"; strTo[90] = "ª"; //ordinal indicator, fem      
            strFrom[91] = "&#171;"; strTo[91] = "«"; //angle quotation mark, left  
            strFrom[92] = "&#172;"; strTo[92] = "¬"; //not sign                    
            strFrom[93] = "&#173;"; strTo[93] = "­"; //soft hyphen                 
            strFrom[94] = "&#174;"; strTo[94] = "®"; //registered sign             
            strFrom[95] = "&#175;"; strTo[95] = "¯"; //macron                      
            strFrom[96] = "&#176;"; strTo[96] = "°"; //degree sign                 
            strFrom[97] = "&plusmn;"; strTo[97] = "±"; //plus-or-minus sign          
            strFrom[98] = "&sup2;"; strTo[98] = "²"; //superscript two          
            strFrom[99] = "&sup3;"; strTo[99] = "³"; //superscript three        
            strFrom[100] = "&acute;"; strTo[100] = "´"; //acute accent             
            strFrom[101] = "&micro;"; strTo[101] = "µ"; //micro sign                
            strFrom[102] = "&para;"; strTo[102] = "¶"; //pilcrow [paragraph sign] 
            strFrom[103] = "&middot;"; strTo[103] = "·"; //middle dot               
            strFrom[104] = "&cedil;"; strTo[104] = "¸"; //cedilla                  
            strFrom[105] = "&sup1;"; strTo[105] = "¹"; //superscript one          
            strFrom[106] = "&ordm;"; strTo[106] = "º"; //ordinal indicator, male  
            strFrom[107] = "&raquo;"; strTo[107] = "»"; //angle quotation mark, right   
            strFrom[108] = "&frac14;"; strTo[108] = "¼"; //fraction one-quarter          
            strFrom[109] = "&frac12;"; strTo[109] = "½"; //fraction one-half             
            strFrom[110] = "&frac34;"; strTo[110] = "¾"; //fraction three-quarters       
            strFrom[111] = "&iquest;"; strTo[111] = "¿"; //inverted question mark        
            strFrom[112] = "&times;"; strTo[112] = "×"; //multiply sign                 
            strFrom[113] = "&div;"; strTo[113] = "÷"; //division sign             
            strFrom[114] = "&#177;"; strTo[114] = "±"; //plus-or-minus sign          
            strFrom[115] = "&#178;"; strTo[115] = "²"; //superscript two          
            strFrom[116] = "&#179;"; strTo[116] = "³"; //superscript three        
            strFrom[117] = "&#180;"; strTo[117] = "´"; //acute accent             
            strFrom[118] = "&#181;"; strTo[118] = "µ"; //micro sign                
            strFrom[119] = "&#182;"; strTo[119] = "¶"; //pilcrow [paragraph sign] 
            strFrom[120] = "&#183;"; strTo[120] = "·"; //middle dot               
            strFrom[121] = "&#184;"; strTo[121] = "¸"; //cedilla                  
            strFrom[122] = "&#185;"; strTo[122] = "¹"; //superscript one          
            strFrom[123] = "&#186;"; strTo[123] = "º"; //ordinal indicator, male  
            strFrom[124] = "&#187;"; strTo[124] = "»"; //angle quotation mark, right   
            strFrom[125] = "&#188;"; strTo[125] = "¼"; //fraction one-quarter          
            strFrom[126] = "&#189;"; strTo[126] = "½"; //fraction one-half             
            strFrom[127] = "&#190;"; strTo[127] = "¾"; //fraction three-quarters       
            strFrom[128] = "&#191;"; strTo[128] = "¿"; //inverted question mark        
            strFrom[129] = "&#215;"; strTo[129] = "×"; //multiply sign                 
            strFrom[130] = "&#247;"; strTo[130] = "÷"; //division sign        
            strFrom[131] = "&lt;"; strTo[131] = "<"; //
            strFrom[132] = "&gt;"; strTo[132] = ">"; //  
            strFrom[133] = "&quot;"; strTo[133] = "\"";
            strFrom[134] = "&amp;"; strTo[134] = "&"; // ampersand

            int i;
            for (i = 1; i < strFrom.Length; i = i + 2)
            {
                //strText = Regex.Replace(strText, strFrom[wordsCount], strTo[wordsCount], RegexOptions.IgnoreCase | RegexOptions.Singleline);
                strText = strText.Replace(strFrom[i], strTo[i]).Replace(strFrom[i + 1], strTo[i + 1]);
            }

            return (strText);
        }

    }
}