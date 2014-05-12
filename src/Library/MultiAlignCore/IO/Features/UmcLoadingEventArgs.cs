#region

using System;

#endregion

namespace MultiAlignCore.IO.Features
{
    public class UmcLoadingEventArgs : EventArgs
    {
        public UmcLoadingEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}