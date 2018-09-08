using OpenLogger;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Core.Utils;
using OpenLogger.Enumerations;

namespace Explorator
{
    public class Utils
    {
        private static Logger Log = new Logger(LoggerType.Console, "Explorator.Utils", true);

        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero; iIcon = 0; dwAttributes = 0; szDisplayName = ""; szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            public string szDisplayName;
            public string szTypeName;
        };
        private enum SHGFI
        {
            SmallIcon = 0x00000001,
            OpenIcon = 0x00000002,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            LinkOverlay = 0x00008000,
            UseFileAttributes = 0x00000010
        }

        [DllImport("Shell32.dll")]
        private static extern int SHGetFileInfo(
            string pszPath, uint dwFileAttributes,
            out SHFILEINFO psfi, uint cbfileInfo,
            SHGFI uFlags);

        [DllImport("shell32.dll")]
        private static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder,
            bool fCreate);

        public static string GetRealMyComputer()
        {
            var pathBuilder = new StringBuilder(260);
            var myPcConst = 0x0011;
            SHGetSpecialFolderPath(IntPtr.Zero, pathBuilder, myPcConst, false);

            Console.WriteLine(pathBuilder.ToString());

            return pathBuilder.ToString();
        }

        public static Bitmap GetIcon(string strPath, bool bSmall, bool bOpen)
        {
            Log.Debug("GetIcon -> {3}{0}, {1}, {2}", strPath, bSmall.ToString(), bOpen.ToString(), Environment.NewLine);

            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;

            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon;

            if (bOpen) flags = flags | SHGFI.OpenIcon;

            SHGetFileInfo(strPath, 0, out info, (uint)cbFileInfo, flags);

            using (var ico = Icon.FromHandle(info.hIcon))
            {
                var bm = new Bitmap(ico.Size.Width, ico.Size.Height);
                using (var g = Graphics.FromImage(bm))
                    g.DrawIcon(ico, 0, 0);

                return bm;
            }
        }

        public static Bitmap GetShortcutOverlayIcon(string strPath, bool bSmall, bool bOpen)
        {
            Log.Debug("GetShortcutOverlayIcon -> {3}{0}, {1}, {2}", strPath, bSmall.ToString(), bOpen.ToString(), Environment.NewLine);

            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;

            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon;

            if (bOpen) flags = flags | SHGFI.OpenIcon;

            flags = flags | SHGFI.LinkOverlay;
            SHGetFileInfo(strPath, 0, out info, (uint)cbFileInfo, flags);

            using (var ico = Icon.FromHandle(info.hIcon))
            {
                var bm = new Bitmap(ico.Size.Width, ico.Size.Height);
                using (var g = Graphics.FromImage(bm))
                    g.DrawIcon(ico, 0, 0);

                return bm;
            }
        }

        public static Bitmap GetEmptyIcon(Size size)
        {
            Log.Debug("GetEmptyIcon -> {2}{0}x{1}", size.Width.ToString(), size.Height.ToString(), Environment.NewLine);

            return new Bitmap(size.Width, size.Height);
        }

        public static string GetFileSize(double byteCount)
        {
            Log.Debug("GetFileSize -> {1}{0}", byteCount.ToString(), Environment.NewLine);

            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }

        public static Icon GetIconDirectoryUp => Icons.GetShellIcon(Shell32Icons.DIRECTORY_UP);

        public static Icon GetFolderMoveUpIcon()
        {
            Log.Debug("GetFolderMoveUpIcon -> {0}N/A", Environment.NewLine);

            var x =
                "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAQAAAC1+jfqAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADdcAAA3XAUIom3gAAAAHdElNRQfiCBAHCgQP/DZhAAAAmUlEQVQoz4WRsQ2EMAxFH4jidDMgXcZgEXqmOFHCJtRMkS7FFXRXB7EEHRTBAQIKdvXjJ/vbgYdIgDffTXXYO6hk2dKiwmIGlF590HSnepMBP/4xDzWviMcZjHdwlybFRLe8ACPjGYD80NCiUNjDSw4wBVfYkcm16S9HEqSH1M0BNJUHKrQ4ACj8RAnRhTv1QBssJ3p4+mtgBYVhTUVs0zzEAAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDE4LTA4LTE2VDA3OjEwOjA0KzAyOjAw/Bfz7QAAACV0RVh0ZGF0ZTptb2RpZnkAMjAxOC0wOC0xNlQwNzoxMDowNCswMjowMI1KS1EAAAAZdEVYdFNvZnR3YXJlAHd3dy5pbmtzY2FwZS5vcmeb7jwaAAAAAElFTkSuQmCC";
            using (var y = new MemoryStream(Convert.FromBase64String(x)))
            using (var z = new Bitmap(y))
            {
                z.MakeTransparent();
                return Icon.FromHandle(z.GetHicon());
            }
        }

        public static bool IsDirectory(string filePath) => Directory.Exists(filePath);
        public static bool IsFile(string fileName) => File.Exists(fileName);

        public static void SetTerminalFont(IntPtr handle)
        {
            var hFnt = GetStockObject(10);
            SendMessage(handle.ToInt32(), 0x30, hFnt, true);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int GetStockObject(int nIndex);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SendMessage(int hWnd, int wMsg, int wParam, bool lParam);
    }
}
