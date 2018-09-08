using System;

namespace Core.Utils.FTPUtils
{
    public struct ObjectInformation
    {
        public long TotalSize;
        public bool IsExtensionKnown;
        public bool IsDirectory;
        public string Name;
        public string Owner;
        public string Group;
        public DateTime Created;
        public DateTime Modified;
        public Permission OwnerPermissions;
        public Permission GroupPermissions;
    }
}
