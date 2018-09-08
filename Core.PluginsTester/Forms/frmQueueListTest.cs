using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Explorator;

namespace Core.PluginsTester.Forms
{
    public partial class frmQueueListTest : Form
    {
        public frmQueueListTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var x = explorer1.SelectedItem;
            //queueList1.AddQueueItem((PCObjectItem)x, "C:\\Temp\\");
            queueList1.AddItemToQueue(x, "C:\\temp\\");
        }

        private void frmQueueListTest_Load(object sender, EventArgs e)
        {
            explorer1.RefreshCurrentPcFolder();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //queueList1.ChangeProgress(0, ((PCObjectItem)explorer1.SelectedItem).ClosedFileInfo.Length / 2);
            //queueList1.ChangeProgress(0, 24, true);
            queueList1.ChangeProgress(queueList1.GetSelected, 34, true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //queueList1.ChangeProgress(1, ((PCObjectItem)explorer1.SelectedItem).ClosedFileInfo.Length / 2 + 5);
        }
    }
}
