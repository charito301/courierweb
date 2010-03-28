using System;
using System.Web;
using System.Web.Mail;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Inobix.ErrorHandling
{
	public class ErrorLogger
	{
		private static readonly string errorLogFile;
		private static readonly bool isFullLogMode;
		private static readonly string loggedErrorTypes;

		private static readonly object lockObject = new object();

		static ErrorLogger()
		{
			errorLogFile = ConfigurationManager.AppSettings["ErrorLogFile"];
            loggedErrorTypes = ConfigurationManager.AppSettings["ErrorLoggedTypes"];
            isFullLogMode = ConfigurationManager.AppSettings["ErrorLogRecordMode"].ToLower() == "full" ? true : false;
		}		

		/// <summary>
		/// Logs the last exception occured on the server.
		/// </summary>
		public static void HandleException()
		{
			if (HttpContext.Current == null)
			{
				return;
			}

			Exception e = HttpContext.Current.Server.GetLastError();

			if (e == null)
				return;

			e = e.GetBaseException();
			
			if (e != null)
				HandleException(e);
		}

		/// <summary>
		/// Checks if the error should be logged and if true then log it to file.
		/// </summary>
		/// <param name="e">An error.</param>
		public static void HandleException(Exception e, string CustomDescription)
		{
			lock(lockObject)
			{
				string sExceptionDescription = FormatExceptionDescription(e,CustomDescription);
      
				if (LogErrorType(e,loggedErrorTypes))
				{
					LogToFile(sExceptionDescription);
				}
			}
		}

		/// <summary>
		/// Checks if the error should be logged and if true then log it to file.
		/// </summary>
		/// <param name="e">An error.</param>
		public static void HandleException(Exception e)
		{
			HandleException(e,"");
		}

		/// <summary>
		/// Format the exception as it will be saved to the file
		/// </summary>
		/// <param name="e">The exception that is being formated.</param>
		/// <returns>Formated exception.</returns>
		protected static string FormatExceptionDescription(Exception e, string CustomDescription)
		{
			string formatExceptionDescription;

			try
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("<Error>" + Environment.NewLine);

				HttpContext context = HttpContext.Current;
                
				sb.Append("<Time>" + DateTime.Now.ToString("g") + "</Time>" + Environment.NewLine);
				sb.Append("<Url>" + context.Request.Url + "</Url>" + Environment.NewLine);
				sb.Append("<CustomDescription>" + CustomDescription + "</CustomDescription>" + Environment.NewLine);
                
				// Check to see if administrator wants full error messages
				if(isFullLogMode)
				{
					sb.Append("<UrlReferrer>" + context.Request.UrlReferrer + "</UrlReferrer>" + Environment.NewLine);
					sb.Append("<ServerName>" + context.Request.ServerVariables["SERVER_NAME"] + "</ServerName>" + Environment.NewLine);
					sb.Append("<QueryString>" + context.Request.QueryString.ToString() + "</QueryString>" + Environment.NewLine);
					sb.Append("<Form>" + context.Request.Form.ToString() + "</Form>" + Environment.NewLine);
					sb.Append("<Platform>" + context.Request.Browser.Platform + "</Platform>" + Environment.NewLine);
					sb.Append("<IsCrawler>" + context.Request.Browser.Crawler.ToString() + "</IsCrawler>" + Environment.NewLine);
					sb.Append("<UserAgent>" + context.Request.UserAgent + "</UserAgent>" + Environment.NewLine);
					sb.Append("<IsJavaScriptSupported>" + context.Request.Browser.EcmaScriptVersion.ToString() + "</IsJavaScriptSupported>" + Environment.NewLine);
					sb.Append("<AreCookiesSupported>" + context.Request.Browser.Cookies.ToString() + "</AreCookiesSupported>" + Environment.NewLine);
					sb.Append("<UserIP>" + context.Request.UserHostAddress + "</UserIP>" + Environment.NewLine);
					sb.Append("<UserHostName>" + context.Request.UserHostName + "</UserHostName>" + Environment.NewLine);
				}
               
				while (e != null)
				{				
					sb.Append("<Message>" + e.Message + "</Message>" + Environment.NewLine);
					sb.Append("<Source>" + e.Source + "</Source>" + Environment.NewLine);
					sb.Append("<TargetSite>" + e.TargetSite + "</TargetSite>" + Environment.NewLine);
					
					if(isFullLogMode)
					{
						sb.Append("<StackTrace>"+ Environment.NewLine + e.StackTrace + Environment.NewLine + "</StackTrace>"+ Environment.NewLine);
					}
					
					e = e.InnerException;
				}
			
				sb.Append("</Error>" + Environment.NewLine + Environment.NewLine);

				formatExceptionDescription = sb.ToString();
			}
			catch(Exception ex)
			{
				StringBuilder sb2 = new StringBuilder();
				sb2.Append("<FormattingError>" + Environment.NewLine);
				sb2.Append("Time of Error: " + DateTime.Now.ToString("g") + Environment.NewLine);
				sb2.Append("The ErrorHandler.FormatExceptionDescription method has thrown an error (May happen if full logging is enabled and it cannot retrieve the User's Information)" + Environment.NewLine);
				sb2.Append("This is a reduced log entry to reduce the chance of another error being thrown" + Environment.NewLine);
				sb2.Append("The FormatExceptionDescription Method failed to write the error. It received the following error: " + Environment.NewLine);
				sb2.Append(Environment.NewLine);
				sb2.Append("Error Message: " + ex.Message.ToString() + Environment.NewLine);
				sb2.Append("Source: " + ex.Source + Environment.NewLine);
				sb2.Append("Target Site: " + ex.TargetSite + Environment.NewLine);
				sb2.Append("Stack Trace: " + ex.StackTrace + Environment.NewLine);
				sb2.Append(Environment.NewLine);
				sb2.Append("The original error was:" + Environment.NewLine);
				sb2.Append(Environment.NewLine);
				sb2.Append("Error Message: " + e.Message.ToString() + Environment.NewLine);
				sb2.Append("Source: " + e.Source + Environment.NewLine);
				sb2.Append("Target Site: " + e.TargetSite + Environment.NewLine);
				sb2.Append("Stack Trace: " + e.StackTrace + Environment.NewLine);
				sb2.Append(Environment.NewLine + "End of Entry." + Environment.NewLine + Environment.NewLine);
				sb2.Append("</FormattingError>" + Environment.NewLine);
				sb2.Append("\n\n");
				formatExceptionDescription = sb2.ToString();  
			}

			return formatExceptionDescription;
		}

		/// <summary>
		/// Saves the exception to the log file.
		/// </summary>
		/// <param name="sText">Formated exception string that will be inserted to the log file</param>
		static void LogToFile(string sText)
		{
			string sPath;
           
			try
			{
				sPath = System.AppDomain.CurrentDomain.BaseDirectory+errorLogFile;

				FileStream fs = new FileStream(sPath, FileMode.Append, FileAccess.Write);
				StreamWriter writer = new StreamWriter(fs);
				writer.Write(sText);
				writer.Close();
				fs.Close();
				fs = null; // Release the file
			}
			catch(Exception ex)
			{
				if (System.Web.HttpContext.Current != null)
				{
					System.Web.HttpContext.Current.Response.Write("<!--Error:"+ex.Message+"-->");
				}
				// nothing, otherwise there is a chance you could end up with an infinite loop.
			}
		}

		/// <summary>
		/// Checks if the error is from the logged types.
		/// </summary>
		/// <param name="e">The error being checked</param>
		/// <param name="errorList">Comma separated list of allowed error types/codes.(404,400)</param>
		/// <returns>Returns true if the error has type that should be logged.</returns>
		static bool LogErrorType(Exception e, string errorList)
		{
			bool logErrorType;

			if(e is HttpException )
			{
				try
				{
					string errorCode = ((HttpException)e).GetHttpCode().ToString();
					int i;
                
					i = errorList.IndexOf(errorCode);
                
						//If all errors should be logged
					if(errorList == "All")
					{   
						logErrorType = true; 
					}
						//If the error type should be logged
					else if (i > -1)
					{
						logErrorType = true;
					}
					else
					{
						logErrorType = false;
					}
				}
				catch(Exception )
				{
					// If there was an error in trying to determine the error code log the original error anyway.
					logErrorType = true;
				}
			} 
			else
			{
				logErrorType = true;
			}
         
			return logErrorType;
		}

	}
}
