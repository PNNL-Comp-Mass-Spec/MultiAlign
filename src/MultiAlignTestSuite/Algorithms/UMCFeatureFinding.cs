using System.Linq;
using Mage;
using MultiAlignCore.Algorithms.FeatureFinding;
using NUnit.Framework;
using PNNLOmicsIO.IO;
using System.IO;

namespace MultiAlignTestSuite.Algorithms
{
    public class UmcFeatureFinding
    {
        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv")]
        public void TestUmcFeatures(string path)
        {         
            var reader          = new MSFeatureLightFileReader { Delimeter = "," };
            var newMsFeatures   = reader.ReadFile(path);
            var finder          = new UMCFeatureFinder();
            var options         = new LCMSFeatureFindingOptions
            {
                AveMassWeight = .01f,
                ConstraintAveMass = 6,
                ConstraintMonoMass = 6,
                FitWeight = .1f,
                IsIsotopicFitFilterInverted = false,
                IsotopicFitFilter = .15,
                IsotopicIntensityFilter = 0,
                LogAbundanceWeight = .1f,
                MaxDistance = .1,
                MinUMCLength = 3,
                MonoMassWeight = .01f,
                NETWeight = .1f,
                ScanWeight = .01f,
                UMCAbundanceReportingType = AbundanceReportingType.Max,
                UseIsotopicFitFilter = true,
                UseIsotopicIntensityFilter = false,
                UseNET = true
            };

            var umcs = finder.FindFeatures(newMsFeatures.ToList(), options, null);
            Assert.Greater(umcs.Count, 0);
        }
    }
}
