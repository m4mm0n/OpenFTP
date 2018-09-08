using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FTPClient.Enumerations;
using FTPClient.Structures;
using OpenLogger;
using OpenLogger.Enumerations;
using OpenLogger.EventLogger;

namespace FTPClient
{
    public class FTP
    {
        #region Private/Internal

        private Logger Log = new Logger(LoggerType.Console, "OpenFTP.FTP", true);
        private Socket client;
        private Stream stream = null;
        private Stream stream2 = null;
        private bool useStream;
        private bool isUpload;
        private byte[] buffer;
        readonly Object m_lock = new Object();

        protected virtual void GeneralAction(LoggerEventArgs e)
        {
            OnGeneralAction?.Invoke(this, e);
        }

        protected virtual void SystemAction(LoggerEventArgs e)
        {
            OnSystemAction?.Invoke(this, e);
        }

        private void SysLog(string format, params object[] param)
        {
#if DEBUG
            Log.Debug(string.Format("(SystemLog) {0}", format), param);
#endif
            var args = new LoggerEventArgs(string.Format("[{0}] {1}", DateTime.Now.ToShortTimeString(), format), param);
            SystemAction(args);
        }

        private void SysLog(ActionReply mes)
        {
#if DEBUG
            Log.Debug(string.Format("(SystemLog) [ActionReply]{0}{1}", Environment.NewLine, mes.ToString()));
#endif
            LoggerEventArgs args = null;
            args = new LoggerEventArgs(string.Format("[{0}] {1} {2} ({3})", DateTime.Now.ToShortTimeString(),
                    mes.Code, mes.Message, mes.InfoMessages));
        }

        private void SysLog(Reply mes)
        {
#if DEBUG
            Log.Debug(string.Format("(SystemLog) [Reply.Struct]{0}Response: {1}{0}Message: {2}{0}Code: {3}",
                Environment.NewLine, mes.Response, mes.Message, mes.Code.ToString()));
#endif
            LoggerEventArgs args = null;
            if (mes.Message.Length > 0)
                args = new LoggerEventArgs(string.Format("[{0}] {1} {2} ({3})", DateTime.Now.ToShortTimeString(),
                    mes.Code.ToString(), mes.Response, mes.Message));
            else
                args = new LoggerEventArgs(string.Format("[{0}] {1} {2}", DateTime.Now.ToShortTimeString(),
                    mes.Code.ToString(), mes.Response));
            SystemAction(args);
        }

        private void GeneralLog(string format, params object[] param)
        {
#if DEBUG
            Log.Debug(format, param);
#endif

            var args = new LoggerEventArgs(string.Format("[{0}] {1}", DateTime.Now.ToShortTimeString(), format),
                param);
            GeneralAction(args);
        }

        private bool OnCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            SysLog("Validating certificate...");

            switch (errors)
            {
                case SslPolicyErrors.RemoteCertificateChainErrors:
                    SysLog("[ERROR!] Remote Certificate Chain Errors");
                    if (IgnoreCertificateErrors)
                    {
                        SysLog("Validation forced to acceptance!");
                        return true;
                    }
                    else
                    {
                        SysLog("Validation failed!");
                        return false;
                    }
                case SslPolicyErrors.RemoteCertificateNameMismatch:
                    SysLog("[ERROR!] Remote Certificate Name-Mismatch");
                    if (IgnoreCertificateErrors)
                    {
                        SysLog("Validation forced to acceptance!");
                        return true;
                    }
                    else
                    {
                        SysLog("Validation failed!");
                        return false;
                    }
                case SslPolicyErrors.RemoteCertificateNotAvailable:
                    SysLog("[ERROR!] Remote Certificate Not Available");
                    if (IgnoreCertificateErrors)
                    {
                        SysLog("Validation forced to acceptance!");
                        return true;
                    }
                    else
                    {
                        SysLog("Validation failed!");
                        return false;
                    }
                default:
                case SslPolicyErrors.None:
                    SysLog("Validated Succesfully!");
                    return true;
            }
        }

