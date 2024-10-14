using System;

namespace KoboldTools
{
    public class LogEntry : ILogEntry
    {
        #region LogEntry
        private System.Object _content;

        public LogEntry(System.Object content, string identifier = "default")
        {
            _content = content;
            _identifier = identifier;
            _timestamp = DateTime.Now;
        }
        #endregion

        #region ILogEntry
        private string _identifier = "default";
        public string identifier
        {
            get
            {
                return _identifier;
            }
        }

        private DateTime _timestamp;
        public DateTime timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public System.Object getContent()
        {
            return _content;
        }
        #endregion
    }
}
