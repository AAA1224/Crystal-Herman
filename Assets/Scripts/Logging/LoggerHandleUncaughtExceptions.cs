using System;
using System.Linq;
using UnityEngine;

namespace KoboldTools
{
    namespace Logging
    {
        public class LoggerHandleUncaughtExceptions : MonoBehaviour
        {
            public bool Enable = true;
            public bool RunInReleaseMode = false;
            public int TruncateStackTrace = 3;
            private static bool HandlerRegistered = false;

            public void Start()
            {
                if (!LoggerHandleUncaughtExceptions.HandlerRegistered)
                {
                    LoggerHandleUncaughtExceptions.HandlerRegistered = true;
                    Application.logMessageReceived += this.HandleUncaughtException;
                }
            }

            public void HandleUncaughtException(string condition, string stackTrace, LogType type)
            {
                if (this.Enable && type == LogType.Exception)
                {
                    if (this.RunInReleaseMode || Application.isEditor || Debug.isDebugBuild)
                    {
                        string[] traceLines = stackTrace.Split(new [] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        string truncatedTrace = String.Join("\n", traceLines.Take(this.TruncateStackTrace).ToArray());
                        string content = String.Format("{0}\n\n{1}", condition, truncatedTrace);

                        Alert.shout(content, new Alert.AlertParams {
                            title = "Uncaught Exception",
                            closeText = "Close",
                        });
                    }
                }
            }
        }
    }
}
