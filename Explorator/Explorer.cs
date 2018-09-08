using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Utils;
using FluentFTP;
using OpenLogger;
using OpenLogger.Enumerations;

namespace Explorator
{
    public enum SpecialFolders : uint
    {
        //UserProfiles,
        DesktopDir_All,
        ApplicationData_All,
        MyDocuments_All,
        MyFavorites_All,
        MyMusic_All,
        MyPictures_All,
        //StartMenu_All,
        MyVideos_All ,

        Desktop,
        DekstopDir,
        MyComputer,
        MyFavorites,
        ApplicationData,
        MyDocuments,
        MyMusic,
        MyPictures,
        MyVideos,
        //MyNetworkPlaces,
        MyDocumentsDir,
        //StartMenu,

        //ControlPanel,
        //Printers,
        ProgramFiles,
        //SendTo,
        //System,
        //Windows,
        //RecycleBin,

        Other
    }

    public partial class Explorer : UserControl
    {
        private Logger Log = new Logger(LoggerType.Console, "Explorator.Explorer", true);

        public int SizeName
        {
            get => columnHeader1.Width;
            set => columnHeader1.Width = value;
        }

        public int SizeSize
        {
            get => columnHeader2.Width;
            set => columnHeader2.Width = value;
        }

        public int SizeDate
        {
            get => columnHeader3.Width;
            set => columnHeader3.Width = value;
        }

        public int SizeAttrib
        {
            get => columnHeader4.Width;
            set => columnHeader4.Width = value;
        }
        [Browsable(true)]
        public SpecialFolders StartupFolder
        {
            get => startupFolder;
            set
            {
                startupFolder = value;

                switch (value)
                {
                    case SpecialFolders.ApplicationData:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        break;
                    case SpecialFolders.ApplicationData_All:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        break;
                    case SpecialFolders.DekstopDir:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        break;
                    case SpecialFolders.DesktopDir_All:
                        currentFolder =
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                        break;
                    case SpecialFolders.Desktop:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        break;
                    case SpecialFolders.MyComputer:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                        break;
                    case SpecialFolders.MyDocuments:
                    case SpecialFolders.MyDocumentsDir:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        break;
                    case SpecialFolders.MyDocuments_All:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
                        break;
                    case SpecialFolders.MyMusic:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                        break;
                    case SpecialFolders.MyMusic_All:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
                        break;
                    case SpecialFolders.MyFavorites:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
                        break;
                    case SpecialFolders.MyPictures:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                        break;
                    case SpecialFolders.MyPictures_All:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
                        break;
                    case SpecialFolders.MyVideos:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                        break;
                    case SpecialFolders.MyVideos_All:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
                        break;
                    case SpecialFolders.ProgramFiles:
                        currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                        break;
                }
            }
        }

        private SpecialFolders startupFolder = SpecialFolders.MyDocuments;

        [Browsable(true)]
        public string StartupOther
        {
            get => otherFolder;
            set
            {
                if (startupFolder == SpecialFolders.Other)
                {
                    otherFolder = value;
                    currentFolder = value;
                }
            }
        }
        private string otherFolder;

        public bool IsPcExplorer { get; set; } = true;
        public Explorer OtherExplorer { get; set; }
        public FtpClient ftpClient;
        public QueueList queueList;

        public ListViewItem SelectedItem =>
            lvDirectoryList.SelectedIndices.Count != 0 ? lvDirectoryList.SelectedItems[0] : null;

        private string currentFolder;
        public Explorer()
        {
            InitializeComponent();

            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (IsPcExplorer)
            {
                ctxAdvancedTransfers.Visible = false;
                ctxChangePerm.Visible = false;
                if (lvDirectoryList.SelectedIndices.Count > 1)
                {
                    ctxQuickView.Enabled = false;
                }
                else
                {
                    ctxQuickView.Enabled = true;
                }
            }
            else
            {
                ctxQuickView.Visible = false;
                if (lvDirectoryList.SelectedIndices.Count > 1)
                {
                    ctxChangePerm.Enabled = false;
                }
                else
                {
                    ctxChangePerm.Enabled = true;
                }
            }
        }

