using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public static class Setup
    {
        public static void SetCustomExceptionHandler()
        {
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        }

        public static void SetInternationalLanguage()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            using (var f = new frmException(e.Exception))
            {
                f.ShowDialog();
            }
        }
    }
}
