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
        private bool _isWebException = false;
       
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

        public ProgressEventArgs(Exception ex, bool isWebException)
        {
            this._eventType = EventTypes.Error;
            _isWebException = isWebException;

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

        public bool IsWebException
        {
            get { return _isWebException; }
        }

    }
}