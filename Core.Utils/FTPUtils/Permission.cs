namespace Core.Utils.FTPUtils
{
    public enum Permission : uint
    {
        /// <summary>
        /// No access
        /// </summary>
        None = 0,
        /// <summary>
        /// Executable
        /// </summary>
        Execute = 1,
        /// <summary>
        /// Writable
        /// </summary>
        Write = 2,
        /// <summary>
        /// Readable
        /// </summary>
        Read = 4
    }
}
