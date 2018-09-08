using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Utils.CRC;
using FluentFTP;

namespace Explorator
{
    public class QueueObjectItem : ListViewItem
    {
        public event EventHandler<ProgressExEventArgs> OnProgressChanged;

        public ListViewItem FileObject;
        public long TotalSize;

        protected virtual void AddProgress(long cur, long max, string speed)
        {
            OnProgressChanged?.Invoke(this, new ProgressExEventArgs(cur, max, speed));
        }

        private FtpClient _client = null;
        private FtpClient _client2 = null;
        private bool isUpload = false;

        private string finalDestinationPath = "";
        private string rootDirFtp = "";

        public async Task PrepareTransfer(QueueList owner)
        {
            var originalItem = this;

            //Upload
            if (_client != null && _client2 == null && finalDestinationPath.Length > 0 && isUpload &&
                FileObject is PCObjectItem)
            {
                var o = (PCObjectItem) FileObject;
                this.rootDirFtp = o.IsDirectory ? o.ClosedDirectoryInfo.Parent.FullName : o.FullSourcePath;

                //this.rootDirFtp = this.rootDirFtp.Replace("\\", "/");

                if (o.IsDirectory && o.IsValid)
                {
                    owner.GetInternalListView.Items.Remove(originalItem);
                    var allFiles = o.ClosedDirectoryInfo.GetFiles("*", SearchOption.AllDirectories).ToList()
                        .OrderByDescending(x => x.FullName).ToList();
                    foreach (var f in allFiles)
                    {
                        if (finalDestinationPath == "/")
                        {
                            owner.AddItem(f, getUploadDirectory(f, this.rootDirFtp) + "\\" + f.Name);
                        }
                        else
                        {
                            owner.AddItem(f,
                                finalDestinationPath + "\\" + getUploadDirectory(f, this.rootDirFtp) + "\\" + f.Name);

                        }
                    }
                }else if (!o.IsDirectory && o.IsValid)
                {
                    owner.GetInternalListView.Items.Remove(originalItem);
                    owner.AddItem(o.ClosedFileInfo, finalDestinationPath);
                }
            }
            //Download
            if (_client != null && _client2 == null && finalDestinationPath.Length > 0 && !isUpload &&
                FileObject is FTPObjectItem)
            {
                var o = (FTPObjectItem) FileObject;

                if (o.IsDirectory)
                {

                }
            }
        }

        private async Task getFilesInServerDirectory(string path, QueueList owner)
        {
            await _client.SetWorkingDirectoryAsync(path);
            foreach (var item in await _client.GetListingAsync())
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                    await getFilesInServerDirectory(item.FullName, owner);
                else
                    owner.AddItem(item, finalDestinationPath);
            }
        }

        private async Task makeUploadDirectories(string toMake)
        {
            var dirs = toMake.Split('\\');
            var orgDir = _client.GetWorkingDirectory();
            var prevDir = orgDir;
            foreach (var dir in dirs)
            {
                await _client.CreateDirectoryAsync(dir);
                prevDir += "\\" + dir;
                _client.SetWorkingDirectory(prevDir);
            }

            await _client.SetWorkingDirectoryAsync(orgDir);
        }

        private string getUploadDirectory(FileInfo finfo, string rootDir)
        {
            var dinfo = finfo.Directory;
            return dinfo.FullName.Replace(rootDir, "");
        }

        public QueueObjectItem(ListViewItem objectToQueue, string destination, bool isDestFtp = false, FtpClient destClient = null)
        {
            FileObject = objectToQueue;
            if (isDestFtp && destClient != null)
            {
                _client = destClient;
            }

            finalDestinationPath = destination;
            if (FileObject is PCObjectItem)
            {
                this.Text = ((PCObjectItem) FileObject).Text;
                this.SubItems.Add("Queued");
                this.SubItems.Add("");
                this.SubItems.Add(Utils.GetFileSize(((PCObjectItem) FileObject).GetObjectSize));
                this.SubItems.Add(((PCObjectItem) FileObject).FullSourcePath);
                this.SubItems.Add(finalDestinationPath);

                isUpload = true;
            }
            else if (FileObject is FTPObjectItem && !isDestFtp)
            {
                this.Text = ((FTPObjectItem) FileObject).Text;
                this.SubItems.Add("Queued");
                this.SubItems.Add("");
                this.SubItems.Add(Utils.GetFileSize(((FTPObjectItem) FileObject).GetObjectSize));
                this.SubItems.Add(((FTPObjectItem) FileObject).ItemInfo.FullName);
                this.SubItems.Add(finalDestinationPath);

                isUpload = false;
            }
            else
                throw new IOException("FileObject's format is not reckognized!");
        }

        public QueueObjectItem(ListViewItem objectToQueue, FtpClient sourceClient, FtpClient destClient)
        {
            FileObject = objectToQueue;
            if (sourceClient == null)
                throw new IOException("Source client cannot be null!");
            if (destClient == null)
                throw new IOException("Destination client cannot be null!");

            if(sourceClient.IsDisposed)
                throw new AccessViolationException("Source client cannot be disposed!");
            if(destClient.IsDisposed)
                throw new AccessViolationException("Destination client cannot be disposed!");
            if(!sourceClient.IsConnected)
                throw new AccessViolationException("Source client cannot be disconnected!");
            if(!destClient.IsConnected)
                throw new AccessViolationException("Destination client cannot be disconnected!");

            _client = destClient;
            _client2 = sourceClient;

            if (FileObject is FTPObjectItem)
            {
                this.Text = ((FTPObjectItem) FileObject).Text;
                this.SubItems.Add("Queued");
                this.SubItems.Add("");
                this.SubItems.Add(Utils.GetFileSize(((FTPObjectItem) FileObject).GetObjectSize));
                this.SubItems.Add(((FTPObjectItem) FileObject).ItemInfo.FullName);
                this.SubItems.Add(destClient.GetWorkingDirectory());
            }
            else
                throw new IOException("FileObject's format MUST be an FTPObjectItem!");
        }

        //public QueueObjectItem()
    }
}
