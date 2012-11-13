using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manassa
{
    public static class Program
    {        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }
    }
}
