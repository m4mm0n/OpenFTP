using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.FTP.Helpers
{
    public enum BinaryMode
    {
        /// <summary>
        /// ASCII is default for upload/reading
        /// </summary>
        ASCII = 0,
        /// <summary>
        /// BINARY is default for download
        /// </summary>
        Binary = 1
    }
}
