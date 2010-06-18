using System;
using System.Text.RegularExpressions;

namespace MMarinov.WebCrawler.Indexer
{
    /// <summary>
    /// Storage for parsed HTML data returned by ParsedHtmlData();
    /// </summary>
    /// <remarks>
    /// Arbitrary class to encapsulate just the properties we need 
    /// to index Html pages (Title, Meta tags, Keywords, etc).
    /// </remarks>
    public class HtmlDocument : Document
    {
        #region Private fields: _Uri, _ContentType, _RobotIndexOK, _RobotFollowOK

        private string _htmlCode = "";
        private String _ContentType;
        private bool _RobotIndexOK = true;
        private bool _RobotFollowOK = true;
        private string _WordsOnly = "";
        /// <summary>MimeType so we know whether to try and parse the contents, eg. "text/html", "text/plain", etc</summary>
        private string _MimeType = "";
        /// <summary>Html &lt;title&gt; tag</summary>
        private String _Title = "";
        /// <summary>Html &lt;meta http-equiv='description'&gt; tag</summary>
        private string _Description = "";
        private string _keywords = "";
        private MMarinov.WebCrawler.Stemming.Languages _language = MMarinov.WebCrawler.Stemming.Languages.None;

        private System.Collections.Generic.List<string> linksLocal = new System.Collections.Generic.List<string>();
        private System.Collections.Generic.List<string> linksExternal = new System.Collections.Generic.List<string>();

        #endregion

        #region Constructor requires Uri

        public HtmlDocument(Uri location)
            : base(location)
        {
            this.Uri = location;
        }

        public HtmlDocument(Uri location, string mimeType)
            : base(location, mimeType)
        {
            this.Uri = location;
            _MimeType = mimeType;
        }

        #endregion

        #region Public Properties: Uri, RobotIndexOK
        /// <summary>
        /// Whether a robot should index the text 
        /// found on this page, or just ignore it
        /// </summary>
        /// <remarks>
        /// Set when page META tags are parsed - no 'set' property
        /// More info:
        /// http://www.robotstxt.org/
        /// </remarks>
        public override bool RobotIndexOK
        {
            get { return _RobotIndexOK; }
        }
        /// <summary>
        /// Whether a robot should follow any links 
        /// found on this page, or just ignore them
        /// </summary>
        /// <remarks>
        /// Set when page META tags are parsed - no 'set' property
        /// More info:
        /// http://www.robotstxt.org/
        /// </remarks>
        public override bool RobotFollowOK
        {
            get { return _RobotFollowOK; }
        }

