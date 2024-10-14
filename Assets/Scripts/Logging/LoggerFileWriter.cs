using System.IO;
using UnityEngine;
using System.Linq;

namespace KoboldTools
{
    public class LoggerFileWriter : ViewController<ILogger>
    {
        public string logFileName = "data";
        public string logFileEnding = ".log";
        public bool addLineTimestamp = false;
        public bool addDateToFileName = true;

        // time format for log line
        public string lineTimestampFormat = "HH:mm:ss";
        // date and time format for file header
        public string dateTimeFormat = "yyyy/MM/dd HH:mm:ss";
        // date and time format for file name
        public string fileNameDateTimeFormat = "yyyy-MM-dd_HH-mm-ss";

        public string[] identifier;

        private string _logDirectory;
        private string _logFilePath;
        private System.DateTime _logFileCreationTime;

        public override void onModelChanged()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                // register event
                model.eDataChanged.AddListener(onDataChanged);
                // prepare log data for log file
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _logDirectory = Application.persistentDataPath + "/koboldgames/logs";
                }
                else
                {
                    _logDirectory = Application.dataPath + "/logs";
                }

                // set log file date
                _logFileCreationTime = System.DateTime.Now;

                //check if date is needed
                string _logFileCreationDate = "";
                if (addDateToFileName)
                {
                    _logFileCreationDate = "_" + _logFileCreationTime.ToString(fileNameDateTimeFormat);
                }
                //set path and name
                _logFilePath = _logDirectory + "/" + sanitizeFileName(logFileName) + _logFileCreationDate + logFileEnding;
            }
        }

        private void onDataChanged()
        {
            ILogEntry log = model.data[model.data.Count - 1];
            
            if (identifier.Any(i=>i==log.identifier))
                {
                string output = "";
                if (addLineTimestamp)
                {
                    output = "[" + log.timestamp.ToString(lineTimestampFormat) + "] ";
                }
                output += log.getContent().ToString();
                writeLine(output);
            }
        }

        private void writeLine(string line)
        {
            //check log file access
            if (checkFileAccess())
            {
                try
                {
                    // write line to file
                    using (StreamWriter sw = File.AppendText(_logFilePath))
                    {
                        sw.WriteLine(line);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Could not write to file '" + _logFilePath + "'. " + e.ToString());
                }
            }
        }

        private bool checkFileAccess()
        {
            //check if directory exist
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Directory '" + _logDirectory + "' for log files could not be created. " + e.ToString());
                return false;
            }

            try
            {
                if (!File.Exists(_logFilePath))
                {
                    //create a file and write date
                    using (StreamWriter sw = File.CreateText(_logFilePath))
                    {
                        sw.WriteLine("Logfile created: " + _logFileCreationTime.ToString(dateTimeFormat));
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("File '" + _logFilePath + "' could not be created. " + e.ToString());
                return false;
            }

            return true;
        }

        private string sanitizeFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
