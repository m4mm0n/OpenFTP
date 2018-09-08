using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Core.Utils
{
    public enum Shell32Icons : int
    {
        UNKNOWN_FILE_TYPE = 0,
        DEFAULT_DOCUMENT = 1,
        DEFAULT_APPLICATION = 2,
        FOLDER_CLOSED = 3,
        FOLDER_OPEN = 4,
        MY_COMPUTER = 15,
        NETWORK_NEIGHBORHOOD = 17,
        SHORTCUT_LNK = 29,
        DESKTOP = 34,
        EXCLAMATION = 77,
        FTP_DIRECTORY = 85,
        PLAY = 137,
        DIRECTORY_UP = 146,
        PREVIEW_SPYGLASS = 171,

    }
    public struct EmbeddedIconInfo
    {
        public string FileName;
        public int IconIndex;
    }
    public static class Icons
    {
        /// <summary>
        /// Icon by Idwar Halid
        /// </summary>
        public static Bitmap TwitterIcon => getImageFromBase64(
            "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");
        /// <summary>
        /// Icon by Idwar Halid
        /// </summary>
        public static Bitmap RSSIcon => getImageFromBase64(
            "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");
        /// <summary>
        /// Icon by Idwar Halid
        /// </summary>
        public static Bitmap GitHubIcon => getImageFromBase64(
            "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");
        /// <summary>
        /// Icon by Idwar Halid
        /// </summary>
        public static Bitmap EmailIcon => getImageFromBase64(
            "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");

        public static Icon GetExtensionIcon(string fileExt)
        {
            try
            {
                //return Icon.ExtractAssociatedIcon(fileExt);
                var emb = getEmbeddedIconInfo(fileExt);
                return Icon.FromHandle(ExtractIcon(0, emb.FileName, emb.IconIndex));
            }
            catch
            {
                return GetShellIcon(Shell32Icons.UNKNOWN_FILE_TYPE);
            }
        }

        public static Image AddShortcutOverlay(Image toAddShort)
        {
            var overlayIco = GetShellIcon(Shell32Icons.SHORTCUT_LNK);
            var overlayImg = overlayIco.ToBitmap();

            using (var g = Graphics.FromImage(toAddShort))
            {
                g.DrawImage(overlayImg, 0, 0);
                return toAddShort;
            }
        }

        public static Icon AddShortcutOverlay(Icon toAddShortcutTo)
        {
            var overlay = GetShellIcon(Shell32Icons.SHORTCUT_LNK);
            var overlayBitmap = overlay.ToBitmap();
            var toWriteOn = toAddShortcutTo.ToBitmap();
            using (var g = Graphics.FromImage(toAddShortcutTo.ToBitmap()))
            {
                var b = overlay.ToBitmap();
                b.MakeTransparent();
                g.DrawImage(b, 0, 0);

                return Icon.FromHandle(b.GetHbitmap());
            }
        }

        static EmbeddedIconInfo getEmbeddedIconInfo(string fileAndParam)
        {
            EmbeddedIconInfo embeddedIcon = new EmbeddedIconInfo();

            if (String.IsNullOrEmpty(fileAndParam))
                return embeddedIcon;

            //Use to store the file contains icon.
            string fileName = String.Empty;

            //The index of the icon in the file.
            int iconIndex = 0;
            string iconIndexString = String.Empty;

            int commaIndex = fileAndParam.IndexOf(",");
            //if fileAndParam is some thing likes that: "C:\\Program Files\\NetMeeting\\conf.exe,1".
            if (commaIndex > 0)
            {
                fileName = fileAndParam.Substring(0, commaIndex);
                iconIndexString = fileAndParam.Substring(commaIndex + 1);
            }
            else
                fileName = fileAndParam;

            if (!String.IsNullOrEmpty(iconIndexString))
            {
                //Get the index of icon.
                iconIndex = int.Parse(iconIndexString);
                if (iconIndex < 0)
                    iconIndex = 0;  //To avoid the invalid index.
            }

            embeddedIcon.FileName = fileName;
            embeddedIcon.IconIndex = iconIndex;

            return embeddedIcon;
        }

        public static Hashtable GetFileTypeAndIcon()
        {
            try
            {
                // Create a registry key object to represent the HKEY_CLASSES_ROOT registry section
                RegistryKey rkRoot = Registry.ClassesRoot;

                //Gets all sub keys' names.
                string[] keyNames = rkRoot.GetSubKeyNames();
                Hashtable iconsInfo = new Hashtable();

                //Find the file icon.
                foreach (string keyName in keyNames)
                {
                    if (String.IsNullOrEmpty(keyName))
                        continue;
                    int indexOfPoint = keyName.IndexOf(".");

                    //If this key is not a file exttension(eg, .zip), skip it.
                    if (indexOfPoint != 0)
                        continue;

                    RegistryKey rkFileType = rkRoot.OpenSubKey(keyName);
                    if (rkFileType == null)
                        continue;

                    //Gets the default value of this key that contains the information of file type.
                    object defaultValue = rkFileType.GetValue("");
                    if (defaultValue == null)
                        continue;

                    //Go to the key that specifies the default icon associates with this file type.
                    string defaultIcon = defaultValue.ToString() + "\\DefaultIcon";
                    RegistryKey rkFileIcon = rkRoot.OpenSubKey(defaultIcon);
                    if (rkFileIcon != null)
                    {
                        //Get the file contains the icon and the index of the icon in that file.
                        object value = rkFileIcon.GetValue("");
                        if (value != null)
                        {
                            //Clear all unecessary " sign in the string to avoid error.
                            string fileParam = value.ToString().Replace("\"", "");
                            iconsInfo.Add(keyName, fileParam);
                        }
                        rkFileIcon.Close();
                    }
                    rkFileType.Close();
                }
                rkRoot.Close();
                return iconsInfo;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public static Icon GetShellIcon(Shell32Icons iconToGet)
        {
            IntPtr[] handlesIconLarge = new IntPtr[1];
            IntPtr[] handlesIconSmall = new IntPtr[1];

            var i = ExtractIconEx(Environment.SystemDirectory + "\\shell32.dll", (int) iconToGet, handlesIconLarge,
                handlesIconSmall, 1);
            return
                Icon.FromHandle(
                    handlesIconLarge[
                        0]); // Icon.FromHandle(resizeIcon(Icon.FromHandle(handlesIconLarge[0]), new Size(32, 32)).GetHicon());
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        static extern uint ExtractIconEx(
            string lpszFile, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);
        [DllImport("shell32.dll", EntryPoint = "ExtractIconA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        static extern IntPtr ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);

        static Bitmap resizeIcon(Icon toResize, Size newSize)
        {
            return resizeImage(toResize.ToBitmap(), newSize);
        }
        static Bitmap resizeImage(IntPtr handle, Size newSize)
        {
            return resizeImage(Image.FromHbitmap(handle), newSize);
        }
        static Bitmap resizeImage(Image imgToResize, Size newSize)
        {
            using (var g = Graphics.FromImage(imgToResize))
            {
                g.Clear(Color.Transparent);
                var b = new Bitmap(newSize.Width, newSize.Height);
                g.DrawImage(b, 0,0, newSize.Width, newSize.Height);

                return b;
            }
        }
        static Bitmap getImageFromBase64(string toGetFrom)
        {
            using (var m = new MemoryStream(Convert.FromBase64String(toGetFrom)))
            {
                using (var g = Graphics.FromImage(Image.FromStream(m)))
                {
                    g.Clear(Color.Transparent);
                    var b = new Bitmap(32, 32);
                    g.DrawImage(b, 0, 0, 32, 32);

                    return b;
                }
            }
        }
    }
}