        public override string ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value.ToString();
                string[] contentTypeArray = _ContentType.Split(';');
                // Set MimeType if it's blank
                if (_MimeType == "" && contentTypeArray.Length >= 1)
                {
                    _MimeType = contentTypeArray[0];
                }
                // Set Encoding if it's blank
                if (Encoding == null && contentTypeArray.Length >= 2)
                {
                    int charsetpos = contentTypeArray[1].IndexOf("charset");
                    if (charsetpos > 0)
                    {
                        Encoding = System.Text.Encoding.GetEncoding(contentTypeArray[1].Substring(charsetpos + 8, contentTypeArray[1].Length - charsetpos - 8));
                    }
                }
            }
        }

        public MMarinov.WebCrawler.Stemming.Languages Language
        {
            get { return _language; }
        }
        #endregion

        #region Public fields: Encoding, Keywords, All

        /// <summary>
        /// Encoding eg. "utf-8", "Shift_JIS", "iso-8859-1", "gb2312", etc
        /// </summary>
        public System.Text.Encoding Encoding;

        /// <summary>
        /// Html &lt;meta http-equiv='keywords'&gt; tag
        /// </summary>
        public string Keywords
        {
            get
            {
                return _keywords;
            }
        }

        /// <summary>
        /// Raw content of page, as downloaded from the server
        /// Html stripped to make up the 'wordsonly'
        /// </summary>
        public override string AllCode
        {
            get { return _htmlCode; }
            set
            {
                _htmlCode = value;
                _WordsOnly = StripHtml(_htmlCode);
            }
        }
        public override string WordsOnly
        {
            get { return this._keywords + " " + this._Description + " " + this._WordsOnly; }
        }

        public override string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = Regex.Replace(value, Common.MatchEmptySpacesPattern, " ").Trim();
            }
        }

        #endregion

        /// <summary>
        /// Pass in a ROBOTS meta tag found while parsing, 
        /// and set HtmlDocument property/ies appropriately
        /// </summary>
        /// <remarks>
        /// More info:
        /// * Robots Exclusion Protocol *
        /// - for META tags http://www.robotstxt.org/wc/meta-user.html
        /// - for ROBOTS.TXT in the siteroot http://www.robotstxt.org/wc/norobots.html
        /// </remarks>
        public void SetRobotDirective(string robotMetaContent)
        {
            robotMetaContent = robotMetaContent.ToLower();
            if (robotMetaContent.IndexOf("none") >= 0)
            {
                // 'none' means you can't Index or Follow!
                _RobotIndexOK = false;
                _RobotFollowOK = false;
            }
            else
            {
                if (robotMetaContent.IndexOf("noindex") >= 0) { _RobotIndexOK = false; }
                if (robotMetaContent.IndexOf("nofollow") >= 0) { _RobotFollowOK = false; }
            }
        }

        #region Parsing

        /// <summary>
        ///
        /// </summary>
        /// <remarks> Regex on this blog will parse ALL attributes from within tags...
        /// IMPORTANT when they're out of order, spaced out or over multiple lines
        /// http://blogs.worldnomads.com.au/matthewb/archive/2003/10/24/158.aspx
        /// http://blogs.worldnomads.com.au/matthewb/archive/2004/04/06/215.aspx
        /// </remarks>
        public override void Parse()
        {
            if (string.IsNullOrEmpty(this._Title))
            {
                this.Title = Regex.Match(_htmlCode, @"(?<=<title[^\>]*>).*?(?=</title>)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Value;
            }

            ParseLanguage();
            ParseMetaTags();
            ParseLinks();

            this.LocalLinks = linksLocal;
            this.ExternalLinks = linksExternal;
        }// Parse

        private void ParseLinks()
        {
            string link = "";

            // Looks for the src attribute of:
            // <A> anchor tags
            // <AREA> imagemap links
            // <FRAME> frameset links
            // <IFRAME> floating frames
            foreach (Match match in Regex.Matches(_htmlCode, @"(?<anchor><\s*(a|area|frame|iframe)\s*(?:(?:\b\w+\b\s*(?:=\s*(?:""[^""]*""|'[^']*'|[^""'<> ]+)\s*)?)*)?\s*>)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
            {
                // Parse ALL attributes from within tags... IMPORTANT when they're out of order!!
                // in addition to the 'href' attribute, there might also be 'alt', 'class', 'style', 'area', etc...
                // there might also be 'spaces' between the attributes and they may be ", ', or unquoted
                link = "";

                foreach (Match submatch in Regex.Matches(match.Value.ToString(), @"(?<name>\b\w+\b)\s*=\s*(""(?<value>[^""]*)""|'(?<value>[^']*)'|(?<value>[^""'<> \s]+)\s*)+", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
                {
                    // we're only interested in the href attribute (although in future maybe index the 'alt'/'title'?)
                    if ("href" == submatch.Groups[1].ToString().ToLower())
                    {
                        link = submatch.Groups[2].ToString();

                        if (link != "#")
                        {
                            break; // break if this isn't just a placeholder href="#", which implies maybe an onclick attribute exists
                        }
                    }
                    if ("onclick" == submatch.Groups[1].ToString().ToLower())
                    {
                        string jscript = submatch.Groups[2].ToString();
                        // some code here to extract a filename/link to follow from the onclick="_____"
                        // say it was onclick="window.location='top.htm'"
                        int firstApos = jscript.IndexOf("'");
                        int secondApos = jscript.IndexOf("'", firstApos + 1);
                        if (secondApos > firstApos)
                        {
                            link = jscript.Substring(firstApos + 1, secondApos - firstApos - 1);
                            break;  // break if we found something, ignoring any later href="" which may exist _after_ the onclick in the <a> element
                        }
                    }
                }

                Document.FoundTotalLinks++;
                AddLinkToCollection(link);
            } // foreach
        }// Parse Links

        /// <summary>
        /// Gets the content of meta tags and set keywords, description and robot directives
        /// </summary>
        private void ParseMetaTags()
        {
            string metaKey = "";
            string metaValue = "";

            foreach (Match metamatch in Regex.Matches(_htmlCode, @"<meta\s*(?:(?:\b(\w|-)+\b\s*(?:=\s*(?:""[^""]*""|'[^']*'|[^""'<> ]+)\s*)?)*)/?\s*>", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
            {
                metaKey = "";
                metaValue = "";
                // Loop through the attribute/value pairs inside the tag
                foreach (Match submetamatch in Regex.Matches(metamatch.Value, @"(?<name>\b(\w|-)+\b)\s*=\s*(""(?<value>[^""]*)""|'(?<value>[^']*)'|(?<value>[^""'<> ]+)\s*)+", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
                {
                    switch (submetamatch.Groups[1].ToString().ToLower())
                    {
                        case "http-equiv":
                            metaKey = submetamatch.Groups[2].ToString();
                            break;
                        case "name":
                            if (metaKey == "")
                            { // if it's already set, HTTP-EQUIV takes precedence
                                metaKey = submetamatch.Groups[2].ToString();
                            }
                            break;
                        case "content":
                            metaValue = submetamatch.Groups[2].ToString();
                            break;
                        default: break;
                    }
                }

                switch (metaKey.ToLower())
                {
                    case "description":
                        _Description = metaValue;
                        break;
                    case "keywords":
                    case "keyword":
                        Array.ForEach<string>(base.WordsStringToArray(metaValue), word => _keywords += word + " ");
                        break;
                    case "robots":
                    case "robot":
                        this.SetRobotDirective(metaValue);
                        break;
                    default: break;
                }
            }

        }//Parse MetaTags

        /// <summary>
        /// Parse HTML tag to look for lang or xml:lang tag.
        /// </summary>
        private void ParseLanguage()
        {
            Match htmlMatch = Regex.Match(_htmlCode, @"<html\b(?>\s+(?:alt=""([^""]*)""|lang=""([^""]*)""|xml:lang=""([^""]*)"")|[^\s>]+|\s+)*>", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            // Loop through the attribute/value pairs inside the tag
            foreach (Match submetamatch in Regex.Matches(htmlMatch.Value, @"(?<name>\b(\w|-)+\b)\s*=\s*(""(?<value>[^""]*)""|'(?<value>[^']*)'|(?<value>[^""'<> ]+)\s*)+", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
            {
                if (submetamatch.Groups[1].Value.ToLower() == "lang")
                {
                    switch (submetamatch.Groups[2].Value.ToLower())
                    {
                        case "en":
                            _language = Stemming.Languages.English;
                            break;
                        case "de":
                            _language = Stemming.Languages.German;
                            break;
                        case "bg":
                            _language = Stemming.Languages.Bulgarian;
                            break;
                        default: break;
                    }
                }
                else if (submetamatch.Groups[1].Value.ToLower() == "xml:lang" && _language == Stemming.Languages.None)
                {
                    switch (submetamatch.Groups[2].Value.ToLower())
                    {
                        case "en":
                            _language = Stemming.Languages.English;
                            break;
                        case "de":
                            _language = Stemming.Languages.German;
                            break;
                        case "bg":
                            _language = Stemming.Languages.Bulgarian;
                            break;
                        default: break;
                    }
                }
            }

        }// ParseLanguage

        /// <summary>
        /// Checks link and adds it to external/local links collection
        /// </summary>
        /// <param name="link"></param>
        private void AddLinkToCollection(string link)
        {
            // strip off internal links, so we don't index same page over again
            if (link.Contains("#"))
            {
                link = link.Substring(0, link.IndexOf("#"));
            }

            //                       !!!!!!!!!!!!!
            if (link.Contains("?"))//!!!!!!!!!!!!!
            {
                link = link.Substring(0, link.IndexOf("?"));
            }

            link = link.Replace('\\', '/');

            if (!Document.IsAWebPage(link))
            {
                return;
            }

            if (link.Length > 1 && !link.StartsWith("/") && (link.StartsWith(Common.HTTP) || link.StartsWith("https://")))
            {
                try
                {
                    Uri address = new Uri(link);
                    string authority = Regex.Replace(address.Authority, Common.MatchWwwDigitDotPattern, "");

                    if (authority == this.Uri.Authority)
                    {
                        if (!linksLocal.Contains(address.PathAndQuery))
                        {
                            linksLocal.Add(address.PathAndQuery); //gets only the relative link
                            Document.FoundValidLinks++;
                        }
                    }
                    else if (!linksExternal.Contains(Common.GetHttpAuthority(address)))
                    {
                        linksExternal.Add(Common.GetHttpAuthority(address));
                        Document.FoundValidLinks++;
                    }
                }
                catch
                {
                    base.DocumentProgressEvent(new Report.ProgressEventArgs(new Exception("Malformed URL: " + link)));
                }
            }
            //else if (link.StartsWith("?"))
            //{
            //    if (!linksLocal.Contains(this.Uri.AbsolutePath + link))
            //    {
            //        // it's possible to have /?query which sends the querystring to the 'default' page in a directory
            //        linksLocal.Add(this.Uri.AbsolutePath + link);
            //    }
            //}
            else if (!linksLocal.Contains(link))
            {
                linksLocal.Add(link);
                Document.FoundValidLinks++;
            }
        }

        #endregion

        public override bool GetResponse(System.Net.HttpWebResponse webResponse)
        {
            if (webResponse.ContentEncoding != "")
            {
                // Use the HttpHeader Content-Type in preference to the one set in META
                this.Encoding = System.Text.Encoding.GetEncoding(webResponse.ContentEncoding);
            }
            else if (this.Encoding == null)
            {
                this.Encoding = System.Text.Encoding.UTF8; // default
            }

            System.IO.StreamReader stream = null;

            try
            {
                // DateTime startDLtime = DateTime.Now;

                stream = new System.IO.StreamReader(webResponse.GetResponseStream(), this.Encoding);

                if (webResponse.ResponseUri != this.Uri)
                {
                    this.Uri = webResponse.ResponseUri; // we *may* have been redirected... and we want the *final* URL

                    if (!base.AddURLtoGlobalVisited(this.Uri))
                    {
                        return false;
                    }
                }

                this.AllCode = stream.ReadToEnd();

                //DateTime endDLtime = DateTime.Now;
                //double periodDL = (endDLtime - startDLtime).TotalSeconds;

                //if (periodDL > 0)
                //{
                //    int bytesPerSec = (int)(this.Encoding.GetBytes(AllCode).Length / periodDL);
                //    System.Console.WriteLine(bytesPerSec / 1024 + " KB/s");
                //}
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

            return true; //success
        }//GetResponse


        /// <summary>
        /// Stripping HTML
        /// http://www.4guysfromrolla.com/webtech/042501-1.shtml
        /// </summary>
        /// <remarks>
        /// Using regex to find tags without a trailing slash
        /// http://concepts.waetech.com/unclosed_tags/index.cfm
        ///
        /// Replace html comment tags
        /// http://www.faqts.com/knowledge_base/view.phtml/aid/21761/fid/53
        /// </remarks>
        protected string StripHtml(string htmlCode)
        {
            const string matchCommentPattern = @"(\<![ \r\n\t]*(--([^\-]|[\r\n]|-[^\-])*--[ \r\n\t]*)\>)";

            htmlCode = Regex.Replace(htmlCode, "&amp;", "&", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            //Strips the <script> and <noscript> tags, comments <!-- --> , <style>
            htmlCode = Regex.Replace(htmlCode, MatchTag("script") + "|" + MatchTag("noscript") + "|" + matchCommentPattern + "|" + MatchTag("style"), " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            // htmlCode = Regex.Replace(htmlCode, @"(<script.*?</script>)|(<noscript.*?</noscript>)|(\<![ \r\n\t]*(--([^\-]|[\r\n]|-[^\-])*--[ \r\n\t]*)\>)|(<style.*?</style>)", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            htmlCode = ISOtoASCII(htmlCode);

            //removes tags, new lines and multiple spaces 
            htmlCode = Regex.Replace(htmlCode, "<(.|\n)*?>", " ");
            // new lines and multiple spaces 
            htmlCode = Regex.Replace(htmlCode, "(&(.|\n)+?;)|(\r?\n?)", "");
            htmlCode = Regex.Replace(htmlCode, Common.MatchEmptySpacesPattern, " ");

            return htmlCode;
        }//stripHtml

        /// <summary>
        /// Convert ISO Characters to ASCII
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        private string ISOtoASCII(string strText)
        {
            string[] strFrom = new string[131];
            string[] strTo = new string[131];
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

            int i;
            for (i = 1; i < strFrom.Length; i = i + 2)
            {
                //strText = Regex.Replace(strText, strFrom[wordsCount], strTo[wordsCount], RegexOptions.IgnoreCase | RegexOptions.Singleline);
                strText = strText.Replace(strFrom[i], strTo[i]).Replace(strFrom[i + 1], strTo[i + 1]);
            }

            return (strText);
        }

        private string MatchTag(string tagName)
        {
            return @"(\<[ \r\n\t]*" + tagName + @"([ \r\n\t\>]|\>){1,}([ \r\n\t]|.)*</[ \r\n\t]*" + tagName + @"[ \r\n\t]*\>)";
        }
    }
}
