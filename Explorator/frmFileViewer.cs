using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenLogger;
using OpenLogger.Enumerations;

namespace Explorator
{
    public partial class frmFileViewer : Form
    {
        private PCObjectItem itemToView;
        private Logger Log = new Logger(LoggerType.Console, "Core.Explorator.frmFileViewer", true);

        public frmFileViewer(object objectToView)
        {
            InitializeComponent();
            if (objectToView is PCObjectItem)
            {
                Log.Debug("Object to view is a PCObjectItem...");
                itemToView = (PCObjectItem) objectToView;
                if (itemToView.IsDirectory)
                {
                    Log.Debug("Closing Viewer - Object is a Directory!");
                    this.Close();
                }
                else if (itemToView.ClosedFileInfo == null)
                {
                    Log.Debug("Closing Viewer - Object is neither Directory or File!");
                    this.Close();
                }
            }
            else
            {
                Log.Debug("Object is not a PCObjectItem...");
                this.Close();
            }
        }

        private void frmFileViewer_Load(object sender, EventArgs e)
        {
            var preIcon =
                "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";
            using (var m = new MemoryStream(Convert.FromBase64String(preIcon)))
            {
                this.Icon = Icon.FromHandle(new Bitmap(m).GetHicon());
            }
            Utils.SetTerminalFont(txtDetails.Handle);

            this.Text = "Viewing " + itemToView.ClosedFileInfo.Name;

            switch (itemToView.ClosedFileInfo.Extension.ToLower())
            {
                case ".sfv":
                case ".txt":
                case ".nfo":
                case ".diz":
                    txtDetails.Text = File.ReadAllText(itemToView.ClosedFileInfo.FullName);
                    break;
                default:
                    if (MessageBox.Show("You want to view this in HexView?", "Default Extension Not Found",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                    }
                    else
                    {
                        txtDetails.Text = File.ReadAllText(itemToView.ClosedFileInfo.FullName);
                    }
                    break;
            }
        }
    }
}