        private void showSslInfo(string serverName, SslStream sslStream, bool verbose)
        {
            showCertificateInfo(sslStream.RemoteCertificate, verbose);

            var dateTime = DateTime.Now.ToShortTimeString();

            var args = new LoggerEventArgs("[{0}] SSL Connection Report:{1}" +
                                           " -> Is Authenticated: {2}{1}" +
                                           " -> Is Encrypted: {3}{1}" +
                                           " -> Is Signed: {4}{1}" +
                                           " -> Is Mutually Authenticated: {5}{1}" +
                                           " -> SSL Protocol: {6}", dateTime, Environment.NewLine,
                                            sslStream.IsAuthenticated ? "Yes" : "No",
                                            sslStream.IsEncrypted ? "Yes" : "No",
                                            sslStream.IsSigned ? "Yes" : "No",
                                            sslStream.IsMutuallyAuthenticated ? "Yes" : "No",
                                            Utilities.GetSslProtocolsString(sslStream.SslProtocol));
        }

        private void showCertificateInfo(X509Certificate remoteCertificate, bool verbose)
        {
            var dateTime = DateTime.Now.ToShortTimeString();

            var args = new LoggerEventArgs("[{0}] Certificate Information:{1}" +
                                           " -> Valid From: {2}{1}" +
                                           " -> Valid To: {3}{1}" +
                                           " -> Certificate Format: {4}{1}" +
                                           " -> Issuer Name: {5}", dateTime, Environment.NewLine,
                                            remoteCertificate.GetEffectiveDateString(),
                                            remoteCertificate.GetExpirationDateString(),
                                            remoteCertificate.GetFormat(),
                                            remoteCertificate.Issuer);
            SystemAction(args);

#if DEBUG
            //meh...
#endif

            //GeneralLog("Certificate Information:");
            //GeneralLog("-> Valid From: {0}", remoteCertificate.GetEffectiveDateString());
            //GeneralLog("-> Valid To: {0}", remoteCertificate.GetExpirationDateString());
            //GeneralLog("-> Certificate Format: {0}", remoteCertificate.GetFormat());
            //GeneralLog("-> Issuer Name: {0}", remoteCertificate.Issuer);

            //if (verbose)
            //{
            //    Console.WriteLine("Serial Number: \n{0}", remoteCertificate.GetSerialNumberString());
            //    Console.WriteLine("Hash: \n{0}", remoteCertificate.GetCertHashString());
            //    Console.WriteLine("Key Algorithm: \n{0}", remoteCertificate.GetKeyAlgorithm());
            //    Console.WriteLine("Key Algorithm Parameters: \n{0}", remoteCertificate.GetKeyAlgorithmParametersString());
            //    Console.WriteLine("Public Key: \n{0}", remoteCertificate.GetPublicKeyString());
            //}
        }

        private void WriteMsg(string message)
        {
            var en = new System.Text.ASCIIEncoding();

            var WriteBuffer = new byte[1024];
            WriteBuffer = en.GetBytes(message);

            stream.Write(WriteBuffer, 0, WriteBuffer.Length);
        }

        private string ReadLine(Encoding encoding)
        {
            List<byte> data = new List<byte>();
            byte[] buf = new byte[1];
            string line = null;

            while (useStream ? (stream.Read(buf, 0, buf.Length) > 0) : (client.Receive(buf) > 0))
            {
                data.Add(buf[0]);
                if ((char)buf[0] == '\n')
                {
                    line = encoding.GetString(data.ToArray()).Trim('\r', '\n');
                    break;
                }
            }

            return line;
        }

