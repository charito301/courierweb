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
            Exception ex = e.Exception;
            // Process unhandled exception
            System.IO.StreamWriter wr = new System.IO.StreamWriter(System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0, System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf('\\')) + "\\log.txt", true);
            string str = "";
            while (ex != null)
            {
                str += ex.Message + System.Environment.NewLine + ex.StackTrace;
                ex = ex.InnerException;
            }

            wr.Write(str);
            wr.Close();
         
            // Prevent default unhandled exception processing
            //e.Handled = true;

        }
    }
}
