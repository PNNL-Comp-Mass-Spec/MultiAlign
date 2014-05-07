namespace MultiAlignTestSuite.IO
{
    public class UMCReaderFailure
    {
        public void Test()
        {
            var path = @"\\protoapps\UserData\MultiAlignTest\BugFiles\Sarc_P01_F04_0064_18Apr11_Cheetah_11-02-24_LCMSFeatures - Copy.txt";
            var reader = new MultiAlignCore.IO.Features.UmcReader(path);            
        }
    }
}
