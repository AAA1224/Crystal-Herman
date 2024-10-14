using UnityEngine.Events;
using System.Collections.Generic;

namespace KoboldTools
{
    public interface ILogger
    {
        List<ILogEntry> data
        { get; }
        UnityEvent eDataChanged
        { get; }

        void log(System.Object data, string identifier);
    }
}
