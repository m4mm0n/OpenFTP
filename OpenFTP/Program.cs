using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTP
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (frmMain frm = new frmMain())
            {
                frm.ShowDialog();
            }
        }
    }
}
