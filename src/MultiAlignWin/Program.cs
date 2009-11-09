using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using MultiAlignWin.UI;
using MultiAlignWin.Diagnostics;

namespace MultiAlignWin
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the MultiAlign Application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMdiMain());            
        }
    }
}