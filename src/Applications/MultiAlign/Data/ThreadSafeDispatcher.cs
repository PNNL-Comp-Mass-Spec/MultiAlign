using System;
using System.Windows;
using System.Windows.Threading;

namespace MultiAlign.Data
{
    public static class ThreadSafeDispatcher
    {
        public static void Invoke(Action action)
        {
#if !DEBUG
            try
            {
#endif
                if (Application.Current == null)
                {
                    return;
                }

                var dispatchObject = Application.Current.Dispatcher;
                if (dispatchObject == null || dispatchObject.CheckAccess())
                {
                    action();
                }
                else
                {
                    dispatchObject.Invoke(action);
                }
#if !DEBUG
            }
            catch (Exception)
            {
                // suppress errors; generally only happens when closing during a task, when the UI thread becomes invalid.
            }
#endif
        }
    }
}