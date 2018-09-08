using System;
using System.Net;
using System.Text;

namespace Core.FTPClient.Structures
{
    /// <summary>
    /// Safely stored username & password in memory using simple Base-64
    /// </summary>
    public struct UserCredentials
    {
        public string UserName => Encoding.Unicode.GetString(Convert.FromBase64String(uName));
        public string PassWord => Encoding.Unicode.GetString(Convert.FromBase64String(uPass));

        private string uName;
        private string uPass;
        /// <summary>
        /// Leave with empty "userName" and empty "passWord" for anonymous credentials...
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        public UserCredentials(string userName, string passWord)
        {
            if (userName == string.Empty && passWord == String.Empty)
            {
                uName = Convert.ToBase64String(Encoding.Unicode.GetBytes("anonymous"));
                uPass = Convert.ToBase64String(Encoding.Unicode.GetBytes("anonymous@password.no"));
            }
            else
            {
                uName = Convert.ToBase64String(Encoding.Unicode.GetBytes(userName));
                uPass = Convert.ToBase64String(Encoding.Unicode.GetBytes(passWord));
            }
        }

        public NetworkCredential ToNetworkCredential() => Utils.GetNetworkCredentials(this);
    }
}
