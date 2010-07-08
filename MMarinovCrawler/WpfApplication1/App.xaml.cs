using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            System.IO.StreamWriter wr = new System.IO.StreamWriter(System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0, System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf('\\') - 1) + "\\log.txt", true);
            string str = "\n-------------------------------\n";
            str += DateTime.Now.ToString() + "\n";
            Exception ex = e.Exception;
            while (ex != null)
            {
                str += ex.Message + "\n" + ex.StackTrace + "\n";
                ex = ex.InnerException;
            }
            str += "-------------------------------";

            wr.Write(str);
            wr.Close();

            MessageBox.Show("An error orrured! " + str, "Марисан", MessageBoxButton.OK, MessageBoxImage.Error);

            // Prevent default unhandled exception processing
            e.Handled = true;

        }
    }
}
