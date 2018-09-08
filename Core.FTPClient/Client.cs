using OpenLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenLogger.Enumerations;

namespace Core.FTPClient
{
    public class Client
    {
        //THIS LOGGER IS INTERNAL USE ONLY!
        private Logger Log = new Logger(LoggerType.Console, "Core.FTPClient", true);
        private const string clientName = "OpenFTP";
        private NetworkCredential connectionCredentials;
    }
}
