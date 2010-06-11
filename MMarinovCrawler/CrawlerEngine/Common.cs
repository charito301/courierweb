using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMarinov.WebCrawler
{
    public static class Common
    {
        public static readonly char[] Separators = new char[] { ' ', '|', '&', '^', '@', '!', '?', '`', '~', '_', '/', '\"', ',', '\'', ';', ':', '.', '(', ')', '[', ']', '<', '>', '%', '*', '$', '+', '-', '=', '#', '*', '€', '£' };

        /// <summary>
        /// Regex pattern that matches sequence of www a digit(may not occur) and a dot
        /// </summary>
        public const string MatchWwwDigitDotPattern = @"^www\d{0,1}\.";

        /// <summary>
        /// Regex pattern that matches sequence of empty spaces
        /// </summary>
        public const string MatchEmptySpacesPattern = @"\s+";

        public const string ErrorLogsFolder = "\\ErrorLogs";
        public const string MessageLogsFolder = "\\MessageLogs";       

        public static readonly string ErrorLog = ErrorLogsFolder + "\\ErrorLog.txt";
        public static readonly string ErrorWebLog = ErrorLogsFolder + "\\ErrorWebLog.txt";
        public static readonly string ErrorWebTimeoutLog = ErrorLogsFolder + "\\ErrorWebTimeoutLog.txt";
        public static readonly string ErrorWebProtocolLog = ErrorLogsFolder + "\\ErrorWebProtocolLog.txt";

        public static readonly string MessagesLog = MessageLogsFolder + "\\MessagesLog.txt";
        public static readonly string IndexedLinksLog = MessageLogsFolder + "\\IndexedLinksLog.txt";
        public const string DBLog = MessageLogsFolder + "DBLog.txt";

        public const string DateFormat = "dd/MM/yyyy HH:mm";
        public const string HTTP = "http://";

        public static string GetHttpAuthority(Uri uri)
        {
            return HTTP + System.Text.RegularExpressions.Regex.Replace(uri.Authority, Common.MatchWwwDigitDotPattern, "");
        }
    }
}
