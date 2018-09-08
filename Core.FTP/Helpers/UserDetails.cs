using System;
using System.Net;
using System.Text;

namespace Core.FTP.Helpers
{
    /// <summary>
    /// Safely stored username & password in memory using simple Base-64
    /// </summary>
    public class UserDetails
    {
        public string UserName => Encoding.Unicode.GetString(Convert.FromBase64String(uName));
        public string PassWord => Encoding.Unicode.GetString(Convert.FromBase64String(uPass));

        private string uName;
        private string uPass;

        public UserDetails(string userName, string passWord)
        {
            uName = Convert.ToBase64String(Encoding.Unicode.GetBytes(userName));
            uPass = Convert.ToBase64String(Encoding.Unicode.GetBytes(passWord));
        }

        public NetworkCredential GetNetworkCredentials => new NetworkCredential(UserName, PassWord);
    }
}