        public void RefreshCurrentPcFolder()
        {
                if (Directory.Exists(currentFolder))
                {
                    lvDirectoryList.Items.Clear();

                    var allDirsFiles = ListCurrentPcDirectory();
                    var imgLst = GetIcons(allDirsFiles);

                    lvDirectoryList.SmallImageList = imgLst;
                    lvDirectoryList.LargeImageList = imgLst;

                    lvDirectoryList.Items.Add(new ListViewItem("...", "folderMoveUp"));

                    for (var i = 0; i < allDirsFiles.Length - 1; i++)
                    {
                        var z = new PCObjectItem(allDirsFiles[i], i + 1, imgLst.Images.Keys[i + 1]);
                        if (z.IsValid)
                            lvDirectoryList.Items.Add(z);
                    }
                }
        }

        public async Task RefreshView()
        {
            if (IsPcExplorer)
                RefreshCurrentPcFolder();
            else
                await RefreshCurrentFtpFolder();
        }

        public async Task RefreshCurrentFtpFolder(bool isFirstTime = false)
        {
            if (isFirstTime)
                currentFolder = string.Empty;
            if (ftpClient != null && !ftpClient.IsDisposed && ftpClient.IsConnected)
            {
                lvDirectoryList.Items.Clear();

                var allDirsFiles = await ftpClient.GetListingAsync(currentFolder);

                var dirsOnly = allDirsFiles.Where(item => item.Type == FtpFileSystemObjectType.Directory).ToList();
                var filesOnly = allDirsFiles.Where(item => item.Type == FtpFileSystemObjectType.File).ToList();
                var linksOnly = allDirsFiles.Where(item => item.Type == FtpFileSystemObjectType.Link).ToList();

                var fullList = new List<FtpListItem>();
                if(dirsOnly.Any())
                    fullList.AddRange(dirsOnly);
                if(filesOnly.Any())
                    fullList.AddRange(filesOnly);
                if(linksOnly.Any())
                    fullList.AddRange(linksOnly);

                var imgLst = GetIcons(fullList.ToArray());

                lvDirectoryList.SmallImageList = imgLst;
                lvDirectoryList.LargeImageList = imgLst;

                lvDirectoryList.Items.Add(new ListViewItem("...", "folderMoveUp"));

                for (var i = 0; i < fullList.Count; i++)
                {
                    var z = new FTPObjectItem(fullList[i], i + 1, imgLst.Images.Keys[i + 1]);
                    lvDirectoryList.Items.Add(z);
                }

                lvDirectoryList.Refresh();
            }
        }

        string[] ListCurrentPcDirectory()
        {
            var lst = new List<string>();

            try
            {
                foreach (var dir in Directory.GetDirectories(currentFolder))
                {
                    lst.Add(dir);
                }
            }
            catch (UnauthorizedAccessException ave)
            {
                Log.Exception(ave, "Unauthorized Access Exception!");
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "General Exception!");
            }

            try
            {
                foreach (var file in Directory.GetFiles(currentFolder))
                {
                    lst.Add(file);
                }
            }
            catch (AccessViolationException ave)
            {
                Log.Exception(ave, "Access Denied Exception!");
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "General Exception!");
            }

