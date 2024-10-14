using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace KoboldTools.Logging {
    [Serializable]
    public enum Level {
        DEBUG = 0x01,
        INFO = 0x02,
        WARNING = 0x04,
        ERROR = 0x08,
        EXCEPTION = 0x10,
    }

    [Flags]
    [Serializable]
    public enum LevelFlag {
        DEBUG = 0x01,
        INFO = 0x02,
        WARNING = 0x04,
        ERROR = 0x08,
        EXCEPTION = 0x10,
    }

    public class RecordEvent : UnityEvent<Record> { }

    public class Logger : MonoBehaviour, ILogHandler {
        public string LoggerName = String.Empty;
        public Level LogLevel = Level.WARNING;
        public LevelFlag CreateStackTrace = LevelFlag.ERROR | LevelFlag.EXCEPTION;
        public bool RedirectUnityLogs = false;
        public bool VerboseContext = false;

        public List<Record> Records { get; private set; }
        public RecordEvent OnRecordCreated { get; private set; }

        private ILogHandler DefaultLogHandler = null;

        private void Awake() {
            this.Records = new List<Record>();
            this.OnRecordCreated = new RecordEvent();

            if (this.RedirectUnityLogs) {
                this.DefaultLogHandler = UnityEngine.Debug.unityLogger.logHandler;
                UnityEngine.Debug.unityLogger.logHandler = this;
            }
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) {
            switch (logType) {
                case LogType.Log:
                    this.Debug(context, format, args);
                    break;

                case LogType.Warning:
                    this.Warning(context, format, args);
                    break;

                case LogType.Assert:
                case LogType.Error:
                    this.Error(context, format, args);
                    break;

                case LogType.Exception:
                    this.Exception(context, format, args);
                    break;

                default:
                    break;
            }
            if (this.DefaultLogHandler != null) {
                this.DefaultLogHandler.LogFormat(logType, context, format, args);
            }
        }

        public void LogException(Exception exception, UnityEngine.Object context) {
            this.Exception(context, exception.Message);
            if (this.DefaultLogHandler != null) {
                this.DefaultLogHandler.LogException(exception, context);
            }
        }

        public void Debug(object context, string format, params object[] args) {
            StackTrace trace = (this.CreateStackTrace & LevelFlag.DEBUG) == LevelFlag.DEBUG ? new StackTrace(true) : null;
            this.CreateEntry(Level.DEBUG, context, trace, format, args);
            if (this.DefaultLogHandler != null) {
                if (context is UnityEngine.Object) {
                    this.DefaultLogHandler.LogFormat(LogType.Log, (UnityEngine.Object) context, format, args);
                } else {
                    this.DefaultLogHandler.LogFormat(LogType.Log, null, format, args);
                }
            }
        }

        public void Info(object context, string format, params object[] args) {
            StackTrace trace = (this.CreateStackTrace & LevelFlag.INFO) == LevelFlag.INFO ? new StackTrace(true) : null;
            this.CreateEntry(Level.INFO, context, trace, format, args);
            if (this.DefaultLogHandler != null) {
                if (context is UnityEngine.Object) {
                    this.DefaultLogHandler.LogFormat(LogType.Log, (UnityEngine.Object) context, format, args);
                } else {
                    this.DefaultLogHandler.LogFormat(LogType.Log, null, format, args);
                }
            }
        }

        public void Warning(object context, string format, params object[] args) {
            StackTrace trace = (this.CreateStackTrace & LevelFlag.WARNING) == LevelFlag.WARNING ? new StackTrace(true) : null;
            this.CreateEntry(Level.WARNING, context, trace, format, args);
            if (this.DefaultLogHandler != null) {
                if (context is UnityEngine.Object) {
                    this.DefaultLogHandler.LogFormat(LogType.Warning, (UnityEngine.Object) context, format, args);
                } else {
                    this.DefaultLogHandler.LogFormat(LogType.Warning, null, format, args);
                }
            }
        }

        public void Error(object context, string format, params object[] args) {
            StackTrace trace = (this.CreateStackTrace & LevelFlag.ERROR) == LevelFlag.ERROR ? new StackTrace(true) : null;
            this.CreateEntry(Level.ERROR, context, trace, format, args);
            if (this.DefaultLogHandler != null) {
                if (context is UnityEngine.Object) {
                    this.DefaultLogHandler.LogFormat(LogType.Error, (UnityEngine.Object) context, format, args);
                } else {
                    this.DefaultLogHandler.LogFormat(LogType.Error, null, format, args);
                }
            }
        }

        public void Exception(object context, string format, params object[] args) {
            StackTrace trace = (this.CreateStackTrace & LevelFlag.EXCEPTION) == LevelFlag.EXCEPTION ? new StackTrace(true) : null;
            this.CreateEntry(Level.EXCEPTION, context, trace, format, args);
            if (this.DefaultLogHandler != null) {
                if (context is UnityEngine.Object) {
                    this.DefaultLogHandler.LogFormat(LogType.Error, (UnityEngine.Object) context, format, args);
                } else {
                    this.DefaultLogHandler.LogFormat(LogType.Error, null, format, args);
                }
            }
        }

        private void CreateEntry(Level level, object context, StackTrace trace, string format, params object[] args) {
            if (level >= this.LogLevel) {
                string contextStr = String.Empty;
                if (this.VerboseContext) {
                    try {
                        if (context != null) {
                            //NetworkBehaviour contextNetBhv = context is NetworkBehaviour ? (NetworkBehaviour) context : null;
                            //GameObject contextGameObj = context is MonoBehaviour ? ((MonoBehaviour) context).gameObject : null;
                            //string className = context.GetType().Name;
                            //string netId = contextNetBhv != null ? String.Format(" (netId: {0})", contextNetBhv.netId) : String.Empty;
                            //string gameObj = contextGameObj != null ? String.Format(" (gobj: {0})", contextGameObj.name) : String.Empty;
                            //contextStr = String.Format("{0}{1}{2}", className, gameObj, netId);
                        }
                    } catch (MissingReferenceException) {
                        // Ignore this exception
                    }
                } else {
                    if (context != null) {
                        contextStr = context.ToString();
                    }
                }

                string message = args.Length > 0 ? String.Format(format, args) : format;

                Record record = new Record(DateTime.Now, level, this.LoggerName, contextStr, message, trace);
                this.Records.Add(record);
                this.OnRecordCreated.Invoke(record);
            }
        }
    }
}