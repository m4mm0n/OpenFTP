using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Core.Utils.FTPUtils;
using OpenLogger;
using OpenLogger.Enumerations;

namespace Core.Utils
{
    public static class Extensions
    {
        static Logger Log = new Logger(LoggerType.Console_File, "Core.Utils.Extensions", true);
        /// <summary>
        /// https://stackoverflow.com/a/2579817
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static string GetTypeName(Type t)
        {
            Log.Start();

            if (!t.IsGenericType) return t.Name;
            if (t.IsNested && t.DeclaringType.IsGenericType)
            {
                throw new NotImplementedException();
            }
            var txt = t.Name.Substring(0, t.Name.IndexOf('`')) + "<";
            var cnt = 0;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (cnt > 0) txt += ", ";
                txt += GetTypeName(arg);
                cnt++;
            }
            return txt + ">";
        }

        public static bool IsInteger(this object toCheck)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(IsInteger) Type: {0} => Value: {1}", GetTypeName(toCheck.GetType()), toCheck);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(IsInteger) Type: {0} => Value: {1}", GetTypeName(toCheck.GetType()), toCheck);
#endif
            int val;
            if(toCheck is string)
                return int.TryParse((string)toCheck, out val);
            else if(toCheck is char)
                return int.TryParse(((char)toCheck).ToString(), out val);
            else if (toCheck is byte)
                return true;

            return false;
        }

        public static bool HasIntervalExpired(this DateTime lastActivity, DateTime now, int interval)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(HasIntervalExpired) lastActivity: {0}, now: {1}, interval: {2}", lastActivity.ToString(),
                now.ToString(), interval);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(HasIntervalExpired) lastActivity: {0}, now: {1}, interval: {2}", lastActivity.ToString(),
                now.ToString(), interval);
#endif

            return interval > 0 && now.Subtract(lastActivity).TotalMilliseconds > interval;
        }

        public static TEnum? ToNullableEnum<TEnum>(this string operand) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Log.Start();
#if DEBUG
            Log.Debug("(ToNullableEnum) Enum: {0} => Operand: {1}", GetTypeName(typeof(TEnum)), operand);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(ToNullableEnum) Enum: {0} => Operand: {1}", GetTypeName(typeof(TEnum)), operand);
#endif

            TEnum enumOut;
            if (Enum.TryParse(operand, true, out enumOut))
            {
                return enumOut;
            }

            return null;
        }

        public static TEnum? ToNullableEnum<TEnum>(this int operand) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Log.Start();
#if DEBUG
            Log.Debug("(ToNullableEnum) Enum: {0} => Operand: {1}", GetTypeName(typeof(TEnum)), operand);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(ToNullableEnum) Enum: {0} => Operand: {1}", GetTypeName(typeof(TEnum)), operand);
#endif

            if (Enum.IsDefined(typeof(TEnum), operand))
            {
                return (TEnum)(object)operand;
            }

            return null;
        }

        public static string GetSslProtocolsString(this SslProtocols prot)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetSslProtocolsString) Protocol Value: {0}", prot.ToString());
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetSslProtocolsString) Protocol Value: {0}", prot.ToString());
#endif

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

        public static string GetSslErrorsString(this SslPolicyErrors spe)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetSslErrorsString) Policy Value: {0}", spe.ToString());
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetSslErrorsString) Policy Value: {0}", spe.ToString());
#endif
            switch (spe)
            {
                case SslPolicyErrors.RemoteCertificateChainErrors:
                    return "Remote Certificate Chain Errors";
                case SslPolicyErrors.RemoteCertificateNameMismatch:
                    return "Remote Certificate Name Mismatch";
                case SslPolicyErrors.RemoteCertificateNotAvailable:
                    return "Remote Certificate Not Available";
                default:
                case SslPolicyErrors.None:
                    return "";
            }
        }

        public static SslProtocols GetSslProtocols(this SSLType ste)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetSslProtocols) SSL Type Value: {0}", ste.ToString());
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetSslProtocols) SSL Type Value: {0}", ste.ToString());
#endif
            switch (ste)
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

        public static SSLType GetSSLType(this SslProtocols spe)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetSSLType) SSL Protocol Value: {0}", spe.ToString());
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetSSLType) SSL Protocol Value: {0}", ste.ToString());
#endif
            switch (spe)
            {
                case SslProtocols.Ssl3 | SslProtocols.Tls:
                    return SSLType.AuthTLSv10;
                case SslProtocols.Ssl3 | SslProtocols.Tls11:
                    return SSLType.AuthTLSv11;
                case SslProtocols.Ssl3 | SslProtocols.Tls12:
                    return SSLType.AuthTLSv12;
                case SslProtocols.Ssl3:
                    return SSLType.AuthSSL;
                case SslProtocols.Ssl2:
                    return SSLType.ImplicitSSL;
                default:
                case SslProtocols.None:
                    return SSLType.Unsecure;

            }
        }

        public static object GetField(this object obj, string fieldName)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetField) Object Type: {0} => FieldName: {1}", GetTypeName(obj.GetType()), fieldName);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetField) Object Type: {0} => FieldName: {1}", GetTypeName(obj.GetType()), fieldName);
