using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{
    public class DataLogger : MonoBehaviour, ILogger
    {
        #region ILogger
        private List<ILogEntry> _data = new List<ILogEntry>();
        public List<ILogEntry> data
        {
            get
            {
                return _data;
            }
        }

        private UnityEvent _eDataChanged = new UnityEvent();
        public UnityEvent eDataChanged
        {
            get
            {
                return _eDataChanged;
            }
        }

        public void log(System.Object data, string identifier = "default")
        {
            _data.Add(new LogEntry(data, identifier));
            eDataChanged.Invoke();
        }
        #endregion
    }
}
