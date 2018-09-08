using System;
using System.Text;

namespace FTPClient.Structures
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

        public UserCredentials(string userName, string passWord)
        {
            uName = Convert.ToBase64String(Encoding.Unicode.GetBytes(userName));
            uPass = Convert.ToBase64String(Encoding.Unicode.GetBytes(passWord));
        }
    }
}
