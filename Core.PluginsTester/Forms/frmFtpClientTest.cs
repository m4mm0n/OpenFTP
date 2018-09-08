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
using Core.Utils;
using Core.Utils.FTPUtils;
using FluentFTP;
using OpenLogger.TraceListener;

//using FTPClient;
//using FTPClient.Enumerations;

namespace Core.PluginsTester.Forms
{
    public partial class frmFtpClientTest : Form
    {
        //private FTP ftp;
        private FtpClient ftp;
        private LoggerTraceListener logger;

        public frmFtpClientTest()
        {
            InitializeComponent();
        }

        private void frmFtpClientTest_Load(object sender, EventArgs e)
        {
            Setup.SetCustomExceptionHandler();
            
            logger = new LoggerTraceListener();
            logger.OnTraceEvent += Logger_OnTraceEvent;
            FtpTrace.AddListener(logger);
            FtpTrace.LogFunctions = false;
        }

        delegate void Logger_OnTraceEventDelegate(object a, OpenLogger.EventLogger.LoggerEventArgs b);
        private void Logger_OnTraceEvent(object sender, OpenLogger.EventLogger.LoggerEventArgs e)
        {
            if (textBox1.InvokeRequired)
                textBox1.Invoke(new Logger_OnTraceEventDelegate(Logger_OnTraceEvent), sender, e);
            else
            {
                textBox1.Text += e.Log + Environment.NewLine;
            }
        }

        private TaskFactory tasks = new TaskFactory();

        private async void button1_Click(object sender, EventArgs e)
        {
            tasks.StartNew(ftp.ConnectAsync);
            //var tsk = new TaskFactory().StartNew(ftp.ConnectAsync);
            //await ftp.ConnectAsync();
            //ftp.SetUserInfo("!styx", "geir2k2");
            //ftp.SetConnectionInfo("dos.dark.sx", "55000", SSLType.AuthTLSv12, 2048, true, true, 120);

            //var th = new Thread(new ThreadStart(ConnectThread));
            //th.Start();
            //await ftp.ConnectAsync();
            //var z = ftp.sendCommand("STAT");
            //Console.WriteLine(z.Response);
            //Console.WriteLine(z.Code);
            //Console.WriteLine(z.Message);
            //ftp.LogIn();

        }

        object m_lock = new object();
        void ConnectThread()
        {
            //await ftp.ConnectAsync();
            //lock (m_lock)
            //{
            //    ftp.ConnectAsync();
            //    ftp.LogIn();
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tasks.StartNew(ftp.DisconnectAsync);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //throw new NotSupportedException("This shit aint supported bitch!", new AccessViolationException("You fat fuck!"));
            //ftp.SetDetails("ftp.darkmaster.no", "21", "darkmaster.no", "styx2007", SSLType.Unsecure, true);
            ftp = new FtpClient("ftp.darkmaster.no", 21, "darkmaster.no", "styx2007");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tasks.StartNew(GetFileList);
        }


        delegate void AddItemDelegate(string a, params object[] b);

        void AddItem(string fileInfo, params object[] args)
        {
            if (textBox1.InvokeRequired)
                textBox1.Invoke(new AddItemDelegate(AddItem), fileInfo, args);
            else
            {
                textBox1.Text += string.Format(fileInfo, args) + Environment.NewLine;
            }
        }
        private async Task GetFileList()
        {
            foreach (var item in await ftp.GetListingAsync())
            {
                AddItem("Full Name: {0}, (Short) Name: {1}", item.FullName, item.Name);
            }
        }
    }
}
