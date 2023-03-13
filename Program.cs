using System;
using System.Drawing;
using System.Windows.Forms;
using customIcons.Forms;

namespace customIcons
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new customIcons_Form1());
        }
    }
}
