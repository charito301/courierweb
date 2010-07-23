using System;

namespace MMarinov.WebCrawler.Indexer
{
    public static class DocumentFactory
    {
        public static Document New(Uri uri, System.Net.HttpWebResponse contentType)
        {
            Document newDoc = null;
            string mimeType = ParseMimeType(contentType.ContentType.ToString()).ToLower();

            System.Text.Encoding encoding = ParseEncoding(contentType);

            switch (mimeType)
            {
                case "text/css":
                case "text/xml":
                case "application/x-msdownload":
                case "application/octet-stream":
                case "application/xml":
                case "application/rss+xml":
                case "application/rdf+xml":
                case "application/atom+xml":
                case "application/xhtml+xml":
                    break;

                case "application/vnd.ms-powerpoint":
                case "application/msword":
                    //TODO: parse !
                    break;

                case "application/pdf":
                    if (!Preferences.IndexOnlyHTMLDocuments)
                    {
                        newDoc = new PdfDocument(uri);
                    }
                    break;

                case "text/plain":
                    newDoc = new TextDocument(uri);
                    break;

                case "audio/mpeg":
                    if (!Preferences.IndexOnlyHTMLDocuments)
                    {
                        newDoc = new Mp3Document(uri);
                    }
                    break;

                default://case "text/html":
                    //newDoc = new HtmlDocument(uri);
                    if (mimeType.IndexOf("text") > -1)
                    {   // If we got 'text' data (not images)
                        newDoc = new HtmlDocument(uri, mimeType, encoding);
                    }
                    break;
            } // switch

            return newDoc;
        }

        private static string ParseMimeType(string contentType)
        {
            string mimeType = "";
            string[] contentTypeArray = contentType.Split(';');
            // Set MimeType if it's blank
            if (mimeType == "" && contentTypeArray.Length >= 1)
            {
                mimeType = contentTypeArray[0];
            }
            return mimeType;
        }

        internal static System.Text.Encoding ParseEncoding(string contentType)
        {
            string encoding = "";
            string[] contentTypeArray = contentType.ToLower().Split(';');
            // Set _Encoding if it's blank
            if (encoding == "" && contentTypeArray.Length >= 2)
            {
                int charsetpos = contentTypeArray[1].IndexOf("charset");
                if (charsetpos > 0)
                {
                    encoding = contentTypeArray[1].Substring(charsetpos + 8, contentTypeArray[1].Length - charsetpos - 8);

                    if (encoding != "")
                    {
                        return System.Text.Encoding.GetEncoding(encoding);
                    }
                }
            }

            return null;
        }

        private static System.Text.Encoding ParseEncoding(System.Net.HttpWebResponse webResponse)
        {
            if (!string.IsNullOrEmpty(webResponse.CharacterSet))
            {
                return System.Text.Encoding.GetEncoding(webResponse.CharacterSet);
            }

            return ParseEncoding(webResponse.ContentType);
        }
    }
}
