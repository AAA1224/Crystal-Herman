using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace KoboldTools.Logging {
    [Serializable]
    public struct Record {
        public DateTime Timestamp;
        public KoboldTools.Logging.Level Level;
        public string LoggerName;
        public string Context;
        public string Message;
        public StackTrace Trace;

        public Record(DateTime timestamp, KoboldTools.Logging.Level level, string origin, string context, string message, StackTrace trace) {
            this.Timestamp = timestamp;
            this.Level = level;
            this.LoggerName = origin;
            this.Context = context;
            this.Message = message;
            this.Trace = trace;
        }

        public override string ToString() {
            return this.ToHumanReadableString(false, false, false, true, true);
        }

        public string ToHumanReadableString(bool showTime, bool showLevel, bool showOrigin, bool showContext, bool showTrace) {
            string timestamp = showTime ? String.Format("[{0:yyyy-MM-dd HH:mm:ss z}]", this.Timestamp) : String.Empty;
            string level = showLevel ? String.Format("[{0}]", this.Level) : String.Empty;
            string origin = showOrigin ? String.Format("[{0}]", this.LoggerName) : String.Empty;
            string target = showContext ? String.Format("[{0}]", this.Context) : String.Empty;
            string trace = (showTrace && (this.Trace != null)) ? String.Format("\n{0}", this.TraceToString(this.Trace)) : String.Empty;

            return String.Format("{0}{1}{2}{3} {4}{5}", timestamp, level, origin, target, this.Message, trace);
        }

        public string ToMachineReadableString() {
            return "JsonConvert.SerializeObject(this)";
        }

        private string TraceToString(StackTrace trace) {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < trace.FrameCount; i++) {
                StackFrame frame = trace.GetFrame(i);
                MethodBase method = frame.GetMethod();
                ParameterInfo[] methodParameters = method.GetParameters();
                StringBuilder argBuf = new StringBuilder();
                argBuf.Append("(");
                for (int j = 0; j < methodParameters.Length; j++) {
                    if (j == methodParameters.Length - 1) {
                        argBuf.AppendFormat("{0}", methodParameters[j]);
                    } else {
                        argBuf.AppendFormat("{0}, ", methodParameters[j]);
                    }
                }
                argBuf.Append(")");
                string fileName = Path.GetFileName(frame.GetFileName());
                buf.AppendFormat("{0}{1} (at {2} line {3})\n", method.Name, argBuf, fileName, frame.GetFileLineNumber());
            }

            return buf.ToString();
        }
    }
}