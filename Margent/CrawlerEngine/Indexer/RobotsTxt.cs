using System;
using System.Text;

namespace MMarinov.WebCrawler.Indexer
{
    /// <summary>
    /// Represents the rules for a specific domain for a specific host 
    /// (ie it aggregates all the rules that match the UserAgent, plus the special * rules)
    /// 
    /// http://www.robotstxt.org/
    /// </summary>
    public class RobotsTxt
    {
        #region Private Fields: _FileContents, _UserAgent, _Server, _DenyUrls, _LogString

        private string _FileContents;
        private string _UserAgent;
        private string _Server;
        /// <summary>lowercase string array of url fragments that are 'denied' to the UserAgent for this RobotsTxt instance</summary>
        private System.Collections.Generic.List<string> _DenyUrls = new System.Collections.Generic.List<string>();

        #endregion

        #region Constructors: require starting Url and UserAgent to create an object
        private RobotsTxt()
        { }

        public RobotsTxt(Uri startPageUri, string userAgent)
        {
            _UserAgent = userAgent;
            _Server = startPageUri.Host;

            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://" + startPageUri.Authority + "/robots.txt");
            try
            {
                System.Net.HttpWebResponse webresponse = (System.Net.HttpWebResponse)req.GetResponse();

                using (System.IO.StreamReader stream = new System.IO.StreamReader(webresponse.GetResponseStream(), Encoding.ASCII))
                {
                    _FileContents = stream.ReadToEnd();
                } // stream.Close();

                string[] fileLines = _FileContents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                bool rulesApply = false;
                foreach (string line in fileLines)
                {
                    RobotInstruction ri = new RobotInstruction(line);
                    if (ri.Instruction == "")
                    {
                        continue;
                    }

                    switch (ri.Instruction[0])
                    {
                        case '#':   //then comment - ignore
                            break;
                        case 'u':   // User-Agent
                            rulesApply = (ri.UrlOrAgent.IndexOf("*") >= 0) || (ri.UrlOrAgent.IndexOf(_UserAgent) >= 0);
                            break;
                        case 'd':   // Disallow
                            if (rulesApply)
                            {
                                _DenyUrls.Add(ri.UrlOrAgent.ToLower());
                            }
                            break;
                        default:
                            // empty/unknown/error/allow
                            break;
                    }
                }
            }
            catch (System.Net.WebException)
            {
                _FileContents = "";
            }
        }
        #endregion

        #region Methods: Allow
        /// <summary>
        /// Does the parsed robots.txt file allow this Uri to be spidered for this user-agent?
        /// </summary>
        /// <remarks>
        /// This method does all its "matching" in lowercase - it expects the _DenyUrl 
        /// elements to be ToLower() and it calls ToLower on the passed-in Uri...
        /// </remarks>
        public bool Allowed(Uri uri)
        {
            if (_DenyUrls.Count == 0) return true;

            string url = uri.AbsolutePath.ToLower();

            foreach (string denyUrlFragment in _DenyUrls)
            {
                if (url.Length >= denyUrlFragment.Length && url.Substring(0, denyUrlFragment.Length) == denyUrlFragment)
                {
                    return false;
                }
                // else url is shorter than fragment, therefore cannot be a 'match' // else not a match
            }
            if (url == "/robots.txt")
            {
                return false;
            }

            // no disallows were found, so allow
            return true;
        }

        #endregion

        /// <summary>
        /// Use this class to read/parse the robots.txt file
        /// </summary>
        /// <remarks>
        /// Types of data coming into this class
        /// User-agent: * ==> _Instruction='User-agent', _Url='*'
        /// Disallow: /cgi-bin/ ==> _Instruction='Disallow', _Url='/cgi-bin/'
        /// Disallow: /tmp/ ==> _Instruction='Disallow', _Url='/tmp/'
        /// Disallow: /~joe/ ==> _Instruction='Disallow', _Url='/~joe/'
        /// </remarks>
        private class RobotInstruction
        {
            private string _Instruction = "";
            private string _Url = "";

            /// <summary>
            /// Constructor requires a line, hopefully in the format [instuction]:[url]
            /// </summary>
            public RobotInstruction(string line)
            {
                string instructionLine = line.Trim();
                int commentPosition = instructionLine.IndexOf('#');
                if (commentPosition == 0)
                {
                    _Instruction = "#";
                }
                if (commentPosition >= 0)
                {   // comment somewhere on the line, trim it off
                    instructionLine = instructionLine.Substring(0, commentPosition);
                }
                if (instructionLine.Length > 0)
                {   // wasn't just a comment line (which should have been filtered out before this anyway
                    string[] lineArray = instructionLine.Split(':');
                    _Instruction = lineArray[0].Trim().ToLower();
                    if (lineArray.Length > 1)
                    {
                        _Url = lineArray[1].Trim();
                    }
                }
            }
            /// <summary>
            /// Lower-case part of robots.txt line, before the colon (:)
            /// </summary>
            public string Instruction
            {
                get { return _Instruction; }
            }
            /// <summary>
            /// Lower-case part of robots.txt line, after the colon (:)
            /// </summary>
            public string UrlOrAgent
            {
                get { return _Url; }
            }
        }
    }
}
