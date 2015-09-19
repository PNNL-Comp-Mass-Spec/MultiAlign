using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace MultiAlignRogue.Utils
{
    public static class TaskBarProgressSingleton
    {
        public static double TaskBarProgress
        {
            get { return Application.Current.MainWindow.TaskbarItemInfo.ProgressValue; }
            set
            {
                Application.Current.Dispatcher.Invoke((Action)(() => { Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = value; }));
            }
        }

        public static bool ShowTaskBarProgress
        {
            get
            {
                if (Application.Current.MainWindow.TaskbarItemInfo.ProgressState == TaskbarItemProgressState.Normal)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        Application.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    }));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => { Application.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None; }));
                }
            }
        }
    }
}