        private ActionReply GetReply(bool isFirstTime = false)
        {
            ActionReply reply = new ActionReply();

            //pretty simple locking mechanism to prevent threads/tasks from 
            //going over eachother and makin crazy shit happen...
            lock (m_lock)
            {
                var buf = "";
                if(!isFirstTime)
                    if (!IsConnected)
                        throw new IOException("No connection available!");

                if (useStream)
                {
                    stream.ReadTimeout = 15000;
                }

                while ((buf = ReadLine(Encoding.ASCII)) != null)
                {
                    Match m;
                    if ((m = Regex.Match(buf, "^(?<code>[0-9]{3}) (?<message>.*)$")).Success)
                    {
                        reply.Code = m.Groups["code"].Value;
                        reply.Message = m.Groups["message"].Value;
                        break;
                    }

                    reply.InfoMessages += (buf + "\n");

                    // log multiline response messages
                    if (reply.InfoMessages != null)
                    {
                        reply.InfoMessages = reply.InfoMessages.Trim();
                    }
                }
            }

            return reply;
        }

        private Reply readReply()
        {
            if (useStream)
                return ResponseMsg();
            else
            {
                var retl = readLine();
                return new Reply(retl, "", int.Parse(retl.Substring(0, 3)));
            }
        }

        private Reply ResponseMsg()
        {
            var enc = new System.Text.ASCIIEncoding();
            var serverbuff = new byte[1024];
            int count = 0;
            while (true)
            {
                var buff = new byte[2];
                int bytes = stream.Read(buff, 0, 1);
                if (bytes == 1)
                {
                    serverbuff[count] = buff[0];
                    count++;

                    if (buff[0] == '\n')
                    {
                        break;
                    }
                }
                else
                {
                    break;
                };
            };

            var retl = enc.GetString(serverbuff, 0, count);
            return new Reply(retl, "", int.Parse(retl.Substring(0, 3)));
        }

        private string readLine()
        {
            var mes = "";
            int bytes = -1;
            while (true)
            {
                if (useStream)
                    bytes = stream.Read(buffer, buffer.Length, 0);
                else
                    bytes = client.Receive(buffer, buffer.Length, 0);


                mes += Encoding.ASCII.GetString(buffer, 0, bytes);
                if (bytes < buffer.Length)
                {
                    break;
                }
            }

            char[] seperator = { '\n' };
            string[] mess = mes.Split(seperator);

            if (mes.Length > 2)
            {
                mes = mess[mess.Length - 2];
            }
            else
            {
                mes = mess[0];
            }

            if (!mes.Substring(3, 1).Equals(" "))
            {
                return readLine();
            }

            return mes;
        }
        #endregion

        #region Public

        public event EventHandler<LoggerEventArgs> OnGeneralAction;
        public event EventHandler<LoggerEventArgs> OnSystemAction;

        public int BufferSize { get; set; }
        public UserCredentials UserInfo { get; set; }
        public ConnectionDetails HostInfo { get; set; }
        public bool IgnoreCertificateErrors = true;
        public bool IsConnected = false;

        public FTP()
        {
            GeneralLog("Initializing FTP Client Core...");
        }

        public void SetUserInfo(string userName, string passWord)
        {
            UserInfo = new UserCredentials(userName, passWord);
            GeneralLog("User Credentials set!");
        }

        public void SetConnectionInfo(string hostAddress, string hostPort, SSLType security,
            int bufferSize, bool passiveMode, bool keepAlive, int timeoutSeconds)
        {
            HostInfo = new ConnectionDetails(hostAddress,
                Utilities.IsInteger(hostPort) ? int.Parse(hostPort) : (security == SSLType.Unsecure ? 21 : 990),
                security, bufferSize, passiveMode, keepAlive, timeoutSeconds);
            BufferSize = bufferSize;
            buffer = new byte[bufferSize];
            GeneralLog("Connection Details set!");
        }

        public void getSslStream()
        {
            this.getSslStream(client);
        }

