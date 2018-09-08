using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.FTP.Client.Stream;
using Core.FTP.Helpers;
using Core.Utils;
using Core.Utils.FTPUtils;
using OpenLogger;
using OpenLogger.Enumerations;
using OpenLogger.EventLogger;

namespace Core.FTP.Client
{
    public class FTPClient : IDisposable
    {
        #region Logging
        private Logger Log = new Logger(LoggerType.Console_File, "Core.FTP.Client.FTPClient", true);

        /// <summary>
        /// Must be initiated
        /// </summary>
        //public event EventHandler<LoggerEventArgs> OnGeneralAction;
        /// <summary>
        /// Must be initiated
        /// </summary>
        //public event EventHandler<LoggerEventArgs> OnSystemAction;

        public event EventHandler<ActionEventArgs> OnFtpAction;

        //protected virtual void SystemAction(LoggerEventArgs e)
        //{
        //    OnSystemAction?.Invoke(this, e);
        //}

        //protected virtual void GeneralAction(LoggerEventArgs e)
        //{
        //    OnGeneralAction?.Invoke(this, e);
        //}
        protected virtual void SystemActionLog(ActionEventArgs e)
        {
#if DEBUG
            Log.Debug(string.Format("(OnFtpAction){0}Reply: {1}{0}Message: {2}{0}Progress: {3}%", Environment.NewLine,
                e.RecievedReply?.ToString(), e.Message, e.CurrentPercent.ToString("000.00")));
#endif
            OnFtpAction?.Invoke(this, e);
        }

        private void ActionLog(ActionEventType actionType, ActionReply reply, long maxProgress, long curProgress)
        {
            var args = new ActionEventArgs(actionType, reply, "", maxProgress, curProgress);
            SystemActionLog(args);
        }

        private void ActionLog(ActionEventType actionType, ActionReply reply)
        {
            ActionLog(actionType, reply, 0, 0);
        }

        private void ActionLog(ActionReply reply)
        {
            ActionLog(reply.Success ? ActionEventType.Success : ActionEventType.Failure, reply);
        }

        private void ActionLog(ActionEventType actionType, string message, long maxProgress, long curProgress)
        {
            var args = new ActionEventArgs(actionType, null, message, maxProgress, curProgress);
            SystemActionLog(args);
        }

        private void ActionLog(ActionEventType actionType, string format, params object[] param)
        {
            ActionLog(actionType, string.Format(format, param), 0, 0);
        }

//        private void SysLog(string format, params object[] param)
//        {
//#if DEBUG
//            Log.Debug(string.Format("(SystemLog) {0}", format), param);
//#endif
//            var args = new LoggerEventArgs(string.Format("[{0}] {1}", DateTime.Now.ToShortTimeString(), format), param);
//            SystemAction(args);
//        }

//        private void SysLog(ActionReply mes)
//        {
//#if DEBUG
//            Log.Debug(string.Format("(SystemLog) [ActionReply]{0}{1}", Environment.NewLine, mes.ToString()));
//#endif
//            LoggerEventArgs args = null;
//            args = new LoggerEventArgs(string.Format("[{0}] {1} {2} ({3})", DateTime.Now.ToShortTimeString(),
//                mes.Code, mes.Message, mes.InfoMessages));
//        }

        #endregion

        public FTPClient()
        {
            Log.Start();
            //Empty constructor to ensure events are initiated before any actual action takes place!
        }

        ~FTPClient()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (m_lock)
            {
                if(isDisposed)
                    return;
            }
        }

