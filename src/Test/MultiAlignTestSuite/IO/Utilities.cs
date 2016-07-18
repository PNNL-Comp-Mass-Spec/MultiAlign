using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignTestSuite.IO
{
    class Utilities
    {
        public static void SleepNow(int sleepTimeSeconds)
        {
            var dtStartTime = DateTime.UtcNow;
            while (DateTime.UtcNow.Subtract(dtStartTime).TotalSeconds < sleepTimeSeconds)
            {
                System.Threading.Thread.Sleep(25);
            }
        }
    }
}