        public async Task getSslStreamAsync(Socket Csocket)
        {
            RemoteCertificateValidationCallback callback = new RemoteCertificateValidationCallback(OnCertificateValidation);
            SslStream _sslStream = new SslStream(new NetworkStream(Csocket), true, callback);

            try
            {
                await _sslStream.AuthenticateAsClientAsync(
                    HostInfo.Hostname,
                    null,
                    Utilities.GetSslProtocols(HostInfo),
                    true);

                if (_sslStream.IsAuthenticated)
                    if (isUpload)
                        stream2 = _sslStream;
                    else
                        stream = _sslStream;

            }
            catch (Exception ex)
            {
                SysLog("An unforseen error when trying to get the secure socket layer initiation process: {0}",
                    ex.Message);
            }

            showSslInfo(HostInfo.Hostname, _sslStream, true);
        }
        public void getSslStream(Socket Csocket)
        {
            RemoteCertificateValidationCallback callback = new RemoteCertificateValidationCallback(OnCertificateValidation);
            SslStream _sslStream = new SslStream(new NetworkStream(Csocket), true, callback);

            try
            {
                _sslStream.AuthenticateAsClient(
                    HostInfo.Hostname,
                    null,
                    Utilities.GetSslProtocols(HostInfo),
                    true);

                if (_sslStream.IsAuthenticated)
                    if (isUpload)
                        stream2 = _sslStream;
                    else
                        stream = _sslStream;

            }
            catch (Exception ex)
            {
                SysLog("An unforseen error when trying to get the secure socket layer initiation process: {0}",
                    ex.Message);
            }

            showSslInfo(HostInfo.Hostname, _sslStream, true);
        }

        public void LogIn()
        {
            SysLog(sendCommand("USER " + UserInfo.UserName));
            SysLog(sendCommand("PASS " + UserInfo.PassWord));
        }

        public void setBinaryMode(BinaryMode mode)
        {
            var rep = new ActionReply();
            switch (mode)
            {
                case BinaryMode.ASCII:
                    rep = sendCommand("TYPE A");
                    break;
                case BinaryMode.Binary:
                    rep = sendCommand("TYPE I");
                    break;
            }

            SysLog(rep);
            if (!rep.Success/*.Code != 200*/)
            {
                //throw new IOException(rep.Response.Substring(4));
                SysLog("Unable to set correct binary mode - you sure this server supports either " + '"' + "TYPE A" +
                       '"' + " or " + '"' + "TYPE I" + '"' + "?");
            }
        }

        public ActionReply sendCommand(string command)
        {
            var cmdBytes = Encoding.ASCII.GetBytes((command + "\r\n").ToCharArray());

            if(command.StartsWith("USER"))
                SysLog("USER ***");
            else if (command.StartsWith("PASS"))
                SysLog("PASS ***");
            else
                SysLog(command);
            

            if (useStream)
            {
                WriteMsg(command + "\r\n");
            }
            else
                client.Send(cmdBytes, cmdBytes.Length, 0);

            //return readReply();
            return GetReply();
        }

