using System;
using System.Windows;
using System.Windows.Shell;
using MultiAlign.Data;

namespace MultiAlignRogue.Utils
{
    public class TaskBarProgressSingleton
    {
        private static TaskBarProgressSingleton _singleton = null;

        private bool _disableStepProgress = false;
        private object _controllingObject;

        public TaskBarProgressSingleton()
        {
            if (_singleton == null)
            {
                _singleton = this;
            }
        }

        public static void TakeTaskbarControl(object callingObj)
        {
            if (_singleton._controllingObject == null)
            {
                _singleton._controllingObject = callingObj;
                _singleton._disableStepProgress = true;
            }
        }

        public static void ReleaseTaskbarControl(object callingObj)
        {
            if (ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                _singleton._controllingObject = null;
                _singleton._disableStepProgress = false;
            }
        }

        public static void SetTaskBarProgress(object callingObj, double pct)
        {
            if (!_singleton._disableStepProgress || ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                ThreadSafeDispatcher.Invoke((Action) (() =>
                {
                    Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = pct / 100.0;
                }));
            }
        }

        public static void ShowTaskBarProgress(object callingObj, bool doShow)
        {
            if (!_singleton._disableStepProgress || ReferenceEquals(_singleton._controllingObject, callingObj))
            {
                if (doShow)
                {
                    ThreadSafeDispatcher.Invoke((Action)(() =>
                    {
                        Application.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    }));
                }
                else
                {
                    ThreadSafeDispatcher.Invoke((Action)(() =>
                    {
                        Application.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                    }));

                }
            }
        }


    }
}
