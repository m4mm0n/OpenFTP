using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentFTP;
using OpenLogger.EventLogger;
using OpenLogger.TraceListener;

namespace Core.PluginsTester.Forms
{
    public partial class frmFtpConnectUploadDownloadTest : Form
    {
        private LoggerTraceListener myLog;
        private FtpClient ftp1;

        public frmFtpConnectUploadDownloadTest()
        {
            InitializeComponent();
        }

        private void frmFtpConnectUploadDownloadTest_Load(object sender, EventArgs e)
        {
            myLog = new LoggerTraceListener();

            explorer1.queueList = queueList1;
            explorer1.IsPcExplorer = false;
            explorer1.OtherExplorer = explorer2;

            explorer2.queueList = queueList1;
            explorer2.IsPcExplorer = true;
            explorer2.RefreshCurrentPcFolder();
            explorer2.OtherExplorer = explorer1;

            queueList1._explorer1 = explorer1;
            queueList1._explorer2 = explorer2;

            //listBox1.SelectionMode = SelectionMode.None;
            FtpTrace.LogFunctions = false;
            FtpTrace.AddListener(myLog);
            myLog.OnTraceEvent += MyLog_OnTraceEvent;
        }

        delegate void TwinVoid(object a, LoggerEventArgs b);
        private void MyLog_OnTraceEvent(object sender, LoggerEventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new TwinVoid(MyLog_OnTraceEvent), sender, e);
            else
            {
                //listViewEx1.Items.Add(e.Log);
                //listViewEx1.SelectedIndices[0] = 
                listBox1.Items.Add(e.Log);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            ftp1 = new FtpClient("ftp.darkmaster.no", 21, "darkmaster.no", "styx2007");

            await ftp1.ConnectAsync();
            explorer1.ftpClient = ftp1;
            //explorer2.OtherExplorer = explorer1;
            await explorer1.RefreshCurrentFtpFolder(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(PrepareTransfers)).Start();
        }

        delegate void SingleVoid();
        async void PrepareTransfers()
        {
            if (this.InvokeRequired)
                this.Invoke(new SingleVoid(PrepareTransfers));
            else
                await queueList1.PrepareTransfers();
        }
    }
}
