using System.IO;
using System.Linq;

namespace Explorator.CustomListViewItems
{
    public class PCListViewItem : IObjectItem<FileSystemInfo>
    {
        public override FileSystemInfo ItemInfo { get; set; }
        public override bool IsDirectory => ItemInfo.Attributes == FileAttributes.Directory;
        public override bool IsValid => IsDirectory ? canAccessDir() : canAccessFile();
        public override long ItemSize => getCompleteSize();

        public PCListViewItem(string filePath, int imageIndex, string imageKey)
        {
            ImageIndex = imageIndex;
            ImageKey = imageKey;

            if(Directory.Exists(filePath))
                ItemInfo = new DirectoryInfo(filePath);
            else if(File.Exists(filePath))
                ItemInfo = new FileInfo(filePath);

            Text = ItemInfo.Name;
        }

        private long getCompleteSize()
        {
            if (IsDirectory && IsValid)
                return Directory.GetFiles(ItemInfo.FullName, "*", SearchOption.AllDirectories).Sum(f => new FileInfo(f).Length);
            else
                return new FileInfo(ItemInfo.FullName).Length;
        }
        private bool canAccessFile()
        {
            if (!IsDirectory)
            {
                if (ItemInfo.Exists && ItemInfo.Attributes != FileAttributes.Encrypted)
                {
                    try
                    {
                        var tmp = new FileStream(ItemInfo.FullName, FileMode.Open);

                        tmp.Close();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }

            return false;
        }
        private bool canAccessDir()
        {
            if (IsDirectory)
            {
                if (ItemInfo.Exists && ItemInfo.Attributes != FileAttributes.Encrypted)
                {
                    try
                    {
                        var d = Directory.GetDirectories(ItemInfo.FullName);
                        var f = Directory.GetFiles(ItemInfo.FullName);

                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }

            return false;
        }
    }
}
