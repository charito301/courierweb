using System;
using System.Linq;

namespace MMarinov.WebCrawler.Indexer
{
    public class SeedList
    {
        private static string _zipFilename = "";
        private static string _csvFilename = "top-1m.csv";

        public static void GetTheList()
        {
            if (DownloadFile())
            {
                ExtractZipArchive();
            }

            FetchFromCVS();
        }

        /// <summary>
        /// Gets links from CVS file and feed the SeedList.
        /// Its stucture is: in first column > cell(index, link)
        /// </summary>
        private static void FetchFromCVS()
        {
            using (System.IO.StreamReader readFile = new System.IO.StreamReader(Preferences.WorkingPath + "\\" + _csvFilename))
            {
                string fileContent = System.Text.RegularExpressions.Regex.Replace(readFile.ReadToEnd(), @"\d+,", "");
                string[] links = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                
                System.Collections.Generic.List<string> linkList = links.ToList<string>();
                linkList.ForEach(url =>
                {
                    try
                    {
                        Spider.GlobalURLsToVisit.Add(Common.GetHttpAuthority(new System.Uri(Common.HTTP + url)));
                    }
                    catch
                    {
                        linkList.Remove(url);
                    }
                });
            }
        }

        private static void ExtractZipArchive()
        {
            using (Ionic.Zip.ZipFile zipArchiveNew = Ionic.Zip.ZipFile.Read(_zipFilename))
            {
                Ionic.Zip.ZipEntry zipFileNew = zipArchiveNew[0];
                _csvFilename = zipFileNew.FileName;
                zipFileNew.Extract(Preferences.WorkingPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);

                MMarinov.WebCrawler.Report.Logger.MessageLog("Zip file " + Preferences.WorkingPath + "\\" + _csvFilename + " extracted at " + System.DateTime.Now.ToString(Common.DateFormat), Report.EventTypes.Other);
            }
        }

        private static bool DownloadFile()
        {
            if (Preferences.SeedURLsSource == "")
            {
                return false;
            }

            try
            {
                System.Uri uriZip = new System.Uri(Preferences.SeedURLsSource);
                _zipFilename = Preferences.WorkingPath + "\\SeedList_" + uriZip.Segments[2];

                System.Net.WebClient client = new System.Net.WebClient();
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFile(uriZip, _zipFilename);
                MMarinov.WebCrawler.Report.Logger.MessageLog("Zip file " + _zipFilename + " downloaded at " + System.DateTime.Now.ToString(Common.DateFormat), Report.EventTypes.Other);

                return true;
            }
            catch (System.Exception e)
            {
                MMarinov.WebCrawler.Report.Logger.ErrorLog(e);
            }

            return false;
        }

        static void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
