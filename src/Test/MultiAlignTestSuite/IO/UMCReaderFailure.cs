#region

using MultiAlignCore.IO.Features;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO
{
    public class UMCReaderFailure: TestBase
    {
        [Test]
        public void Test()
        {
            const string relativePath = @"Data\BUG_FILES\AbudanceBug-691\Sarc_P01_F04_0064_18Apr11_Cheetah_11-02-24_LCMSFeatures - Copy.txt";

            var path = GetPath(relativePath);

            var reader = new UmcReader(path);

            var reader2 = new LCMSFeatureFileReader(path);
        }
    }
}