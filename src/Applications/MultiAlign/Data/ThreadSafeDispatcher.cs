using System;
using System.Windows;
using System.Windows.Threading;

namespace MultiAlign.Data
{
    public static class ThreadSafeDispatcher
    {
        public static void Invoke(Action action)
        {
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
        }
    }
}