using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.FTP.Helpers;
using Core.Utils;
using Core.Utils.FTPUtils;
using OpenLogger;
using OpenLogger.Enumerations;
using OpenLogger.EventLogger;

namespace Core.FTP.Client.Stream
{
    public class SocketStream : System.IO.Stream, IDisposable
    {
        private Logger Log = new Logger(LoggerType.Console_File, "Core.FTP.Client.SocketStream", true);

        public event EventHandler<LoggerEventArgs> OnSystemAction;

        public SocketStream(SslProtocols ssl)
        {
            SSLProtocols = ssl;
        }

        public SocketStream(SSLType ssl) : this(ssl.GetSslProtocols())
        { }

        public Socket Socket;
        public NetworkStream NetworkStream;
        public SslStream SslStream;
        public System.IO.Stream BaseStream
        {
            get
            {
                if (SslStream != null)
                    return SslStream;
                else if (NetworkStream != null)
                    return NetworkStream;

                return null;
            }
        }

        public int SocketPollInterval;
        public bool IgnoreCertError = false;
        public bool IsConnected
        {
            get
            {
                Log.Start();

                try
                {
                    if (Socket == null)
                        return false;
                    if (!Socket.Connected)
                        return false;
                    if (!CanRead || !CanWrite)
                        return false;

                    if (SocketPollInterval > 0 &&
                        DateTime.Now.Subtract(LastActivity).TotalMilliseconds > SocketPollInterval)
                    {
#if DEBUG
                        Log.Debug("(IsConnected) Socket.Poll");
#endif
#if !DEBUG && VERBOSE
                        Log.Verbose("(IsConnected) Socket.Poll");
#endif
                        LastActivity = DateTime.Now;
                        if (Socket.Poll(500000, SelectMode.SelectRead) && Socket.Available == 0)
                            return false;
                    }
                }
                catch (SocketException sex)
                {
                    Log.Exception(sex);
                    return false;
                }
                catch (IOException iex)
                {
                    Log.Exception(iex);
                    return false;
                }

                return true;
            }
        }

        public bool IsEncrypted => SslStream != null;

        public SslProtocols SSLProtocols;
        public DateTime LastActivity;
        public int SocketDataAvailable => Socket != null ? Socket.Available : 0;

        public override bool CanRead => NetworkStream != null && NetworkStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => NetworkStream != null && NetworkStream.CanWrite;

        public override long Length => 0;

        public override long Position
        {
            get => BaseStream != null ? BaseStream.Position : 0; set => throw new NotImplementedException();
        }

        public override int ReadTimeout
        {
            get
            {
                if (NetworkStream != null)
                    return NetworkStream.ReadTimeout;
                return Timeout.Infinite;
            }
            set
            {
                if (NetworkStream != null)
                    NetworkStream.ReadTimeout = value;
            }
        }

        public int ConnectTimeout;

        public IPEndPoint LocalEndPoint => (IPEndPoint) Socket?.LocalEndPoint;
        public IPEndPoint RemoteEndPoint => (IPEndPoint) Socket?.RemoteEndPoint;

        protected bool OnValidateCertificate(X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            var args = new LoggerEventArgs("Certificate Validation:{0} => Issuer: {1}{0} => Name: {2}{0} => Accepted: {3}",
                                            Environment.NewLine,
                                            cert?.Issuer, cert?.Subject, 
                                            (errors == SslPolicyErrors.None) ? "Yes" : ("No - " + errors.GetSslErrorsString() + (IgnoreCertError ? " (Ignoring!)" : "")));
            SystemAction(args);
            if (errors != SslPolicyErrors.None)
                if (!IgnoreCertError)
                    return false;

            return true;
        }