#endif

            var tp = obj.GetType();
            var info = GetAllFields(tp).Where(f => f.Name == fieldName).Single();
            return info.GetValue(obj);
        }
        public static void SetField(this object obj, string fieldName, object value)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(SetField) Object Type: {0} => FieldName: {1}, Value Type: {2}", GetTypeName(obj.GetType()),
                fieldName, GetTypeName(value.GetType()));
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(SetField) Object Type: {0} => FieldName: {1}, Value Type: {2}", GetTypeName(obj.GetType()),
                fieldName, GetTypeName(value.GetType()));
#endif

            var tp = obj.GetType();
            var info = GetAllFields(tp).Where(f => f.Name == fieldName).Single();
            info.SetValue(obj, value);
        }
        public static object GetStaticField(this Assembly assembly, string typeName, string fieldName)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetStaticField) Assembly Name: {0} => TypeName: {1}, FieldName: {2}", assembly.FullName,
                typeName, fieldName);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetStaticField) Assembly Name: {0} => TypeName: {1}, FieldName: {2}", assembly.FullName,
                typeName, fieldName);
#endif

            var tp = assembly.GetType(typeName);
            var info = GetAllFields(tp).Where(f => f.IsStatic).Where(f => f.Name == fieldName).Single();
            return info.GetValue(null);
        }
        public static object GetProperty(this object obj, string propertyName)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetProperty) Object Type: {0} => PropertyName: {1}", GetTypeName(obj.GetType()), propertyName);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetProperty) Object Type: {0} => PropertyName: {1}", GetTypeName(obj.GetType()), propertyName);
#endif
            var tp = obj.GetType();
            var info = GetAllProperties(tp).Where(f => f.Name == propertyName).Single();
            return info.GetValue(obj, null);
        }
        public static object CallMethod(this object obj, string methodName, params object[] prm)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(CallMethod) Object Type: {0} => MethodName: {1}, Params Count: {2}", GetTypeName(obj.GetType()),
                methodName, prm.Length);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(CallMethod) Object Type: {0} => MethodName: {1}, Params Count: {2}", GetTypeName(obj.GetType()),
                methodName, prm.Length);
#endif
            var tp = obj.GetType();
            var info = GetAllMethods(tp).Where(f => f.Name == methodName && f.GetParameters().Length == prm.Length).Single();
            object rez = info.Invoke(obj, prm);
            return rez;
        }
        public static object NewInstance(this Assembly assembly, string typeName, params object[] prm)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(NewInstance) Assembly Name: {0} => TypeName: {1}, Params Count: {2}", assembly.FullName,
                typeName, prm.Length);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(NewInstance) Assembly Name: {0} => TypeName: {1}, Params Count: {2}", assembly.FullName,
                typeName, prm.Length);
#endif
            var tp = assembly.GetType(typeName);
            var info = tp.GetConstructors().Where(f => f.GetParameters().Length == prm.Length).Single();
            object rez = info.Invoke(prm);
            return rez;
        }
        public static object InvokeStaticMethod(this Assembly assembly, string typeName, string methodName, params object[] prm)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(InvokeStaticMethod) Assembly Name: {0} => TypeName: {1}, MethodName: {2}, Params Count: {3}",
                assembly.FullName, typeName, methodName, prm.Length);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(InvokeStaticMethod) Assembly Name: {0} => TypeName: {1}, MethodName: {2}, Params Count: {3}",
                assembly.FullName, typeName, methodName, prm.Length);
#endif
            var tp = assembly.GetType(typeName);
            var info = GetAllMethods(tp).Where(f => f.IsStatic).Where(f => f.Name == methodName && f.GetParameters().Length == prm.Length).Single();
            object rez = info.Invoke(null, prm);
            return rez;
        }
        public static object GetEnumValue(this Assembly assembly, string typeName, int value)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetEnumValue) Assembly Name: {0} => TypeName: {1}, Value: {2}", assembly.FullName, typeName,
                value);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetEnumValue) Assembly Name: {0} => TypeName: {1}, Value: {2}", assembly.FullName, typeName,
                value);
#endif
            var tp = assembly.GetType(typeName);
            object rez = Enum.ToObject(tp, value);
            return rez;
        }
        private static IEnumerable<FieldInfo> GetAllFields(this Type t)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetAllFields) Type Name: {0}", GetTypeName(t));
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetAllFields) Type Name: {0}", GetTypeName(t));
#endif
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }
        private static IEnumerable<PropertyInfo> GetAllProperties(this Type t)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetAllProperties) Type Name: {0}", GetTypeName(t));
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetAllProperties) Type Name: {0}", GetTypeName(t));
#endif
            if (t == null)
                return Enumerable.Empty<PropertyInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetProperties(flags).Concat(GetAllProperties(t.BaseType));
        }
        private static IEnumerable<MethodInfo> GetAllMethods(this Type t)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(GetAllMethods) Type Name: {0}", GetTypeName(t));
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(GetAllMethods) Type Name: {0}", GetTypeName(t));
#endif
            if (t == null)
                return Enumerable.Empty<MethodInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return t.GetMethods(flags).Concat(GetAllMethods(t.BaseType));
        }
    }
}