        public virtual void Connect()
        {
            Log.Start();

            ActionReply reply = new ActionReply();
            lock (m_lock)
            {
                if(isDisposed)
                    throw new ApplicationException("This client has been disposed!");

                if (m_stream == null)
                {
                    m_stream = new SocketStream(ConnectionInfo.Security);
                }
                else
                {
                    if (IsConnected)
                        Disconnect();
                }

                if (ConnectionInfo.Hostname == null)
                    throw new MissingPrimaryKeyException("Hostname is empty!");
                if (UserInfo == null)
                    throw new MissingPrimaryKeyException("No user information has been set!");

                m_stream.ConnectTimeout = ConnectionInfo.TimeoutMilliseconds;
                m_stream.Connect(ConnectionInfo.Hostname, ConnectionInfo.Port);
                m_stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive,
                    ConnectionInfo.KeepAlive);

                if (ConnectionInfo.Security != SSLType.Unsecure)
                {
                    switch (ConnectionInfo.Security)
                    {
                        case SSLType.AuthSSL:
                            reply = SendCommand("AUTH SSL");
                            if(reply.Success)
                                m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3);
                            //else
                            //    throw new SecurityNotAvailableException("SSL call failed!");
                            break;
                        case SSLType.AuthTLSv10:
                            reply = SendCommand("AUTH TLS");
                            if(reply.Success)
                                m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3 | SslProtocols.Tls);
                            //else
                            //    throw new SecurityNotAvailableException("TLS call failed!");
                            break;
                        case SSLType.AuthTLSv11:
                            reply = SendCommand("AUTH TLS");
                            if (reply.Success)
                                m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3 | SslProtocols.Tls11);
                            //else
                            //    throw new SecurityNotAvailableException("TLS call failed!");
                            break;
                        case SSLType.AuthTLSv12:
                            reply = SendCommand("AUTH TLS");
                            if (reply.Success)
                                m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3 | SslProtocols.Tls12);
                            //else
                            //    throw new SecurityNotAvailableException("TLS call failed!");
                            break;
                        case SSLType.ImplicitSSL:
                            m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl2);
                            break;
                    }

                    ActionLog(reply);
                    if (!reply.Success)
                    {
                        Disconnect();
                        return;
                    }
                }

                Handshake();
                LogIn();

                if (m_stream.IsEncrypted/* && DataConnectionEncryption*/)
                {
                    if (!(reply = SendCommand("PBSZ 0")).Success)
                    {
                        ActionLog(reply);
                        Disconnect();
                        return;
                        //throw new FtpCommandException(reply);
                    }

                    if (!(reply = SendCommand("PROT P")).Success)
                    {
                        ActionLog(reply);
                        Disconnect();
                        return;
                        //throw new FtpCommandException(reply);
                    }
                }

                if ((reply = SendCommand("FEAT")).Success && reply.InfoMessages != null)
                    GetServerOptions(reply);

                ActionLog(ActionEventType.None, reply);

                if (TextEncoding == Encoding.ASCII && HasFeature(ServerCapabilities.UTF8))
                {
                    TextEncoding = Encoding.UTF8;
                    reply = SendCommand("OPTS UTF8 ON");
                    ActionLog(reply);
                }

                if ((reply = SendCommand("SYST")).Success)
                {
                    ServerSystem = reply.Message;
                    ActionLog(reply);
                }

                if (m_stream.IsEncrypted && ConnectionInfo.Security == SSLType.Unsecure)
                {
                    if (!(reply = SendCommand("CCC")).Success)
                    {
                        ActionLog(reply);
                        Disconnect();
                        return;
                    }
                    else
                    {
                        m_stream.DeactivateEncryption();
                        ReadStaleData(false, true);
                    }
                }

                //list whatever files and shit!
            }
        }

        public virtual async Task ConnectAsync()
        {
            Log.Start();
            ActionReply reply = new ActionReply();

            if (isDisposed)
                throw new ApplicationException("This client has been disposed!");

            if (m_stream == null)
            {
                m_stream = new SocketStream(ConnectionInfo.Security);
            }
            else
            {
                if (IsConnected)
                    await DisconnectAsync();
            }

            if (ConnectionInfo.Hostname == null)
                throw new MissingPrimaryKeyException("Hostname is empty!");
            if (UserInfo == null)
                throw new MissingPrimaryKeyException("No user information has been set!");

            m_stream.ConnectTimeout = ConnectionInfo.TimeoutMilliseconds;
            m_stream.Connect(ConnectionInfo.Hostname, ConnectionInfo.Port);
            m_stream.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive,
                ConnectionInfo.KeepAlive);

            if (ConnectionInfo.Security != SSLType.Unsecure)
            {
                switch (ConnectionInfo.Security)
                {
                    case SSLType.AuthSSL:
                        reply = await SendCommandAsync("AUTH SSL");
                        if (reply.Success)
                            m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3);
                        //else
                        //    throw new SecurityNotAvailableException("SSL call failed!");
                        break;
                    case SSLType.AuthTLSv10:
                        reply = await SendCommandAsync("AUTH TLS");
                        if (reply.Success)
                            m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3 | SslProtocols.Tls);
                        //else
                        //    throw new SecurityNotAvailableException("TLS call failed!");
                        break;
                    case SSLType.AuthTLSv11:
                        reply = await SendCommandAsync("AUTH TLS");
                        if (reply.Success)
                            m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3 | SslProtocols.Tls11);
                        //else
                        //    throw new SecurityNotAvailableException("TLS call failed!");
                        break;
                    case SSLType.AuthTLSv12:
                        reply = await SendCommandAsync("AUTH TLS");
                        if (reply.Success)
                            m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl3 | SslProtocols.Tls12);
                        //else
                        //    throw new SecurityNotAvailableException("TLS call failed!");
                        break;
                    case SSLType.ImplicitSSL:
                        m_stream.ActivateEncryption(ConnectionInfo.Hostname, null, SslProtocols.Ssl2);
                        break;
                }

                ActionLog(reply);
                if (!reply.Success)
                {
                    await DisconnectAsync();
                    return;
                }
            }

            await HandshakeAsync();
            await LogInAsync();

            if (m_stream.IsEncrypted/* && DataConnectionEncryption*/)
            {
                if (!(reply = await SendCommandAsync("PBSZ 0")).Success)
                {
                    ActionLog(reply);
                    await DisconnectAsync();
                    return;
                    //throw new FtpCommandException(reply);
                }

                if (!(reply = await SendCommandAsync("PROT P")).Success)
                {
                    ActionLog(reply);
                    await DisconnectAsync();
                    return;
                    //throw new FtpCommandException(reply);
                }
            }

            if ((reply = await SendCommandAsync("FEAT")).Success && reply.InfoMessages != null)
                GetServerOptions(reply);

            ActionLog(ActionEventType.None, reply);

            if (TextEncoding == Encoding.ASCII && HasFeature(ServerCapabilities.UTF8))
            {
                TextEncoding = Encoding.UTF8;
                reply = await SendCommandAsync("OPTS UTF8 ON");
                ActionLog(reply);
            }

            if ((reply = await SendCommandAsync("SYST")).Success)
            {
                ServerSystem = reply.Message;
                ActionLog(reply);
            }

            if (m_stream.IsEncrypted && ConnectionInfo.Security == SSLType.Unsecure)
            {
                if (!(reply = await SendCommandAsync("CCC")).Success)
                {
                    ActionLog(reply);
                    await DisconnectAsync();
                    return;
                }
                else
                {
                    m_stream.DeactivateEncryption();
                    await ReadStaleDataAsync(false, true);
                }
            }

            //list whatever files and shit!
        }

        public virtual void Disconnect()
        {
            lock (m_lock)
            {
                if (m_stream != null && m_stream.IsConnected)
                {
                    try
                    {
                        ActionLog(SendCommand("QUIT"));
                    }
                    catch (SocketException sockex)
                    {
                        ActionLog(ActionEventType.CriticalError, "SocketException caught and discarded while closing control connection: " +
                               sockex.ToString());
                        //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): SocketException caught and discarded while closing control connection: " + sockex.ToString());
                    }
                    catch (IOException ioex)
                    {
                        ActionLog(ActionEventType.CriticalError, "IOException caught and discarded while closing control connection: " + ioex.ToString());
                        //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): IOException caught and discarded while closing control connection: " + ioex.ToString());
                    }
                    catch (Exception ftpex)
                    {
                        ActionLog(ActionEventType.CriticalError, "Exception caught and discarded while closing control connection: " + ftpex.ToString());
                        //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): FtpException caught and discarded while closing control connection: " + ftpex.ToString());
                    }
                    finally
                    {
                        m_stream.Close();
                    }
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (m_stream != null && m_stream.IsConnected)
            {
                try
                {
                    ActionLog(await SendCommandAsync("QUIT"));
                }
                catch (SocketException sockex)
                {
                    ActionLog(ActionEventType.CriticalError, "SocketException caught and discarded while closing control connection: " + sockex.ToString());
                    //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): SocketException caught and discarded while closing control connection: " + sockex.ToString());
                }
                catch (IOException ioex)
                {
                    ActionLog(ActionEventType.CriticalError, "IOException caught and discarded while closing control connection: " + ioex.ToString());
                    //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): IOException caught and discarded while closing control connection: " + ioex.ToString());
                }
                catch (Exception ftpex)
                {
                    ActionLog(ActionEventType.CriticalError, "Exception caught and discarded while closing control connection: " + ftpex.ToString());
                    //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "FtpClient.Disconnect(): FtpException caught and discarded while closing control connection: " + ftpex.ToString());
                }
                finally
                {
                    m_stream.Close();
                }
            }
        }

        protected virtual void LogIn()
        {
            ActionReply reply;
            if (!(reply = SendCommand("USER " + UserInfo.UserName)).Success)
            {
                ActionLog(reply);
            }
                //throw new IOException("Unable to logon with chosen username!",
                //    new IOException(string.Format("Code: {0}, Message: {1}", reply.Code, reply.ErrorMessage)));

            if (reply.Type == 2
                && !(reply = SendCommand("PASS " + UserInfo.PassWord)).Success)
            {
                ActionLog(reply);
                return;
            }
            ActionLog(reply);
            //throw new IOException("Unable to logon with chosen password!",
            //    new IOException(string.Format("Code: {0}, Message: {1}", reply.Code, reply.ErrorMessage)));
        }

        protected async Task LogInAsync()
        {
            ActionReply reply;

            if (!(reply = await SendCommandAsync("USER " + UserInfo.UserName)).Success)
            {
                ActionLog(reply);
            }
                //throw new IOException("Unable to logon with chosen username!",
                //    new IOException(string.Format("Code: {0}, Message: {1}", reply.Code, reply.ErrorMessage)));

            if (reply.Type == 2
                && !(reply = await SendCommandAsync("PASS " + UserInfo.PassWord)).Success)
            {
                ActionLog(reply);
                return;
            }

            ActionLog(reply);
            //throw new IOException("Unable to logon with chosen password!",
            //    new IOException(string.Format("Code: {0}, Message: {1}", reply.Code, reply.ErrorMessage)));
        }

        protected virtual void Handshake()
        {
            ActionReply reply;
            if (!(reply = GetReply()).Success)
            {
                if (reply.Code == null)
                {
                    ActionLog(ActionEventType.Error, "The connection was terminated before a greeting could be read.");
                    return;
                }
                else
                {
                    ActionLog(reply);
                    return;
                }
            }
            ActionLog(reply);
        }

        protected async Task HandshakeAsync()
        {
            ActionReply reply;
            if (!(reply = await GetReplyAsync()).Success)
            {
                if (reply.Code == null)
                {
                    ActionLog(ActionEventType.Error, "The connection was terminated before a greeting could be read.");
                    return;
                }
                else
                {
                    ActionLog(reply);
                    return;
                }
            }
            ActionLog(reply);
        }

        public ActionReply SendCommand(string cmd)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(SendCommand) {0}", cmd);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(SendCommand) {0}", cmd);
#endif

            ActionReply reply = new ActionReply();
            lock (m_lock)
            {
                if (!IsConnected)
                {
                    if (cmd == "QUIT")
                    {
                        reply.Code = "200";
                        reply.Message = "Connection already closed...";
                        return reply;
                    }

                    reply.Code = "530";
                    reply.Message = "Not connected!";
                    return reply;
                }

                var cmdToLog = cmd;
                if (cmd.StartsWith("PASS"))
                    cmdToLog = "PASS ***";

                ActionLog(ActionEventType.None, cmdToLog);

                m_stream.WriteLine(TextEncoding, cmd);
                reply = GetReply();
            }

            return reply;
        }
        public async Task<ActionReply> SendCommandAsync(string cmd)
        {
            Log.Start();
#if DEBUG
            Log.Debug("(SendCommandAsync) {0}", cmd);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose("(SendCommandAsync) {0}", cmd);
#endif

            ActionReply reply = new ActionReply();
            if (!IsConnected)
            {
                if (cmd == "QUIT")
                {
                    reply.Code = "200";
                    reply.Message = "Connection already closed...";
                    return reply;
                }

                reply.Code = "530";
                reply.Message = "Not connected!";
                return reply;
            }

            var cmdToLog = cmd;
            if (cmd.StartsWith("PASS"))
                cmdToLog = "PASS ***";

            ActionLog(ActionEventType.None, cmdToLog);

            m_stream.WriteLine(TextEncoding, cmd);
            reply = GetReply();


            return reply;
        }

        public ActionReply GetReply()
        {
            Log.Start();

            ActionReply reply = new ActionReply();
            var buf = "";
            lock (m_lock)
            {
                if (!IsConnected)
                {
                    ActionLog(ActionEventType.Error, "No connection available!");
                    return null;
                }
                    //throw new IOException("No connection available, so reply cannot be retrieved!");

                m_stream.ReadTimeout = ConnectionInfo.TimeoutMilliseconds;
                while ((buf = m_stream.ReadLine(TextEncoding)) != null)
                {
                    Match m;
                    if ((m = Regex.Match(buf, "^(?<code>[0-9]{3}) (?<message>.*)$")).Success)
                    {
                        reply.Code = m.Groups["code"].Value;
                        reply.Message = m.Groups["message"].Value;
                        break;
                    }
                    reply.InfoMessages += (buf + "\n");
                }

                // log multiline response messages
                if (reply.InfoMessages != null)
                {
                    reply.InfoMessages = reply.InfoMessages.Trim();
                }
            }

            return reply;
        }
        public async Task<ActionReply> GetReplyAsync()
        {
            Log.Start();

            ActionReply reply = new ActionReply();
            var buf = "";

            if (!IsConnected)
            {
                ActionLog(ActionEventType.Error, "No connection available!");
                return null;
            }
                //throw new IOException("No connection available, so reply cannot be retrieved!");

            m_stream.ReadTimeout = ConnectionInfo.TimeoutMilliseconds;
            while ((buf = m_stream.ReadLine(TextEncoding)) != null)
            {
                Match m;
                if ((m = Regex.Match(buf, "^(?<code>[0-9]{3}) (?<message>.*)$")).Success)
                {
                    reply.Code = m.Groups["code"].Value;
                    reply.Message = m.Groups["message"].Value;
                    break;
                }
                reply.InfoMessages += (buf + "\n");
            }

            // log multiline response messages
            if (reply.InfoMessages != null)
            {
                reply.InfoMessages = reply.InfoMessages.Trim();
            }

            return reply;
        }

        private bool isDisposed = false;

        protected virtual FTPClient Create()
        {
            return new FTPClient();
        }
        protected System.IO.Stream BaseStream => m_stream;

        readonly Object m_lock = new Object();

        public Encoding TextEncoding = Encoding.ASCII;
        public ConnectionDetails ConnectionInfo;
        public UserDetails UserInfo;

        public SocketStream m_stream = null;

        public bool IsConnected => m_stream?.IsConnected ?? false;
        public bool IsClone = false;
        public bool IgnoreCertificateErrors = false;
        public ServerCapabilities AvailableOptions = new ServerCapabilities();
        public ServerHashAlgorithm UsedHashAlgorithms = new ServerHashAlgorithm();
        public bool HasFeature(ServerCapabilities cap) => (this.AvailableOptions & cap) == cap;
        public string ServerSystem;

        #region Public Initiations

        public void SetDetails(string hostname, string hostport, string username, string password,
            SSLType securityLayer = SSLType.Unsecure, bool isPassive = true)
        {
            Log.Start();

            SetDetails(hostname, hostport, username,
                password, securityLayer, isPassive, true, 120);
        }

        public void SetDetails(string hostname, string hostport, string username, string password,
            SslProtocols securityProtocols = SslProtocols.None, bool isPassive = true)
        {
            Log.Start();

            SetDetails(hostname, hostport,
                username, password, securityProtocols.GetSSLType(), isPassive, true, 120);
        }

        public void SetDetails(string hostname, string hostport)
        {
            Log.Start();

            SetDetails(hostname, hostport, Helper.AnonUser, Helper.AnonPass,
                SSLType.Unsecure);
        }

