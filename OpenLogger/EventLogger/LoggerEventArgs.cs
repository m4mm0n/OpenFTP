using System;

namespace OpenLogger.EventLogger
{
    public class LoggerEventArgs : EventArgs
    {
        public string Log { get; set; }

        public LoggerEventArgs(string format, params object[] param)
        {
            Log = string.Format(format, param);
        }
    }
}
