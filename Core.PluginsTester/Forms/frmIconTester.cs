using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Utils;

namespace Core.PluginsTester.Forms
{
    public partial class frmIconTester : Form
    {
        public frmIconTester()
        {
            InitializeComponent();
        }

        private Hashtable ht;

        private void frmIconTester_Load(object sender, EventArgs e)
        {
            ht = Icons.GetFileTypeAndIcon();

            var lstIcons = Enum.GetValues(typeof(Shell32Icons)).Cast<Shell32Icons>().ToList();
            foreach (var icon in lstIcons)
            {
                comboBox1.Items.Add(icon.ToString());
            }

            foreach (var htKey in ht.Keys)
            {
                comboBox1.Items.Add(htKey);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var lstIcons = Enum.GetValues(typeof(Shell32Icons)).Cast<Shell32Icons>().ToList();
            var iconSet = false;
            foreach (var icon in lstIcons)
            {
                if (icon.ToString() == comboBox1.Text)
                {
                    pictureBox1.Image = Icons.GetShellIcon(icon).ToBitmap();
                    iconSet = true;
                    break;
                }
            }

            if (!iconSet)
            {
                try
                {
                    pictureBox1.Image = Icons.GetExtensionIcon(ht[comboBox1.Text].ToString()).ToBitmap();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to get icon for extension!", ex);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var m = new Bitmap(pictureBox1.Image);
            var ico = Icon.FromHandle(m.GetHbitmap());
            var x = Icons.AddShortcutOverlay(m);

            pictureBox1.Image = x;
            //pictureBox1.Image = Icons.AddShortcutoverlay(Icon.FromHandle(((Bitmap) pictureBox1.Image).GetHbitmap())).ToBitmap();
        }
    }
}
