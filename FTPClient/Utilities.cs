using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using FTPClient.Enumerations;
using FTPClient.Structures;

namespace FTPClient
{
    static class Utilities
    {
        public static bool IsInteger(string toCheck)
        {
            int val;
            return int.TryParse(toCheck, out val);
        }

        public static bool IsInteger(char toCheck)
        {
            int val;
            return int.TryParse(toCheck.ToString(), out val);
        }
        public static bool HasIntervalExpired(this DateTime lastActivity, DateTime now, int interval)
        {
            return interval > 0 && now.Subtract(lastActivity).TotalMilliseconds > interval;
        }
        public static SslProtocols GetSslProtocols(ConnectionDetails conn)
        {
            switch (conn.Security)
            {
                case SSLType.AuthSSL:
                    return SslProtocols.Ssl3;
                case SSLType.AuthTLSv10:
                    return SslProtocols.Ssl3 | SslProtocols.Tls;
                case SSLType.AuthTLSv11:
                    return SslProtocols.Ssl3 | SslProtocols.Tls11;
                case SSLType.AuthTLSv12:
                    return SslProtocols.Ssl3 | SslProtocols.Tls12;
                case SSLType.ImplicitSSL:
                    return SslProtocols.Ssl2;
                case SSLType.Unsecure:
                    return SslProtocols.None;
                default:
                    return SslProtocols.Default;
            }
        }

        public static string GetSslProtocolsString(SslProtocols prot)
        {
            var mes = "";
            switch (prot)
            {
                case SslProtocols.None:
                    return "None";
                case SslProtocols.Ssl2:
                    return "Implicit SSL";
                case SslProtocols.Ssl3:
                    return "Auth SSL";
                case SslProtocols.Tls:
                    return "Auth TLS v1.0";
                case SslProtocols.Tls11:
                    return "Auth TLS v1.1";
                case SslProtocols.Tls12:
                    return "Auth TLS v1.2";
                case SslProtocols.Default:
                    return "Unknown";
            }

            return mes;
        }
        public static TEnum? ToNullableEnum<TEnum>(this string operand) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            TEnum enumOut;
            if (Enum.TryParse(operand, true, out enumOut))
            {
                return enumOut;
            }

            return null;
        }
        public static TEnum? ToNullableEnum<TEnum>(this int operand) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (Enum.IsDefined(typeof(TEnum), operand))
            {
                return (TEnum)(object)operand;
            }

            return null;
        }
    }
}
