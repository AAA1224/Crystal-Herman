using System;

namespace KoboldTools.Logging
{
    public class LoggerWriteConsole : VCBehaviour<Logger> {
        public bool ThrowOnExceptionRecord = false;
        public bool DisplayTime = false;
        public bool DisplayLevel = false;
        public bool DisplayOrigin = false;
        public bool DisplayContext = true;
        public bool DisplayTrace = true;

        public override void onModelChanged() {
            this.model.OnRecordCreated.AddListener(this.OnRecordCreated);
        }

        public override void onModelRemoved() {
            this.model.OnRecordCreated.RemoveListener(this.OnRecordCreated);
        }

        private void OnRecordCreated(Record record) {
            if (!this.model.RedirectUnityLogs) {
                if (record.Level == Level.DEBUG) {
                    UnityEngine.Debug.Log(record.ToHumanReadableString(this.DisplayTime, this.DisplayLevel, this.DisplayOrigin, this.DisplayContext, this.DisplayTrace));
                } else if (record.Level == Level.INFO) {
                    UnityEngine.Debug.Log(record.ToHumanReadableString(this.DisplayTime, this.DisplayLevel, this.DisplayOrigin, this.DisplayContext, this.DisplayTrace));
                } else if (record.Level == Level.WARNING) {
                    UnityEngine.Debug.LogWarning(record.ToHumanReadableString(this.DisplayTime, this.DisplayLevel, this.DisplayOrigin, this.DisplayContext, this.DisplayTrace));
                } else if (record.Level == Level.ERROR) {
                    UnityEngine.Debug.LogError(record.ToHumanReadableString(this.DisplayTime, this.DisplayLevel, this.DisplayOrigin, this.DisplayContext, this.DisplayTrace));
                } else if (record.Level == Level.EXCEPTION) {
                    if (this.ThrowOnExceptionRecord) {
                        throw new Exception(record.ToHumanReadableString(this.DisplayTime, this.DisplayLevel, this.DisplayOrigin, this.DisplayContext, this.DisplayTrace));
                    } else {
                        UnityEngine.Debug.LogError(record.ToHumanReadableString(this.DisplayTime, this.DisplayLevel, this.DisplayOrigin, this.DisplayContext, this.DisplayTrace));
                    }
                }
            }
        }

    }
}