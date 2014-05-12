#region

using System;

#endregion

namespace MultiAlignCore.IO
{
    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(string message, long size, DateTime time)
        {
            Message = message;
            Time = time;
            Size = size;
        }

        public long Size { get; set; }
        public DateTime Time { get; private set; }
        public string Message { get; private set; }
    }
}