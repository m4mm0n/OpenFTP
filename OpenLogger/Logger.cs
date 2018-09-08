﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using OpenLogger.Enumerations;

namespace OpenLogger
{
    public class Logger : IDisposable
    {
        private readonly string datetimeFormat;
        private readonly string logFilename;
        private readonly LoggerType loggerType;
        private readonly string logOwner;
        private readonly bool useVerbose;

        public Logger(LoggerType loggingType, string loggerOwner, bool isVerbose = false)
        {
            loggerType = loggingType;
            logOwner = loggerOwner;// ?? "";
            useVerbose = isVerbose;

            if ((loggingType == LoggerType.File) | (loggingType == LoggerType.Console_File))
            {
                if (!Directory.Exists("\\LOGS\\"))
                    Directory.CreateDirectory("LOGS");

                datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
                logFilename = "LOGS\\" + loggerOwner + ".log";

                var logHeader = string.Format("[{0}]{1}", loggerOwner, Environment.NewLine);
                if (!File.Exists(logFilename))
                    WriteLine(logHeader, false);
                else
                    WriteLine(Environment.NewLine + Environment.NewLine + "[START NEW LOGGING PROCESS: " +
                              DateTime.Now.ToString(datetimeFormat) + "]" + Environment.NewLine);
            }
        }

        public void Dispose()
        {
            if ((loggerType == LoggerType.File) | (loggerType == LoggerType.Console_File))
                WriteLine("[END OF LOG]" + Environment.NewLine + Environment.NewLine);
        }

        public void Start()
        {
            Log(LogType.Verbose, new StackFrame(1).GetMethod().Name);
        }

        public void Log(LogType lType, string format, params object[] args)
        {
            WriteFormattedLog(lType, string.Format(format, args));
        }

        public void Log(string format, params object[] args)
        {
            Log(LogType.Normal, format, args);
        }

        public void Exception(Exception exception)
        {
            Exception(exception, "");
        }

        public void Exception(Exception exception, string format, params object[] args)
        {
            var ex = $"Exception: {exception}\r\n";
            Log(LogType.Exception, ex + format, args);
        }

        public void Verbose(string format, params object[] args)
        {
            Log(LogType.Verbose, format, args);
        }

        /// <summary>
        ///     Logs the input
        ///     Using <see cref="System.Diagnostics.Debug" /> to print directly
        ///     to the debugger aswell...
        /// </summary>
        /// <param name="format">Pre-formatted text</param>
        /// <param name="args">Optionally set arguments if formatted characters is used</param>
        public void Debug(string format, params object[] args)
        {
#if DEBUG
            Log(LogType.Debug, format, args);
            System.Diagnostics.Debug.Print(format, args);
#endif
        }


        private void WriteFormattedLog(LogType level, string text)
        {
            switch (loggerType)
            {
                case LoggerType.Console:
                    if (level == LogType.Verbose && useVerbose)
                        WriteConsole(level, text);
                    if (level == LogType.Debug)
                    {
#if DEBUG
                        WriteConsole(level, text);
#endif
                    }
                    else
                    {
                        WriteConsole(level, text);
                    }

                    break;
                case LoggerType.File:
                    WriteFile(level, text);
                    break;
                case LoggerType.Console_File:
                    if(level == LogType.Verbose && useVerbose)
                        WriteConsole(level, text);
                    if (level == LogType.Debug)
                    {
#if DEBUG
                        WriteConsole(level, text);
#endif
                    }
                    else
                    {
                        WriteConsole(level, text);
                    }

                    WriteFile(level, text);
                    break;
            }
        }

        private void WriteFile(LogType level, string text)
        {
            string pretext;

            switch (level)
            {
                case LogType.Normal:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [INFO]    ";
                    break;
                case LogType.Debug:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [DEBUG]   ";
                    break;
                case LogType.Warning:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [WARNING] ";
                    break;
                case LogType.Error:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [ERROR]   ";
                    break;
                case LogType.Critical:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [CRITICAL]   ";
                    break;
                case LogType.Exception:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [EXCEPTION]   ";
                    break;
                case LogType.Failure:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [FAILURE]   ";
                    break;
                case LogType.Success:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [SUCCESS]   ";
                    break;
                case LogType.Verbose:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [VERBOSE]   ";
                    break;
                default:
                    pretext = "";
                    break;
            }

            WriteLine(pretext + text);
        }

        private void WriteConsole(LogType level, string text)
        {
            var orgCol = Console.ForegroundColor;

            switch (level)
            {
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("SUCCESS");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Normal:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") ");
                    Console.Write(" -> ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("WARNING");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Critical:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("CRITICAL");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Verbose:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("VERBOSE");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("DEBUG");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("ERROR");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Exception:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("EXCEPTION");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Failure:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    Console.Write(" (");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(logOwner);
                    Console.ForegroundColor = orgCol;
                    Console.Write(") [");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("FAILURE");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
            }
        }

        private void WriteLine(string text, bool append = true)
        {
            try
            {
                using (var writer = new StreamWriter(logFilename, append, Encoding.UTF8))
                {
                    if (text != "") writer.WriteLine(text);
                }
            }
            catch
            {
                //throw;
            }
        }
    }
}
