using FluentFTP;

namespace Explorator.CustomListViewItems
{
    public class FTPListViewItem : IObjectItem<FtpListItem>
    {
        public override FtpListItem ItemInfo { get; set; }
        public override bool IsDirectory => ItemInfo.Type == FtpFileSystemObjectType.Directory;
        public override long ItemSize => ItemInfo.Size;
        public override bool IsValid => ItemInfo != null;

        public FTPListViewItem(FtpListItem itemInfo, int imageIndex, string imageKey)
        {
            ImageIndex = imageIndex;
            ImageKey = imageKey;

            ItemInfo = itemInfo;
            Text = ItemInfo.Name;
        }
    }
}
