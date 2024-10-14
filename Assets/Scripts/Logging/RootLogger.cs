using System;
using System.Diagnostics;
using UnityEngine;

namespace KoboldTools.Logging {
    /// <summary>
    /// Provides global access to a logger instance. In many logging
    /// frameworks, this is called the root logger.
    /// </summary>
    [RequireComponent(typeof(Logger))]
    public class RootLogger : MonoBehaviour {
        private static Logger Instance;

        private void Awake() {
            RootLogger.Instance = GetComponent<Logger>();
            RootLogger.Instance.LoggerName = "Root Logger";
        }

        public static Level LogLevel {
            get {
                if (RootLogger.Instance != null) {
                    return RootLogger.Instance.LogLevel;
                } else {
                    return Level.EXCEPTION;
                }
            }
        }

        // [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Debug(object context, string format, params object[] args) {
            if (RootLogger.Instance != null) {
                RootLogger.Instance.Debug(context, format, args);
            }
        }

        public static void Info(object context, string format, params object[] args) {
            if (RootLogger.Instance != null) {
                RootLogger.Instance.Info(context, format, args);
            }
        }

        public static void Warning(object context, string format, params object[] args) {
            if (RootLogger.Instance != null) {
                RootLogger.Instance.Warning(context, format, args);
            }
        }

        public static void Error(object context, string format, params object[] args) {
            if (RootLogger.Instance != null) {
                RootLogger.Instance.Error(context, format, args);
            }
        }

        public static void Exception(object context, string format, params object[] args) {
            if (RootLogger.Instance != null) {
                RootLogger.Instance.Exception(context, format, args);
            }
        }
    }
}