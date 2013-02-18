using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MultiAlignCore.Algorithms.FeatureFinding;
using PNNLOmics.Data;

namespace MultiAlignTestSuite.Algorithms.Xic
{
    [TestFixture]
    public class XicTest
    {
        [Test]
        [TestCase(  @"M:\data\proteomics\IQ-Api-Testing\226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05.RAW",
                    @"M:\data\proteomics\IQ-Api-Testing\226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05_peaks.txt",            
                    8794,
                    697.44746)]
        public void TestXicAdaptor(string rawPath, string peaksPath, int scan, double mz)
        {
            Console.WriteLine("Testing Xic Adaptor");

            // This object uses the data files to find Xic's
            XicAdaptor adaptor          = new XicAdaptor(rawPath, peaksPath);            
            List<XYData> xic            = adaptor.FindXic(mz,      scan);

            // This one finds the target.
            XicFinder finder            = new XicFinder();
            List<XYData> trueFeature    = finder.FindTarget(xic, scan);

            Console.WriteLine("Detected feature at {0} and scan {1}", mz, scan);
            foreach (XYData data in xic)
            {
                Console.WriteLine("{0}\t{1}", data.X, data.Y);
            }
        }
    }
}
