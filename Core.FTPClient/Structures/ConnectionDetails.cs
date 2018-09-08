using System.Security.Cryptography.X509Certificates;

namespace Core.FTPClient.Structures
{
    public struct ConnectionDetails
    {
        /// <summary>
        /// Timeout in milliseconds
        /// (Default: 2000)
        /// </summary>
        public int TimeoutMilliseconds { get; }
        /// <summary>
        /// Passive mode
        /// (Default: true)
        /// </summary>
        public bool PassiveMode { get; set; }
        /// <summary>
        /// Keep connection alive
        /// (Default: true)
        /// </summary>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// Host address
        /// </summary>
        public string Hostname { get; }
        /// <summary>
        /// Host port
        /// (Default: 21)
        /// </summary>
        public int Port { get; }
        /// <summary>
        /// Host security
        /// (Default: Unsecure/None)
        /// </summary>
        public SSLType Security { get; }
        /// <summary>
        /// Internal memory buffer size
        /// (Default: 2048)
        /// </summary>
        public int BufferSize { get; }

        public X509CertificateCollection ClientCertificates;// = new X509CertificateCollection();

        public ConnectionDetails(string hostName) : this(hostName, 21, SSLType.Unsecure, 2048, true, true, 2)
        { }
        public ConnectionDetails(string hostName, string port, SSLType security) : this(hostName,
            Utils.IsInt32(port) ? int.Parse(port) : -1, security, 2048, true, true, 2)
        { }
        public ConnectionDetails(string hostName, int port, SSLType security, int bufferSize, bool passiveMode,
            bool keepAlive, int timeoutSeconds)
        {
            Hostname = hostName;
            Port = port;
            Security = security;
            BufferSize = bufferSize;
            PassiveMode = passiveMode;
            KeepAlive = keepAlive;
            TimeoutMilliseconds = timeoutSeconds * 1000;
            ClientCertificates = new X509CertificateCollection();
        }

        public ConnectionDetails(string hostName, string port) : this(hostName,
            Utils.IsInt32(port) ? int.Parse(port) : -1, 2048)
        { }
        public ConnectionDetails(string hostName, int port, int bufferSize) : this(hostName, port, SSLType.Unsecure,
            bufferSize, true, true, 2)
        { }
    }
}