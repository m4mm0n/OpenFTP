using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core.PluginsTester.Forms
{
    public partial class frmExplorerTest : Form
    {
        public frmExplorerTest()
        {
            InitializeComponent();
        }

        private void frmExplorerTest_Load(object sender, EventArgs e)
        {
            explorer1.RefreshCurrentPcFolder();
        }
    }
}