        protected virtual void SystemAction(LoggerEventArgs e)
        {
            OnSystemAction?.Invoke(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            //FtpTrace.WriteStatus(FtpTraceLevel.Verbose, "Disposing FtpSocketStream...");
            if (Socket != null)
            {
                try
                {
                    if (Socket.Connected)
                    {
                        Socket.Close();
                    }
                }
                catch (SocketException ex)
                {
                    //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "Caught and discarded a SocketException while cleaning up the Socket: " + ex.ToString());

                }
                finally
                {
                    Socket = null;
                }
            }

            if (NetworkStream != null)
            {
                try
                {
                    NetworkStream.Dispose();
                }
                catch (IOException ex)
                {
                    //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "Caught and discarded an IOException while cleaning up the NetworkStream: " + ex.ToString());
                }
                finally
                {
                    NetworkStream = null;
                }
            }

//#if !NO_SSL
            if (SslStream != null)
            {
                try
                {
                    SslStream.Dispose();
                }
                catch (IOException ex)
                {
                    //FtpTrace.WriteStatus(FtpTraceLevel.Warn, "Caught and discarded an IOException while cleaning up the SslStream: " + ex.ToString());
                }
                finally
                {
                    SslStream = null;
                }
            }
//#endif
        }

        public void Connect(string host, int port, int ipVersions = 2)
        {
            IAsyncResult ar = null;
            IPAddress[] addresses = Dns.GetHostAddresses(host);

            if (ipVersions == 0)
                throw new ArgumentException("The ipVersions parameter must contain at least 1 flag.");

            for (int i = 0; i < addresses.Length; i++)
            {
                // we don't need to do this check unless
                // a particular version of IP has been
                // omitted so we won't.
                if (ipVersions != 2)
                {
                    switch (addresses[i].AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            if ((ipVersions & 0) != 0)
                            {
                                continue;
                            }
                            break;
                        case AddressFamily.InterNetworkV6:
                            if ((ipVersions & 1) != 1)
                            {
                                continue;
                            }
                            break;
                    }
                }

                //if (FtpTrace.LogIP)
                //{
                //    FtpTrace.WriteStatus(FtpTraceLevel.Info, "Connecting to " + addresses[i].ToString() + ":" + port);
                //}
                //else
                //{
                //    FtpTrace.WriteStatus(FtpTraceLevel.Info, "Connecting to ***:" + port);
                //}

                Socket = new Socket(addresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                ar = Socket.BeginConnect(addresses[i], port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(ConnectTimeout, true))
                {
                    Close();

                    // check to see if we're out of addresses, and throw a TimeoutException
                    if ((i + 1) == addresses.Length)
                    {
                        throw new TimeoutException("Timed out trying to connect!");
                    }
                }
                else
                {
                    Socket.EndConnect(ar);
                    // we got a connection, break out
                    // of the loop.
                    break;
                }
            }

            // make sure that we actually connected to
            // one of the addresses returned from GetHostAddresses()
            if (Socket == null || !Socket.Connected)
            {
                Close();
                throw new IOException("Failed to connect to host.");
            }

            NetworkStream = new NetworkStream(Socket);
            NetworkStream.ReadTimeout = ReadTimeout;
            LastActivity = DateTime.Now;
        }
        public async Task ConnectAsync(string host, int port, int ipVersions = 2)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(host);

            if (ipVersions == 0)
                throw new ArgumentException("The ipVersions parameter must contain at least 1 flag.");

            for (int i = 0; i < addresses.Length; i++)
            {
                // we don't need to do this check unless
                // a particular version of IP has been
                // omitted so we won't.
                if (ipVersions != 2)
                {
                    switch (addresses[i].AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            if ((ipVersions & 0) != 0)
                            {
                                continue;
                            }
                            break;
                        case AddressFamily.InterNetworkV6:
                            if ((ipVersions & 1) != 1)
                            {
                                continue;
                            }
                            break;
                    }
                }

                //if (FtpTrace.LogIP)
                //{
                //    FtpTrace.WriteStatus(FtpTraceLevel.Info, "Connecting to " + addresses[i].ToString() + ":" + port);
                //}
                //else
                //{
                //    FtpTrace.WriteStatus(FtpTraceLevel.Info, "Connecting to ***:" + port);
                //}

                Socket = new Socket(addresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                var connectResult = Socket.BeginConnect(addresses[i], port, null, null);
                await Task.Factory.FromAsync(connectResult, Socket.EndConnect);
                break;
            }

            // make sure that we actually connected to
            // one of the addresses returned from GetHostAddresses()
            if (Socket == null || !Socket.Connected)
            {
                Close();
                throw new IOException("Failed to connect to host.");
            }

            NetworkStream = new NetworkStream(Socket);
            NetworkStream.ReadTimeout = ReadTimeout;
            LastActivity = DateTime.Now;
        }

        public override void Flush()
        {
            if(!IsConnected)
                throw new InvalidOperationException("Socket is not connected!");
            if(BaseStream == null)
                throw new InvalidOperationException("BaseStream has not been initialized!");

            BaseStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Socket is not connected!");
            if (BaseStream == null)
                throw new InvalidOperationException("BaseStream has not been initialized!");

            return BaseStream.FlushAsync(cancellationToken);
        }

        public int RawSocketRead(byte[] buffer)
        {
            if (Socket != null && Socket.Connected)
                return Socket.Receive(buffer, buffer.Length, 0);

            return -1;
        }

        public async Task<int> RawSocketReadAsync(byte[] buffer)
        {
            if (Socket != null && Socket.Connected)
                return await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), 0);
            return -1;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            IAsyncResult ar = null;
            if (BaseStream == null)
                return -1;

            LastActivity = DateTime.Now;
            ar = BaseStream.BeginRead(buffer, offset, count, null, null);
            if(!ar.AsyncWaitHandle.WaitOne(ReadTimeout, true))
                throw new TimeoutException("Time-out while trying to read data from stream!");

            return BaseStream.EndRead(ar);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (BaseStream == null)
                return -1;
            LastActivity = DateTime.Now;
            return await BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public string ReadLine(System.Text.Encoding encoding)
        {
            List<byte> data = new List<byte>();
            byte[] buf = new byte[1];
            string line = null;

            while (Read(buf, 0, buf.Length) > 0)
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

        public async Task<string> ReadLineAsync(System.Text.Encoding encoding, CancellationToken token)
        {
            List<byte> data = new List<byte>();
            byte[] buf = new byte[1];
            string line = null;

            while (await ReadAsync(buf, 0, buf.Length, token) > 0)
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

        public async Task<string> ReadLineAsync(System.Text.Encoding encoding)
        {
            return await ReadLineAsync(encoding, CancellationToken.None);
        }

        public IEnumerable<string> ReadAllLines(System.Text.Encoding encoding, int bufferSize)
        {
            int charRead;
            List<byte> data = new List<byte>();
            byte[] buf = new byte[bufferSize];

            while ((charRead = Read(buf, 0, buf.Length)) > 0)
            {
                var firstByteToReadIdx = 0;

                var separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array readed

                while (separatorIdx >= 0) // at least one '\n' returned
                {
                    while (firstByteToReadIdx <= separatorIdx)
                        data.Add(buf[firstByteToReadIdx++]);

                    var line = encoding.GetString(data.ToArray()).Trim('\r', '\n'); // convert data to string
                    yield return line;
                    data.Clear();

                    separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array readed
                }

                while (firstByteToReadIdx < charRead)  // add all remainings characters to data
                    data.Add(buf[firstByteToReadIdx++]);
            }
        }

        public async Task<IEnumerable<string>> ReadAllLinesAsync(System.Text.Encoding encoding, int bufferSize)
        {
            int charRead;
            List<byte> data = new List<byte>();
            List<string> lines = new List<string>();
            byte[] buf = new byte[bufferSize];

            while ((charRead = await ReadAsync(buf, 0, buf.Length)) > 0)
            {
                var firstByteToReadIdx = 0;

                var separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array readed

                while (separatorIdx >= 0) // at least one '\n' returned
                {
                    while (firstByteToReadIdx <= separatorIdx)
                        data.Add(buf[firstByteToReadIdx++]);

                    var line = encoding.GetString(data.ToArray()).Trim('\r', '\n'); // convert data to string
                    lines.Add(line);
                    data.Clear();

                    separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array readed
                }

                while (firstByteToReadIdx < charRead)  // add all remainings characters to data
                    data.Add(buf[firstByteToReadIdx++]);
            }

            return lines;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (BaseStream == null)
                return;

            BaseStream.Write(buffer, offset, count);
            LastActivity = DateTime.Now;
        }

        public void SetSocketOption(SocketOptionLevel level, SocketOptionName name, bool value)
        {
            if (Socket == null)
                throw new InvalidOperationException("The underlying socket is null. Have you established a connection?");

            Socket.SetSocketOption(level, name, value);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            if (BaseStream == null)
                return;

            await BaseStream.WriteAsync(buffer, offset, count, token);
            LastActivity = DateTime.Now;
        }

        public void WriteLine(System.Text.Encoding encoding, string buf)
        {
            byte[] data;
            data = encoding.GetBytes((buf + "\r\n"));
            Write(data, 0, data.Length);
        }

        public async Task WriteLineAsync(System.Text.Encoding encoding, string buf, CancellationToken token)
        {
            byte[] data = encoding.GetBytes(buf + "\r\n");
            await WriteAsync(data, 0, data.Length, token);
        }

        public async Task WriteLineAsync(System.Text.Encoding encoding, string buf)
        {
            await WriteLineAsync(encoding, buf, CancellationToken.None);
        }

        public void DeactivateEncryption()
        {
            if (!IsConnected)
                throw new InvalidOperationException("The SocketStream object is not connected.");

            if (SslStream == null)
                throw new InvalidOperationException("SSL Encryption has not been enabled on this stream.");

            SslStream.Close();
            SslStream = null;
        }

        public async Task ActivateEncryptionAsync(string targethost)
        {
            await ActivateEncryptionAsync(targethost, null, SSLProtocols);
        }

        public async Task ActivateEncryptionAsync(string targethost, X509CertificateCollection clientCerts)
        {
            await ActivateEncryptionAsync(targethost, clientCerts, SSLProtocols);
        }

        public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts)
        {
            ActivateEncryption(targethost, clientCerts, SSLProtocols);
        }

        public void ActivateEncryption(string targethost)
        {
            ActivateEncryption(targethost, null, SSLProtocols);
        }

        public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts, SslProtocols sslProtocols)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Socket is not connected.");

            if (NetworkStream == null)
                throw new InvalidOperationException("The base network stream is null.");

            if (SslStream != null)
                throw new InvalidOperationException("SSL Encryption has already been enabled on this stream.");

            try
            {
                DateTime auth_start;
                TimeSpan auth_time_total;

                SslStream = new SslStreamEx(NetworkStream, true, new RemoteCertificateValidationCallback(
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                        return OnValidateCertificate(certificate, chain, sslPolicyErrors);
                    }
                ));

                auth_start = DateTime.Now;
                SslStream.AuthenticateAsClient(targethost, clientCerts, sslProtocols, true);

                auth_time_total = DateTime.Now.Subtract(auth_start);
                //FtpTrace.WriteStatus(FtpTraceLevel.Info, "FTPS Authentication Successful");
                //FtpTrace.WriteStatus(FtpTraceLevel.Verbose, "Time to activate encryption: " + auth_time_total.Hours + "h " + auth_time_total.Minutes + "m " + auth_time_total.Seconds + "s.  Total Seconds: " + auth_time_total.TotalSeconds + ".");
            }
            catch (AuthenticationException)
            {
                // authentication failed and in addition it left our 
                // ssl stream in an unusable state so cleanup needs
                // to be done and the exception can be re-thrown for
                // handling down the chain. (Add logging?)
                Close();
                //FtpTrace.WriteStatus(FtpTraceLevel.Error, "FTPS Authentication Failed");
                throw;
            }
        }

