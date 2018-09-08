using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorator
{
    public class PCObjectItem : ListViewItem
    {
        public bool IsDirectory { get; }
        public bool IsValid { get; }
        public DirectoryInfo ClosedDirectoryInfo { get; }
        public FileInfo ClosedFileInfo { get; }

        public string FullSourcePath => IsDirectory && IsValid ? ClosedDirectoryInfo.FullName :
            !IsDirectory && IsValid ? ClosedFileInfo.FullName : "";

        public long GetObjectSize => IsDirectory && IsValid ? getDirectoryCompleteSize() :
            !IsDirectory && IsValid ? ClosedFileInfo.Length : 0;

        public PCObjectItem(string filePath, int imageIndex, string imageKey)
        {
            if (Directory.Exists(filePath))
            {
                IsDirectory = true;
                IsValid = true;

                ImageIndex = imageIndex;
                ImageKey = imageKey;

                var di = new DirectoryInfo(filePath);

                ClosedDirectoryInfo = di;

                if (canAccessFolder())
                {
                    //if()
                    Text = di.Name;

                    SubItems.Add("N/A");
                    SubItems.Add(di.CreationTime.ToShortDateString());
                    SubItems.Add(di.Attributes.ToString());
                }
                else
                {
                    IsValid = false;
                }
            }
            else
            {
                if (File.Exists(filePath))
                {
                    IsDirectory = false;
                    IsValid = true;

                    ImageIndex = imageIndex;
                    ImageKey = imageKey;

                    var fi = new FileInfo(filePath);

                    ClosedFileInfo = fi;

                    Text = fi.Name;

                    SubItems.Add(Utils.GetFileSize(fi.Length));
                    SubItems.Add(fi.CreationTime.ToShortDateString());
                    SubItems.Add(fi.Attributes.ToString());
                }
                else
                {
                    IsValid = false;
                    IsDirectory = false;
                }
            }
        }

        long getDirectoryCompleteSize()
        {
            long m = 0;
            foreach (var f in ClosedDirectoryInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                m += f.Length;
            }

            return m;
        }

        bool canAccessFolder()
        {
            try
            {
                var m = ClosedDirectoryInfo.GetDirectories();
                var n = ClosedDirectoryInfo.GetFiles();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
