using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignTestSuite.IO
{
    public class UMCReaderFailure
    {
        public void Test()
        {
            string path = @"\\protoapps\UserData\MultiAlignTest\BugFiles\Sarc_P01_F04_0064_18Apr11_Cheetah_11-02-24_LCMSFeatures - Copy.txt";
            MultiAlignCore.IO.Features.UmcReader reader = new MultiAlignCore.IO.Features.UmcReader(path);            
        }
    }
}
