#region

using System;

#endregion

namespace MultiAlignCore.IO.RawData
{
    public class ScanOutOfRangeException : Exception
    {
        public ScanOutOfRangeException(string message)
            : base(message)
        {
        }
    }
}