        public async Task ConnectAsync()
        {
            if (OnSystemAction == null)
            {
                if (MessageBox.Show("No reference to " + '"' + "OnSystemAction" + '"' + " could be found - do you wish to continue?", "Reference Issue!", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            if (OnGeneralAction == null)
            {
                if (MessageBox.Show("No reference to " + '"' + "OnGeneralAction" + '"' + " could be found - do you wish to continue?", "Reference Issue!", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ep = new IPEndPoint(Dns.GetHostEntry(HostInfo.Hostname).AddressList[0], HostInfo.Port);

            try
            {
                //client.Connect(ep);
                await client.ConnectAsync(ep);
            }
            catch (Exception ex)
            {
                SysLog("Could not reach the server!");
                return;
            }

            var rep = GetReply(true);
            SysLog(rep);
            if (!rep.Success)
                Disconnect();
            else
            {
                IsConnected = true;
                if (HostInfo.Security == SSLType.Unsecure)
                    IsConnected = true;
                else
                {
                    var zx = new ActionReply();
                    switch (HostInfo.Security)
                    {
                        case SSLType.AuthTLSv10:
                        case SSLType.AuthTLSv11:
                        case SSLType.AuthTLSv12:
                        case SSLType.AuthSSL:
                            zx = sendCommand("AUTH SSL");
                            if (zx.Success)
                            {
                                await getSslStreamAsync(client);
                                useStream = true;
                                IsConnected = true;
                            }
                            break;
                    }
                }
            }
        }

        public void Connect()
        {
            if (OnSystemAction == null)
            {
                if(MessageBox.Show("No reference to " + '"' + "OnSystemAction" + '"' + " could be found - do you wish to continue?", "Reference Issue!", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            if (OnGeneralAction == null)
            {
                if (MessageBox.Show("No reference to " + '"' + "OnGeneralAction" + '"' + " could be found - do you wish to continue?", "Reference Issue!", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ep = new IPEndPoint(Dns.GetHostEntry(HostInfo.Hostname).AddressList[0], HostInfo.Port);

            try
            {
                client.Connect(ep);
            }
            catch (Exception ex)
            {
                SysLog("Could not reach the server!");
                return;
            }

            var rep = GetReply(true);
            SysLog(rep);
            if (!rep.Success)
                Disconnect();
            else
            {
                IsConnected = true;
                if (HostInfo.Security == SSLType.Unsecure)
                    IsConnected = true;
                else
                {
                    var zx = new ActionReply();
                    switch (HostInfo.Security)
                    {
                        case SSLType.AuthTLSv10:
                        case SSLType.AuthTLSv11:
                        case SSLType.AuthTLSv12:
                        case SSLType.AuthSSL:
                            zx = sendCommand("AUTH SSL");
                            if (zx.Success)
                            {
                                getSslStream(client);
                                useStream = true;
                                IsConnected = true;
                            }
                            break;
                    }
                }
            }
        }

        public void Disconnect()
        {
            if (client != null)
            {
                SysLog(sendCommand("QUIT"));
                client.Close();
                client = null;
                IsConnected = false;
            }
        }

        public Socket createDataSocket()
        {
            if (HostInfo.PassiveMode)
            {
                var m = sendCommand("PASV");
                SysLog(m);
                if (!m.Success)
                {
                    //throw new IOException(m.Response.Substring(4));
                    //SysLog(m.Response.Substring(4));
                    SysLog("Closing data socket!");
                    return null;
                }


                int index1 = m.Message.IndexOf('(');
                int index2 = m.Message.IndexOf(')');
                string ipData = m.Message.Substring(index1 + 1, index2 - index1 - 1);
                int[] parts = new int[6];

                int len = ipData.Length;
                int partCount = 0;
                string buf = "";

                for (int i = 0; i < len && partCount <= 6; i++)
                {
                    char ch = Char.Parse(ipData.Substring(i, 1));
                    if (Char.IsDigit(ch))
                        buf += ch;
                    else if (ch != ',')
                    {
                        //throw new IOException("Malformed PASV reply: " + m.Response);
                        SysLog("Malformed PASV reply - closing data socket!");
                        return null;
                    }

                    if (ch == ',' || i + 1 == len)
                    {

                        try
                        {
                            parts[partCount++] = Int32.Parse(buf);
                            buf = "";
                        }
                        catch (Exception)
                        {
                            //throw new IOException("Malformed PASV reply: " + m.Response);
                            SysLog("Malformed PASV reply - closing data socket!");
                            return null;
                        }
                    }
                }

                string ipAddress = parts[0] + "." + parts[1] + "." +
                                   parts[2] + "." + parts[3];

                int port = (parts[4] << 8) + parts[5];

                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(Dns.Resolve(ipAddress).AddressList[0], port);

                try
                {
                    s.Connect(ep);
                    return s;
                }
                catch (Exception)
                {
                    //throw new IOException("Can't connect to remoteserver");
                    SysLog("Cannot reach the remote server - closing data socket!");
                    return null;
                }
            }
            throw new NotSupportedException("Non-passive mode has yet to be implemented!");
            //return s;
        }
        #endregion
    }
}
