using System;

namespace Core.FTP.Helpers
{
    public class SecurityNotAvailableException : Exception
    {
        public SecurityNotAvailableException(string msg, Exception innerEx) : base(msg, innerEx)
        { }

        public SecurityNotAvailableException(string msg) : base(msg)
        { }
    }
}
