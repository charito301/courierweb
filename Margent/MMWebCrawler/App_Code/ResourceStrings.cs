using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Margent
{
    /// <summary>
    /// Summary description for ResourceStrings
    /// </summary>
    public static class ResourceStrings
    {
        #region EN

        public static string ConnectionErrorEN = "An error occured while trying to fetch data from the DB.";
        public static string EmptyDataEN = "There were not found words, associated to that query. We apologize for the inconvenience.";
        public static string TooShortQueryEN = "Please enter a word, longer that two letters.";
        public static string SearchPrefixEN = "Search the knowledge base:";
        public static string PageTitleEN = "Margent - MMarinov's Search Agent";

        #endregion

        #region DE

        public static string ConnectionErrorDE = "Ein Fehler trat beim Versuch, Daten aus der DB holen.";
        public static string EmptyDataDE = "Es gab keine Worte gefunden, um diese Abfrage zugeordnet. Wir entschuldigen uns für die Unannehmlichkeiten entschuldigen.";
        public static string TooShortQueryDE = "Bitte geben Sie ein Wort, mehr, dass zwei Briefe.";
        public static string SearchPrefixDE = "Suchen Sie das Wissenbank:";
        public static string PageTitleDE = "Margent - der Suchagent des Marinov";

        #endregion

        #region BG

        public static string ConnectionErrorBG = "Възникна грешка при извличане резултати от базата данни. Моля да ни извините за неудобството.";
        public static string EmptyDataBG = "Не бяха открити думи, свързани с търсената.";
        public static string TooShortQueryBG = "Моля, въведете дума с повече от две букви.";
        public static string SearchPrefixBG = "Търсене:";
        public static string PageTitleBG = "Margent - ММаринов Агент";

        #endregion
    }
}