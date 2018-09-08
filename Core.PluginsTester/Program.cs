using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.PluginsTester.Forms;

namespace Core.PluginsTester
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //using (var frm1 = new frmFtpClientTest())
            //{
            //    frm1.ShowDialog();
            //}

            //using (var frm2 = new frmExplorerTest())
            //{
            //    frm2.ShowDialog();
            //}

            //using (var frm3 = new frmIconTester())
            //{
            //    frm3.ShowDialog();
            //}

            //using (var frm4 = new frmFtpExplorerTest())
            //{
            //    frm4.ShowDialog();
            //}

            //using (var frm5 = new frmQueueListTest())
            //{
            //    frm5.ShowDialog();
            //}

            using (var frm6 = new frmFtpConnectUploadDownloadTest())
            {
                frm6.ShowDialog();
            }
        }
    }
}
