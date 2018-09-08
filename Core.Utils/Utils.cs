using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core.Utils
{
    public static class Utils
    {
        public static DialogResult MsgBox(string boxText, string boxTitle,
            MessageBoxButtons boxButtons = MessageBoxButtons.OK, MessageBoxIcon boxIcon = MessageBoxIcon.None)
        {
            return MessageBox.Show(boxText, boxTitle, boxButtons, boxIcon);
        }
    }
}
