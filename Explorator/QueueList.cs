using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Utils.CRC;
using FluentFTP;

namespace Explorator
{
    public partial class QueueList : UserControl
    {
        public int SizeName
        {
            get => columnHeader1.Width;
            set => columnHeader1.Width = value;
        }

        public int SizeProgress
        {
            get => columnHeader2.Width;
            set => columnHeader2.Width = value;
        }

        public int SizeSpeed
        {
            get => columnHeader3.Width;
            set => columnHeader3.Width = value;
        }

        public int SizeSize
        {
            get => columnHeader4.Width;
            set => columnHeader4.Width = value;
        }

        public int SizeSource
        {
            get => columnHeader5.Width;
            set => columnHeader5.Width = value;
        }

        public int SizeDestination
        {
            get => columnHeader6.Width;
            set => columnHeader6.Width = value;
        }

        public QueueList()
        {
            InitializeComponent();
        }

        public ListViewItem GetSelected => lvQueue.SelectedItems[0];

        public ListViewItem GetItem(string name)
        {
            foreach (ListViewItem it in lvQueue.Items)
            {
                if (it.Text == name)
                    return it;
            }

            return null;
        }

        public Explorer _explorer1;
        public Explorer _explorer2;

        private List<string> pbars = new List<string>();

        public ListViewEx.ListViewItemCollection GetAllItems => lvQueue.Items;
        public ListViewEx GetInternalListView => this.lvQueue;

        public async Task Transfer(FtpClient connection = null)
        {

        }

        void BeginOpenWriteCallback(IAsyncResult ar)
        {
            var con = ar.AsyncState as FtpClient;
            Stream istream = null, ostream = null;

            var buf = new byte[8192];

        }

        public async Task PrepareTransfers()
        {
            foreach (var allItem in GetAllItems)
            {
                if (allItem is QueueObjectItem)
                    await ((QueueObjectItem) allItem).PrepareTransfer(this);
            }
        }

        public void ChangeProgress(ListViewItem item, long currentByte, bool isPercentage = false)
        {
            var c = (PercentageBar) lvQueue.GetAddedControl(item);
            if (!isPercentage)
                c.CurrentBytes = currentByte;
            else
                c.CurrentBytes = c.MaximumBytes * currentByte / 100;
        }

        public void AddItem(FileInfo fi, string dest)
        {
            var tmp = new ListViewItem(Path.GetFileName(fi.Name));
            tmp.SubItems.Add("");
            tmp.SubItems.Add("0 b/s");
            tmp.SubItems.Add(Utils.GetFileSize(fi.Length));
            tmp.SubItems.Add(fi.FullName);
            //tmp.SubItems.Add(dest + "\\" + Path.GetFileName(fi.Name));
            tmp.SubItems.Add(dest);

            lvQueue.Items.Add(tmp);
            AddProgressBar(tmp, fi.FullName, fi.Length);
        }

        public void AddItem(DirectoryInfo dir, string dest)
        {
            foreach (var f in dir.GetFiles())
            {
                AddItem(f, dest);
            }

            foreach (var di in dir.GetDirectories())
            {
                AddItem(di, dest + "\\" + di.Name);
            }
        }

        public void AddItem(FtpListItem fli, string dest)
        {
            var tmp = new ListViewItem(fli.Name);
            tmp.SubItems.Add("");
            tmp.SubItems.Add("0 b/s");
            tmp.SubItems.Add(Utils.GetFileSize(fli.Size));
            tmp.SubItems.Add(fli.FullName);
            //tmp.SubItems.Add(dest + "\\" + fli.Name);
            tmp.SubItems.Add(dest);

            lvQueue.Items.Add(tmp);
            AddProgressBar(tmp, fli.FullName, fli.Size);
        }

        private void AddProgressBar(ListViewItem item, string fullFileName, long totalSize)
        {
            var pb = new PercentageBar()
            {
                AutomaticallyFormatPercentageToBytes = true,
                ShowPercentage = true,
                MaximumBytes = totalSize,
                Maximum = totalSize,
                Minimum = 0,
                CurrentBytes = 0,
                Name = CRC32
                    .QuickCompute(
                        Encoding.ASCII.GetBytes(fullFileName + totalSize.ToString("X") + DateTime.Now.Ticks.ToString()),
                        SlicingType.SliceBy8).ToString("X8")
            };

            lvQueue.AddControl(pb, item);
        }

