using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core.Utils
{
    public partial class frmException : Form
    {
        private Exception exception;
        public frmException(Exception ex, bool isInnerEx = false)
        {
            exception = ex;
            InitializeComponent();

            this.Text = ex.GetType().Name;

            if (isInnerEx)
            {
                button1.Enabled = false;
                button3.Text = "Close";
            }
        }

        private void frmException_Load(object sender, EventArgs e)
        {
            this.Icon = Icons.GetShellIcon(Shell32Icons.EXCLAMATION);

            if (exception.InnerException != null)
                button3.Enabled = true;
            else
                button3.Enabled = false;

            var sb = new StringBuilder();

            SetTerminalFont(textBox1.Handle);

            sb.AppendLine(string.Format("{0}{1}", formatString("HResult"), exception.HResult));
            sb.AppendLine(string.Format("{0}{1}", formatString("Help Link"), exception.HelpLink));
            sb.AppendLine(string.Format("{0}{1}", formatString("Message"), exception.Message));
            sb.AppendLine(string.Format("{0}{1}", formatString("Source"), exception.Source));
            sb.AppendLine(string.Format("{0}{1}", formatString("Stack Trace"), exception.StackTrace));
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(exception.ToString());

            textBox1.Text = sb.ToString();
        }

        string formatString(string format, int maxLen = 16/*, params object[] args*/)
        {
            var frmt = format;
            while (frmt.Length < maxLen)
            {
                frmt += " ";
            }

            frmt += ": ";
//            return string.Format(frmt, args);
            return frmt;
        }

        static void SetTerminalFont(IntPtr handle)
        {
            var hFnt = GetStockObject(10);
            SendMessage(handle.ToInt32(), 0x30, hFnt, true);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        static extern int GetStockObject(int nIndex);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        static extern int SendMessage(int hWnd, int wMsg, int wParam, bool lParam);

        private void button3_Click(object sender, EventArgs e)
        {
            using (var fr = new frmException(exception.InnerException, true))
                fr.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
