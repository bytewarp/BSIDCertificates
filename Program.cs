using BSIDCertificates.AS;
using BSIDCertificates.DS21;
using BSIDCertificates.DS22;
using BSIDCertificates.DS24;
using BSIDCertificates.DS25;
using BSIDCertificates.PG;
using BSIDCertificates.PH;
using BSIDCertificates.RS;
using BSIDCertificates.TC;
using BSIDCertificates.TF;
using BSIDCertificates.TR;
using BSIDCertificates.WS29;
using BSIDCertificates.XM;
using BSIDCertificates.XM01;
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
            Application.Run(new PGForm());
        }
    }
}
