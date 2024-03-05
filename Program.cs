using BSIDCertificates.PG;
using BSIDCertificates.TF;
using BSIDCertificates.TR;
using BSIDCertificates.XM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSIDCertificates
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TFForm());
        }
    }
}
