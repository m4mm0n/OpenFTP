using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorator
{
    public partial class FrmRenamer : Form
    {
        public string Result { get; set; }
        public string OldName { get; set; }

        public FrmRenamer(string oldName)
        {
            OldName = oldName;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Result = textBox1.Text;
        }

        private void FrmRenamer_Load(object sender, EventArgs e)
        {
            this.lblRenameLabel.Text = lblRenameLabel.Text.Replace("<orginalname>", OldName);
        }
    }
}