        public void AddItemToQueue(object item, string dest)
        {
            if (item is PCObjectItem)
            {
                var x = (PCObjectItem) item;
                if(x.IsDirectory && x.IsValid)
                    AddItem(x.ClosedDirectoryInfo, dest);
                else if (!x.IsDirectory && x.IsValid)
                    AddItem(x.ClosedFileInfo, dest);
            }
        }

        public void AddQueueItem(ListViewItem item, string dest = "", FtpClient destClient = null, FtpClient sourceClient = null)
        {
            if (item is PCObjectItem)
            {
                if (dest.Length == 0)
                {
                    if (destClient == null)
                        throw new IOException("destClient cannot be null!");

                    lvQueue.Items.Add(new QueueObjectItem((PCObjectItem) item, destClient.GetWorkingDirectory(), true,
                        destClient));
                }
                else
                {
                    lvQueue.Items.Add(new QueueObjectItem((PCObjectItem) item, (string)dest));
                }
            }
            else if (item is FTPObjectItem)
            {
                if (destClient == null)
                {
                    if(dest.Length == 0)
                        throw new IOException("dest cannot be null!");
                    if (sourceClient == null)
                        throw new IOException("sourceClient cannot be null!");

                    throw new NotImplementedException("This function has not been properly implemented yet!");
                    //lvQueue.Items.Add(new QueueObjectItem((FTPObjectItem)item, sourceClient, de))
                }
            }
        }

        /*
        public void AddQueueItem(object item, string dest)
        {
            var pb = new PercentageBar();
            pb.AutomaticallyFormatPercentageToBytes = true;
            pb.ShowPercentage = true;

            if (item is FTPObjectItem)
            {
                var f = (FTPObjectItem) item;
                var tmp = new ListViewItem();
                tmp.Text = f.Text;
                pb.Name = CRC32
                    .QuickCompute(Encoding.ASCII.GetBytes(f.Text + dest + DateTime.Now.ToBinary().ToString()),
                        SlicingType.SliceBy8).ToString("X8");
                pbars.Add(pb.Name);
                pb.Maximum = f.ItemInfo.Size;
                pb.MaximumBytes = f.ItemInfo.Size;
                pb.Minimum = 0;
                pb.CurrentBytes = 0;

                tmp.SubItems.Add("");
                tmp.SubItems.Add("0 b/s");
                tmp.SubItems.Add(Utils.GetFileSize(f.ItemInfo.Size));
                tmp.SubItems.Add(f.ItemInfo.FullName);
                tmp.SubItems.Add(dest);
                lvQueue.Items.Add(tmp);
                lvQueue.AddControl(pb, 1, lvQueue.Items.Count - 1);
            }else if (item is PCObjectItem)
            {
                var f = (PCObjectItem) item;
                var tmp = new ListViewItem();
                tmp.Text = f.Text;

                pb.Name = CRC32
                    .QuickCompute(Encoding.ASCII.GetBytes(f.Text + dest + DateTime.Now.ToBinary().ToString()),
                        SlicingType.SliceBy8).ToString("X8");
                pbars.Add(pb.Name);
                if (f.IsDirectory && f.IsValid)
                {
                    //comin l8r
                }
                else if (!f.IsDirectory && f.IsValid)
                {
                    pb.Maximum = f.ClosedFileInfo.Length;
                    pb.MaximumBytes = f.ClosedFileInfo.Length;
                    pb.CurrentBytes = 0;
                    pb.Minimum = 0;

                    tmp.SubItems.Add("");
                    tmp.SubItems.Add("0 b/s");
                    tmp.SubItems.Add(Utils.GetFileSize(f.ClosedFileInfo.Length));
                    tmp.SubItems.Add(f.ClosedFileInfo.FullName);
                    tmp.SubItems.Add(dest);
                    lvQueue.Items.Add(tmp);
                    lvQueue.AddControl(pb, 1, lvQueue.Items.Count - 1);
                }
            }
        }
        */
        private void AddInternalQueue(object item, string dest)
        {
            if (item is PCObjectItem)
            {
                var f = ((PCObjectItem) item).ClosedDirectoryInfo;
                foreach (var dir in f.GetDirectories())
                {
                    
                }
            }
        }
    }
}
