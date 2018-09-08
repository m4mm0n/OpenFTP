using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FTP.Helpers
{
    public enum ServerHashAlgorithm : int
    {
        /// <summary>
        /// HASH command is not supported
        /// </summary>
        NONE = 0,
        /// <summary>
        /// SHA-1
        /// </summary>
        SHA1 = 1,
        /// <summary>
        /// SHA-256
        /// </summary>
        SHA256 = 2,
        /// <summary>
        /// SHA-512
        /// </summary>
        SHA512 = 4,
        /// <summary>
        /// MD5
        /// </summary>
        MD5 = 8,
        /// <summary>
        /// CRC
        /// </summary>
        CRC = 16
    }
}