        public async Task ActivateEncryptionAsync(string targethost, X509CertificateCollection clientCerts, SslProtocols sslProtocols)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Socket is not connected.");

            if (NetworkStream == null)
                throw new InvalidOperationException("The base network stream is null.");

            if (SslStream != null)
                throw new InvalidOperationException("SSL Encryption has already been enabled on this stream.");

            try
            {
                DateTime auth_start;
                TimeSpan auth_time_total;

                SslStream = new SslStreamEx(NetworkStream, true, new RemoteCertificateValidationCallback(
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return OnValidateCertificate(certificate, chain, sslPolicyErrors);
                    }
                ));

                auth_start = DateTime.Now;
                await SslStream.AuthenticateAsClientAsync(targethost, clientCerts, sslProtocols, true);

                auth_time_total = DateTime.Now.Subtract(auth_start);
                //FtpTrace.WriteStatus(FtpTraceLevel.Info, "FTPS Authentication Successful");
                //FtpTrace.WriteStatus(FtpTraceLevel.Verbose, "Time to activate encryption: " + auth_time_total.Hours + "h " + auth_time_total.Minutes + "m " + auth_time_total.Seconds + "s.  Total Seconds: " + auth_time_total.TotalSeconds + ".");
            }
            catch (AuthenticationException)
            {
                // authentication failed and in addition it left our 
                // ssl stream in an unusable state so cleanup needs
                // to be done and the exception can be re-thrown for
                // handling down the chain. (Add logging?)
                Close();
                //FtpTrace.WriteStatus(FtpTraceLevel.Error, "FTPS Authentication Failed");
                throw;
            }
        }

        public void Listen(IPAddress address, int port)
        {
            if (!IsConnected)
            {
                if (Socket == null)
                    Socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                Socket.Bind(new IPEndPoint(address, port));
                Socket.Listen(1);
            }
        }

        public void Accept()
        {
            if (Socket != null)
                Socket = Socket.Accept();
        }

        public async Task AcceptAsync()
        {
            if (Socket != null)
            {
                var iar = Socket.BeginAccept(null, null);
                await Task.Factory.FromAsync(iar, Socket.EndAccept);
            }
        }


    }
}
