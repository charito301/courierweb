using System;

namespace MMarinov.WebCrawler.Report
{
    /// <summary>
    /// Declaring the Event Handler delegate
    /// </summary>
    public delegate void SpiderProgressEventHandler(ProgressEventArgs ea);

    /// <summary>
    /// Progress Event Type
    /// </summary>
    public enum EventTypes
    {
        Start,
        Error,
        End,
        Crawling,
        Other
    }
    /// <summary>
    /// Declare the Event arguments
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        private EventTypes _eventType;
        private string _Message = null;
        private System.Net.WebExceptionStatus _webExStatus = System.Net.WebExceptionStatus.Success;
       
        public ProgressEventArgs(EventTypes eventType, string message)
        {
            this._eventType = eventType;
            this._Message = message;
        }

        public ProgressEventArgs(Exception ex)
        {
            this._eventType = EventTypes.Error;

            _Message = Logger.FormatErrorMsg(ex);
        }

        public ProgressEventArgs(Exception ex, System.Net.WebExceptionStatus status)
        {
            this._eventType = EventTypes.Error;
            _webExStatus = status;

            _Message = Logger.FormatErrorMsg(ex);
        }

        public EventTypes EventType
        {
            get { return this._eventType; }
        }

        public string Message
        {
            get { return this._Message; }
        }

        public System.Net.WebExceptionStatus WebExStatus
        {
            get { return _webExStatus; }
        }

    }
}