using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient.SSL
{
    public class ValidationEventArgs : EventArgs
    {
        /// <summary>
        /// The certificate to be validated
        /// </summary>
        public X509Certificate Certificate { get; set; }

        /// <summary>
        /// The certificate chain
        /// </summary>
        public X509Chain SslChain { get; set; }

        /// <summary>
        /// Validation errors, if any.
        /// </summary>
        public SslPolicyErrors PolicyErrors { get; set; } = SslPolicyErrors.None;

        /// <summary>
        /// Gets or sets a value indicating if this certificate should be accepted. The default
        /// value is false. If the certificate is not accepted, an AuthenticationException will
        /// be thrown.
        /// </summary>
        public bool Accept { get; set; }
    }
}
