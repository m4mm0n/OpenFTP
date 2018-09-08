namespace FTPClient.Enumerations
{
    public enum SSLType
    {
        /// <summary>
        /// The most commonly used - no security measures...
        /// </summary>
        Unsecure,
        /// <summary>
        /// https://en.wikipedia.org/wiki/FTPS#Implicit
        /// </summary>
        ImplicitSSL,
        /// <summary>
        /// https://en.wikipedia.org/wiki/FTPS#Transport_Layer_Security_(TLS)/Secure_Socket_Layer_(SSL)
        /// </summary>
        AuthSSL,
        /// <summary>
        /// https://en.wikipedia.org/wiki/FTPS#Transport_Layer_Security_(TLS)/Secure_Socket_Layer_(SSL)
        /// </summary>
        AuthTLSv10,
        /// <summary>
        /// https://en.wikipedia.org/wiki/FTPS#Transport_Layer_Security_(TLS)/Secure_Socket_Layer_(SSL)
        /// </summary>
        AuthTLSv11,
        /// <summary>
        /// https://en.wikipedia.org/wiki/FTPS#Transport_Layer_Security_(TLS)/Secure_Socket_Layer_(SSL)
        /// </summary>
        AuthTLSv12
    }
}
