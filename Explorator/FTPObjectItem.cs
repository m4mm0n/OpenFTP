using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Utils.FTPUtils;
using FluentFTP;

namespace Explorator
{
    internal class FTPObjectItem : ListViewItem
    {
        public bool IsDirectory => ItemInfo.Type == FtpFileSystemObjectType.Directory;
        public bool IsValid { get; }

        public long GetObjectSize => ItemInfo.Size;

        public FtpListItem ItemInfo { get; }

        public FTPObjectItem(FtpListItem itemInfo, int imageIndex, string imageKey)
        {
            ImageIndex = imageIndex;
            ImageKey = imageKey;

            ItemInfo = itemInfo;
            Text = ItemInfo.Name;

            if (!IsDirectory)
            {
                SubItems.Add(Utils.GetFileSize((double) ItemInfo.Size));
                SubItems.Add(ItemInfo.Modified.ToShortDateString());
                SubItems.Add(ItemInfo.Chmod.ToString());
            }
            else if(IsDirectory)
            {
                SubItems.Add("N/A");
                SubItems.Add(ItemInfo.Modified.ToShortDateString());
                SubItems.Add(ItemInfo.Chmod.ToString());
            }
            else
            {
                //most likely link...
                SubItems.Add(Utils.GetFileSize((double)ItemInfo.Size));
                SubItems.Add(ItemInfo.Modified.ToShortDateString());
                SubItems.Add(ItemInfo.RawPermissions);
            }
        }
    }
}