            return lst.ToArray();
        }

        ImageList GetIcons(FtpListItem[] lstToRead)
        {
            var m = new ImageList()
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };
            m.Images.Add("folderMoveUp", Utils.GetIconDirectoryUp);
            var folderIcon = Utils.GetIcon(Path.GetTempPath(), true, false);
            var folderShortcutIcon = Utils.GetShortcutOverlayIcon(Path.GetTempPath(), true, false);

            foreach (var item in lstToRead)
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                    m.Images.Add(item.FullName, folderIcon);
                else if (item.Type == FtpFileSystemObjectType.File)
                    m.Images.Add(item.FullName,
                        getPrefixIcon(Path.GetExtension(item.Name)));
                else if (item.Type == FtpFileSystemObjectType.Link)
                {
                    if (item.LinkObject.Type == FtpFileSystemObjectType.Directory)
                        m.Images.Add(item.FullName, folderShortcutIcon);
                    else if (item.LinkObject.Type == FtpFileSystemObjectType.File)
                        m.Images.Add(item.FullName, Icons.AddShortcutOverlay(getPrefixIcon(Path.GetExtension(item.Name))));
                }
            }

            return m;
        }
        ImageList GetIcons(string[] lstToRead)
        {
            var m = new ImageList();
            m.ImageSize = new Size(16, 16);
            m.ColorDepth = ColorDepth.Depth32Bit;

            //m.Images.Add("folderMoveUp", Utils.GetFolderMoveUpIcon());
            m.Images.Add("folderMoveUp", Utils.GetIconDirectoryUp);

            foreach (var s in lstToRead)
            {
                Bitmap imgY = null;
                if (!Utils.IsDirectory(s) && Utils.IsFile(s))
                {
                    try
                    {
                        imgY = Icons.GetExtensionIcon(prefixIcons["." + Path.GetExtension(s)].ToString()).ToBitmap();
                    }
                    catch
                    {
                        imgY = null;
                    }
                }

                if (imgY == null)
                {
                    var x = Utils.GetIcon(s, true, false);
                    if (x == null)
                    {
                        x = Utils.GetEmptyIcon(new Size(16, 16));
                    }

                    if (Utils.IsDirectory(s))
                        m.Images.Add(new DirectoryInfo(s).Name, x);
                    else if (Utils.IsFile(s))
                        m.Images.Add(Path.GetFileName(s), x);
                }
                else
                {
                    m.Images.Add(Path.GetFileName(s), imgY);
                }
            }

            return m;
        }

        Image getPrefixIcon(string extName)
        {
            //try
            //{
            //    return Icons.GetExtensionIcon(prefixIcons["." + extName].ToString()).ToBitmap();
            //}
            //catch(Exception ex)
            //{
            //    return Icons.GetShellIcon(Shell32Icons.DEFAULT_DOCUMENT).ToBitmap();
            //}
            return prefixIcons.Contains(extName)
                ? Icons.GetExtensionIcon(prefixIcons[extName].ToString()).ToBitmap()
                : Icons.GetShellIcon(Shell32Icons.UNKNOWN_FILE_TYPE).ToBitmap();
        }
        private Hashtable prefixIcons = Icons.GetFileTypeAndIcon();

        bool isHighestOnDrive()
        {
            return Environment.GetLogicalDrives().Contains(currentFolder);
        }

        async void ListFtpRoot()
        {
            currentFolder = "";
            await RefreshCurrentFtpFolder();
        }

        void ListMyDesktop()
        {
            currentFolder = "explorator::my<>desktop"; //just to be sure its illegal characters!

            lvDirectoryList.Items.Clear();
            var m = new ImageList();

            m.Images.Add("my<>computer",
                Core.Utils.Icons.GetShellIcon(Shell32Icons.MY_COMPUTER));
            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), true, false));
            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), true, false));
            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), true, false));
            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), true, false));

            var files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            foreach (var file in files)
            {
                m.Images.Add(Path.GetFileName(file), Utils.GetIcon(file, true, false));
            }

            lvDirectoryList.SmallImageList = m;
            lvDirectoryList.LargeImageList = m;

            lvDirectoryList.Items.Add(
                "My Computer", 0);
            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 1,
                m.Images.Keys[1]));
            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), 2,
                m.Images.Keys[2]));
            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), 3,
                m.Images.Keys[3]));
            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 4,
                m.Images.Keys[4]));

            foreach (var file in files)
            {
                lvDirectoryList.Items.Add(new PCObjectItem(file, m.Images.IndexOfKey(Path.GetFileName(file)),
                    Path.GetFileName(file)));
            }
        }

        void ListMyComputer()
        {
            currentFolder = "explorator::my<>computer"; //just to be sure its illegal characters!

            lvDirectoryList.Items.Clear();
            var m = new ImageList();

            //m.Images.Add("folderMoveUp", Utils.GetFolderMoveUpIcon());
            m.Images.Add("folderMoveUp", Utils.GetIconDirectoryUp);

            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), true, false));
            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), true, false));
            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), true, false));
            m.Images.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)).Name,
                Utils.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), true, false));

            var drives = Environment.GetLogicalDrives();
            foreach (var drive in drives)
            {
                m.Images.Add(new DirectoryInfo(drive).Name, Utils.GetIcon(drive, true, false));
            }

            lvDirectoryList.SmallImageList = m;
            lvDirectoryList.LargeImageList = m;

            lvDirectoryList.Items.Add(new ListViewItem("...", "folderMoveUp"));

            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 1,
                m.Images.Keys[1]));
            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), 2,
                m.Images.Keys[2]));
            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), 3,
                m.Images.Keys[3]));
            lvDirectoryList.Items.Add(new PCObjectItem(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 4,
                m.Images.Keys[4]));

            foreach (var drive in drives)
            {
                lvDirectoryList.Items.Add(new PCObjectItem(drive, m.Images.IndexOfKey(drive), drive));
            }
        }

        private void Explorer_Load(object sender, EventArgs e)
        {
            lvDirectoryList.MouseDoubleClick += LvDirectoryList_MouseDoubleClick;
        }

        private async void LvDirectoryList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(IsPcExplorer)
            {
                if (currentFolder != string.Empty)
                {
                    if (lvDirectoryList.SelectedItems[0].ImageKey == "folderMoveUp")
                    {
                        if (isHighestOnDrive())
                        {
                            ListMyComputer();
                        }
                        else if (currentFolder == "explorator::my<>computer")
                        {
                            currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            ListMyDesktop();
                        }
                        else if(currentFolder != Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
                        {
                            currentFolder = new DirectoryInfo(currentFolder).Parent.FullName;
                            RefreshCurrentPcFolder();
                        }
                    }
                    else if (lvDirectoryList.SelectedItems[0] is PCObjectItem &&
                             ((PCObjectItem) lvDirectoryList.SelectedItems[0]).IsDirectory)
                    {
                        currentFolder = ((PCObjectItem) lvDirectoryList.SelectedItems[0]).ClosedDirectoryInfo.FullName;
                        RefreshCurrentPcFolder();
                    }
                    else if (lvDirectoryList.SelectedItems[0] is PCObjectItem &&
                             !((PCObjectItem) lvDirectoryList.SelectedItems[0]).IsDirectory)
                    {
                        Process.Start("explorer",
                            ((PCObjectItem) lvDirectoryList.SelectedItems[0]).ClosedFileInfo.FullName);
                    }
                    else if (lvDirectoryList.SelectedItems[0].Text == "My Computer")
                    {
                        ListMyComputer();
                    }
                }
            }
            else
            {
                FtpReply rep;

                if (lvDirectoryList.SelectedIndices.Count == 1)
                {
                    Log.Debug("CurrentFolder: {0}", currentFolder);

                    if (lvDirectoryList.SelectedItems[0].ImageKey == "folderMoveUp")
                    {
                        if (currentFolder != string.Empty)
                        {
                            if ((rep = await ftpClient.ExecuteAsync("cdup")).Success)
                            {
                                currentFolder = ftpClient.GetWorkingDirectory();
                                await RefreshCurrentFtpFolder();
                            }
                        }
                        else
                        {
                            await RefreshCurrentFtpFolder();
                        }
                    }
                    else
                    {
                        if (lvDirectoryList.SelectedItems[0] is FTPObjectItem &&
                            ((FTPObjectItem) lvDirectoryList.SelectedItems[0]).IsDirectory)
                        {
                            await ftpClient.SetWorkingDirectoryAsync(((FTPObjectItem) lvDirectoryList.SelectedItems[0])
                                .ItemInfo.FullName);
                            currentFolder = ftpClient.GetWorkingDirectory();
                            await RefreshCurrentFtpFolder();
                        }
                    }
                }
            }
        }

        private void quickViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvDirectoryList.SelectedItems.Count == 1 && lvDirectoryList.SelectedItems[0] is PCObjectItem &&
                !((PCObjectItem) lvDirectoryList.SelectedItems[0]).IsDirectory)
            {
                using (var m = new frmFileViewer(lvDirectoryList.SelectedItems[0]))
                    m.ShowDialog();
            }
        }

        private void ctxTransfer_Click(object sender, EventArgs e)
        {
            if (OtherExplorer != null)
            {
                if (OtherExplorer.IsPcExplorer)
                {
                    //here it comes soon!
                    throw new NotImplementedException("This method has yet to be implemented!");
                }
                else
                {
                    //also soon!
                    throw new NotImplementedException("This method has yet to be implemented!");
                }
            }
        }

        private void ctxQueue_Click(object sender, EventArgs e)
        {
            if (OtherExplorer != null)
            {
                if (OtherExplorer.IsPcExplorer)
                {
                    //here it comes soon!
                    throw new NotImplementedException("This method has yet to be implemented!");
                }
                else
                {
                    //also soon!
                    foreach (ListViewItem item in lvDirectoryList.SelectedItems)
                    {
                        queueList.AddQueueItem(item, "", OtherExplorer.ftpClient);
                    }
                }
            }
        }

        private void ctxTransferAs_Click(object sender, EventArgs e)
        {
            if (OtherExplorer != null)
            {
                if (OtherExplorer.IsPcExplorer)
                {
                    //here it comes soon!
                    throw new NotImplementedException("This method has yet to be implemented!");
                }
                else
                {
                    //also soon!
                    throw new NotImplementedException("This method has yet to be implemented!");
                }
            }
        }

        private void ctxQueueAs_Click(object sender, EventArgs e)
        {
            if (OtherExplorer != null)
            {
                if (OtherExplorer.IsPcExplorer)
                {
                    //here it comes soon!
                    throw new NotImplementedException("This method has yet to be implemented!");
                }
                else
                {
                    //also soon!
                    throw new NotImplementedException("This method has yet to be implemented!");
                }
            }
        }

        private async void ctxRename_Click(object sender, EventArgs e)
        {
            using (var f = new FrmRenamer(lvDirectoryList.SelectedItems[0].Text))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    if (IsPcExplorer)
                    {
                        try
                        {
                            var m = (PCObjectItem)lvDirectoryList.SelectedItems[0];
                            Log.Debug(m.ClosedDirectoryInfo.FullName.Substring(0, m.ClosedDirectoryInfo.FullName.Length - m.ClosedDirectoryInfo.Name.Length -1));

                            if (m.IsDirectory)
                                m.ClosedDirectoryInfo.MoveTo(m.ClosedDirectoryInfo.FullName.Substring(0, m.ClosedDirectoryInfo.FullName.Length - m.ClosedDirectoryInfo.Name.Length - 1) + "\\" + f.Result);
                            else if (!m.IsDirectory && m.IsValid)
                                m.ClosedFileInfo.MoveTo(f.Result);
                                
                            RefreshCurrentPcFolder();
                        }
                        catch (UnauthorizedAccessException uae)
                        {
                            throw uae;
                        }
                        catch (AccessViolationException ave)
                        {
                            throw ave;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                    {
                        if (ftpClient != null && !ftpClient.IsDisposed && ftpClient.IsConnected)
                        {
                            var m = (FTPObjectItem) lvDirectoryList.SelectedItems[0];
                            try
                            {
                                await ftpClient.RenameAsync(m.ItemInfo.Name, f.Result);
                                await RefreshCurrentFtpFolder();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }
        }

        private async void ctxDelete_Click(object sender, EventArgs e)
        {
            if (IsPcExplorer)
            {
                //language change text l8r
                if (Core.Utils.Utils.MsgBox("Are you sure you wish to delete the selected directories/files?",
                        "Delete Selected", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        //if (lvDirectoryList.SelectedIndices.Count == 1 &&
                        //    lvDirectoryList.SelectedItems[0] is PCObjectItem)
                        //{
                        //    var m = (PCObjectItem) lvDirectoryList.SelectedItems[0];
                        //    if(m.IsDirectory && m.IsValid)
                        //        Directory.Delete(m.ClosedDirectoryInfo.FullName);
                        //    else if(!m.IsDirectory && m.IsValid)
                        //        File.Delete(m.ClosedFileInfo.FullName);

                        //    RefreshCurrentPcFolder();
                        //}
                        if (lvDirectoryList.SelectedIndices.Count > 0)
                        {
                            foreach (var item in lvDirectoryList.SelectedItems)
                            {
                                if (item is PCObjectItem && ((PCObjectItem) item).IsValid)
                                {
                                    if (((PCObjectItem) item).IsDirectory)
                                        Directory.Delete(((PCObjectItem) item).ClosedDirectoryInfo.FullName);
                                    else
                                        File.Delete(((PCObjectItem) item).ClosedFileInfo.FullName);
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        Log.Log(LogType.Failure, "Failed to delete due to unauthorized access!");
                    }
                }
            }
            else
            {
                if (Core.Utils.Utils.MsgBox("Are you sure you wish to delete the selected directories/files?",
                        "Delete Selected", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        //if (ftpClient != null && !ftpClient.IsDisposed && ftpClient.IsConnected &&
                        //    lvDirectoryList.SelectedIndices.Count == 1 &&
                        //    lvDirectoryList.SelectedItems[0] is FTPObjectItem)
                        //{
                        //    var m = (FTPObjectItem) lvDirectoryList.SelectedItems[0];
                        //    if (m.IsDirectory)
                        //        await ftpClient.DeleteDirectoryAsync(m.ItemInfo.FullName);
                        //    else
                        //        await ftpClient.DeleteFileAsync(m.ItemInfo.FullName);

                        //    await RefreshCurrentFtpFolder();
                        //}
                        if (ftpClient != null && !ftpClient.IsDisposed && ftpClient.IsConnected &&
                            lvDirectoryList.SelectedIndices.Count > 0)
                        {
                            foreach (var item in lvDirectoryList.SelectedItems)
                            {
                                if (item is FTPObjectItem)
                                {
                                    if (((FTPObjectItem) item).IsDirectory)
                                        await ftpClient.DeleteDirectoryAsync(((FTPObjectItem) item).ItemInfo.FullName);
                                    else
                                        await ftpClient.DeleteFileAsync(((FTPObjectItem) item).ItemInfo.FullName);
                                }

                                await RefreshCurrentFtpFolder();
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private void ctxMakeDir_Click(object sender, EventArgs e)
        {
            if (IsPcExplorer)
            {
                
            }
        }

        private void ctxCopyNames_Click(object sender, EventArgs e)
        {
            if (lvDirectoryList.SelectedIndices.Count > 0)
            {
                var toCopyLst = "";
                foreach (var item in lvDirectoryList.SelectedItems)
                {
                    toCopyLst += ((ListViewItem)item).Text + Environment.NewLine;
                }
                
                Clipboard.Clear();
                Clipboard.SetText(toCopyLst);
            }
        }
    }
}
