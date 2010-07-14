using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace MMarinov.WebCrawler.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            System.IO.StreamWriter wr = new System.IO.StreamWriter(System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0, System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf('\\')) + "\\log.txt", true);
            string str = Report.Logger.FormatErrorMsg(e.Exception);
            wr.Write(str);
            wr.Close();

            MessageBox.Show("An error occured! " + str, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Prevent default unhandled exception processing
            e.Handled = true;

        }
    }
}
