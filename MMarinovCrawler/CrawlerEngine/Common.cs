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
        public static readonly string MatchWwwDigitDotPattern = @"^www\d{0,1}\.";

        /// <summary>
        /// Regex pattern that matches sequence of empty spaces
        /// </summary>
        public static readonly string MatchEmptySpacesPattern = @"\s+";
    }
}
