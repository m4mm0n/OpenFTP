using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Utils;
using FluentFTP;
using OpenLogger.TraceListener;

namespace Core.PluginsTester.Forms
{
    public partial class frmFtpExplorerTest : Form
    {
        private FluentFTP.FtpClient ftp;
        private LoggerTraceListener log;

        public frmFtpExplorerTest()
        {
            InitializeComponent();

            //Setup.SetCustomExceptionHandler();
            Setup.SetInternationalLanguage();
            log = new LoggerTraceListener();
            log.OnTraceEvent += Log_OnTraceEvent;
        }

        delegate void LogDelegate(object a, OpenLogger.EventLogger.LoggerEventArgs b);
        private void Log_OnTraceEvent(object sender, OpenLogger.EventLogger.LoggerEventArgs e)
        {
            if (listBox1.InvokeRequired)
                listBox1.Invoke(new LogDelegate(Log_OnTraceEvent), sender, e);
            else
            {
                listBox1.Items.Add(e.Log);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            FtpTrace.AddListener(log);
            ftp = new FtpClient("ftp.darkmaster.no", 21, "darkmaster.no", "styx2007");
            await ftp.ConnectAsync();

            explorer1.ftpClient = ftp;
            explorer1.IsPcExplorer = false;
            await explorer1.RefreshCurrentFtpFolder(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ex = new UnauthorizedAccessException("Fuck this!");
            throw ex;
        }
    }
}
