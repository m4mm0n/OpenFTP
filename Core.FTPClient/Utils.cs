using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.FTPClient.Structures;

namespace Core.FTPClient
{
    internal class Utils
    {
        public static bool IsInt32(object toParse)
        {
            Int32 a = -1;
            return toParse is string && Int32.TryParse((string) toParse, out a);
        }

        public static NetworkCredential GetNetworkCredentials(UserCredentials uc)
        {
            return new NetworkCredential(uc.UserName, uc.PassWord);
        }
    }
}