#endregion

        private void GetServerOptions(ActionReply reply)
        {
            foreach (string feat in reply.InfoMessages.Split('\n'))
            {
                if (feat.ToUpper().Trim().StartsWith("MLST") || feat.ToUpper().Trim().StartsWith("MLSD"))
                    AvailableOptions |= ServerCapabilities.MLSD;
                else if (feat.ToUpper().Trim().StartsWith("MDTM"))
                    AvailableOptions |= ServerCapabilities.MDTM;
                else if (feat.ToUpper().Trim().StartsWith("REST STREAM"))
                    AvailableOptions |= ServerCapabilities.REST;
                else if (feat.ToUpper().Trim().StartsWith("SIZE"))
                    AvailableOptions |= ServerCapabilities.SIZE;
                else if (feat.ToUpper().Trim().StartsWith("UTF8"))
                    AvailableOptions |= ServerCapabilities.UTF8;
                else if (feat.ToUpper().Trim().StartsWith("PRET"))
                    AvailableOptions |= ServerCapabilities.PRET;
                else if (feat.ToUpper().Trim().StartsWith("MFMT"))
                    AvailableOptions |= ServerCapabilities.MFMT;
                else if (feat.ToUpper().Trim().StartsWith("MFCT"))
                    AvailableOptions |= ServerCapabilities.MFCT;
                else if (feat.ToUpper().Trim().StartsWith("MFF"))
                    AvailableOptions |= ServerCapabilities.MFF;
                else if (feat.ToUpper().Trim().StartsWith("MD5"))
                    AvailableOptions |= ServerCapabilities.MD5;
                else if (feat.ToUpper().Trim().StartsWith("XMD5"))
                    AvailableOptions |= ServerCapabilities.XMD5;
                else if (feat.ToUpper().Trim().StartsWith("XCRC"))
                    AvailableOptions |= ServerCapabilities.XCRC;
                else if (feat.ToUpper().Trim().StartsWith("XSHA1"))
                    AvailableOptions |= ServerCapabilities.XSHA1;
                else if (feat.ToUpper().Trim().StartsWith("XSHA256"))
                    AvailableOptions |= ServerCapabilities.XSHA256;
                else if (feat.ToUpper().Trim().StartsWith("XSHA512"))
                    AvailableOptions |= ServerCapabilities.XSHA512;
                else if (feat.ToUpper().Trim().StartsWith("HASH"))
                {
                    Match m;

                    AvailableOptions |= ServerCapabilities.HASH;

                    if ((m = Regex.Match(feat.ToUpper().Trim(), @"^HASH\s+(?<types>.*)$")).Success)
                    {
                        foreach (string type in m.Groups["types"].Value.Split(';'))
                        {
                            switch (type.ToUpper().Trim())
                            {
                                case "SHA-1":
                                case "SHA-1*":
                                    UsedHashAlgorithms |= ServerHashAlgorithm.SHA1;
                                    break;
                                case "SHA-256":
                                case "SHA-256*":
                                    UsedHashAlgorithms |= ServerHashAlgorithm.SHA256;
                                    break;
                                case "SHA-512":
                                case "SHA-512*":
                                    UsedHashAlgorithms |= ServerHashAlgorithm.SHA512;
                                    break;
                                case "MD5":
                                case "MD5*":
                                    UsedHashAlgorithms |= ServerHashAlgorithm.MD5;
                                    break;
                                case "CRC":
                                case "CRC*":
                                    UsedHashAlgorithms |= ServerHashAlgorithm.CRC;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void SetDetails(string host, string port, string user, string pass, SSLType ssl, bool pasv, bool keepAlive, int timeoutSecs)
        {
            if (OnFtpAction == null)
                throw new IOException("EventHandler OnSystemAction has not been set!");
            //if (OnGeneralAction == null)
            //    throw new IOException("EventHandler OnGeneralAction has not been set!");

#if DEBUG
            Log.Debug(
                "(SetDetails){0}{1}:{2}{0}{3}:***{0}PASV? {4}{0}KeepAlive? {5}{0}Seconds to Time-out: {6}",
                Environment.NewLine, host, port, user, (pasv ? "Yes" : "No"), (keepAlive ? "Yes" : "No"), timeoutSecs);
#endif
#if !DEBUG && VERBOSE
            Log.Verbose(
                "(SetDetails){0}{1}:{2}{0}{3}:***{0}PASV? {4}{0}KeepAlive? {5}{0}Seconds to Time-out: {6}",
                Environment.NewLine, host, port, user, (pasv ? "Yes" : "No"), (keepAlive ? "Yes" : "No"), timeoutSecs);
#endif
            UserInfo = new UserDetails(user, pass);
            ConnectionInfo = new ConnectionDetails(host,
                port.IsInteger() ? int.Parse(port) : (ssl == SSLType.Unsecure ? 21 : 990), ssl, 2048, pasv, keepAlive, timeoutSecs);

            ActionLog(ActionEventType.None, "User Details & Connection Details set!");
        }

        private void ReadStaleData(bool closeStream, bool evenEncrypted)
        {
            if (m_stream != null && m_stream.SocketDataAvailable > 0)
            {
                if (m_stream.IsConnected && (!m_stream.IsEncrypted || evenEncrypted))
                {
                    byte[] buf = new byte[m_stream.SocketDataAvailable];
                    m_stream.RawSocketRead(buf);
                }

                if (closeStream)
                {
                    m_stream.Close();
                }
            }
        }

        private async Task ReadStaleDataAsync(bool closeStream, bool evenEncrypted)
        {
            if (m_stream != null && m_stream.SocketDataAvailable > 0)
            {
                if (m_stream.IsConnected && (!m_stream.IsEncrypted || evenEncrypted))
                {
                    byte[] buf = new byte[m_stream.SocketDataAvailable];
                    await m_stream.RawSocketReadAsync(buf);
                }

                if (closeStream)
                {
                    m_stream.Close();
                }
            }
        }

        public ActionReply CloseDataStream(DataStream strm)
        {
            ActionReply reply = new ActionReply();
            if (strm == null)
            {
                ActionLog(ActionEventType.Error, "The data stream parameter was null!");
                return null;
            }

            lock (m_lock)
            {
                try
                {
                    if (IsConnected)
                    {
                        if (strm.CommandStatus.Type == 2)
                        {
                            if (!(reply = GetReply()).Success)
                            {
                                return reply;
                            }
                        }
                    }
                }
                finally
                {

                }
            }

            return reply;
        }
    }
}
