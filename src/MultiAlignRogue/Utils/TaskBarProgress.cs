using System;
using System.Windows;
using System.Windows.Shell;
using MultiAlign.Data;

namespace MultiAlignRogue.Utils
{
    public class TaskBarProgress
    {
        private static readonly TaskBarProgress Instance = new TaskBarProgress();

        /// <summary>
        /// Automatically initialize the instance.
        /// </summary>
        static TaskBarProgress()
        {
            if (Instance == null)
            {
                Instance = new TaskBarProgress();
            }
        }

        public static TaskBarProgress GetInstance()
        {
            return Instance;
        }

        private bool disableStepProgress = false;
        private object controllingObject;

        /// <summary>
        /// Prevent instantiation outside of the static constructor
        /// </summary>
        private TaskBarProgress()
        {
        }

        public void TakeControl(object callingObj)
        {
            if (this.controllingObject == null)
            {
                this.controllingObject = callingObj;
                this.disableStepProgress = true;
            }
        }

        public void ReleaseControl(object callingObj)
        {
            if (ReferenceEquals(this.controllingObject, callingObj))
            {
                this.controllingObject = null;
                this.disableStepProgress = false;
            }
        }

        public void SetProgress(object callingObj, double pct)
        {
            if (!this.disableStepProgress || ReferenceEquals(this.controllingObject, callingObj))
            {
                ThreadSafeDispatcher.Invoke((Action)(() =>
                {
                    Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = pct / 100.0;
                }));
            }
        }

        public void ShowProgress(object callingObj, bool doShow)
        {
            if (!this.disableStepProgress || ReferenceEquals(this.controllingObject, callingObj))
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
