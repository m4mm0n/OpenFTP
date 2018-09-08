using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Configuration;
using System.Text;
using OpenLogger.EventLogger;

namespace OpenLogger.TraceListener
{
    public class LoggerTraceListener : System.Diagnostics.TraceListener
    {
        public event EventHandler<LoggerEventArgs> OnTraceEvent;

        private bool hasBegunDispose = false;

        protected virtual void AddLog(string format, params object[] args)
        {
            OnTraceEvent?.Invoke(this, new LoggerEventArgs(format, args));
        }

        public override void Write(string message)
        {
            if (message.StartsWith("Command:") | message.StartsWith("Response:") | message.StartsWith("Status:") | !message.Trim().StartsWith("#") | !message.Contains("Disposing"))
                AddLog("[{0}] {1}", DateTime.Now.ToShortTimeString(), prepareMessage(message));
        }

        public override void WriteLine(string message)
        {
            if (message.StartsWith("Command:") | message.StartsWith("Response:") | message.StartsWith("Status:") | !message.Trim().StartsWith("#") | !message.Contains("Disposing"))
                AddLog("[{0}] {1}", DateTime.Now.ToShortTimeString(), prepareMessage(message));
        }

        private string prepareMessage(string toPrep)
        {
            var msg = "";
            var isError = false;
            var isWarning = false;

            if (Containz(toPrep, '\n'))
            {
                foreach (var s in toPrep.Split('\n'))
                {
                    if(s.StartsWith("#"))
                        continue;
                    var tmp = removePrefixes(s, out isError, out isWarning) + " ";
                    if (tmp.Length > 0)
                        msg += tmp;
                }
            }
            else
                msg = removePrefixes(toPrep, out isError, out isWarning);

            return isError ? "[ERROR!] " + msg : isWarning ? "[WARN!] " + msg : msg;
        }

        private string removePrefixes(string toFix, out bool isError, out bool isWarning)
        {
            isError = false;
            isWarning = false;

            if (toFix.StartsWith("Response:"))
                return toFix.Substring(9).Trim();
            else if (toFix.StartsWith("Command:"))
                return toFix.Substring(8).Trim();
            else if (toFix.StartsWith("Listing:"))
                return "";
            else if (toFix.StartsWith("Status:"))
                return toFix.Substring(7).Trim();
            else if (toFix.StartsWith("Error:"))
            {
                isError = true;
                return toFix.Substring(6).Trim();
            }
            else if (toFix.StartsWith("Warning:"))
            {
                isWarning = true;
                return toFix.Substring(8).Trim();
            }

            return toFix;
        }

        private bool Containz(string toCheck, char hasThis)
        {
            foreach (var ch in toCheck.ToCharArray())
            {
                if (ch == hasThis)
                    return true;
            }

            return false;
        }
    }
}
