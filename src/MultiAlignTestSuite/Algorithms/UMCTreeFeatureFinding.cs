using System;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.RawData;
using NUnit.Framework;
using PNNLOmics.Algorithms.Chromatograms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;
using PNNLOmicsIO.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiAlignTestSuite.Algorithms
{
    [TestFixture]
    public class UmcTreeFeatureFinding
    {
        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv")]
        public IEnumerable<UMCLight> TestUmcFeatures(string path)
        {
            var reader = new MSFeatureLightFileReader { Delimeter = "," };
            var newMsFeatures = reader.ReadFile(path);

            var finder = new UmcTreeFeatureFinder();
            var options = new LCMSFeatureFindingOptions { ConstraintMonoMass = 8, MinUMCLength = 50 };
            var features = finder.FindFeatures(newMsFeatures.ToList(), options, null);

            // Work on total feature count here.
            Assert.Greater(features.Count, 0);

            return features;
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
                  @"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW")]
        public IEnumerable<UMCLight> TestUmcFeatures(string path, string rawPath)
        {
            var reader          = new MSFeatureLightFileReader { Delimeter = "," };
            var newMsFeatures   = reader.ReadFile(path);
            var finder          = new UmcTreeFeatureFinder();
            var options         = new LCMSFeatureFindingOptions { ConstraintMonoMass = 12, MinUMCLength = 50 };

            var features = finder.FindFeatures(newMsFeatures.ToList(), options, null);


            var provider = RawLoaderFactory.CreateFileReader(rawPath);
            provider.AddDataFile(rawPath, 0);            
            
            var chromatogramFinder = new XicCreator();
            chromatogramFinder.CreateXic(features, options.ConstraintMonoMass, provider);

            var dirPath = Path.GetDirectoryName(path);
            using (var writer = File.CreateText(Path.Combine(dirPath, "xics.csv")))
            {
                foreach (var feature in features)
                {
                    writer.WriteLine();
                    writer.WriteLine("Feature {0}", feature.ID);
                    var chargeMap = feature.CreateChargeMap();
                    foreach (int charge in chargeMap.Keys)
                    {
                        writer.WriteLine();
                        foreach (var msFeature in chargeMap[charge])
                        {
                            var count = msFeature.MSnSpectra.Count;
                            writer.WriteLine("{0},{1},{2},{3},{4}", charge, msFeature.Mz, msFeature.Scan, msFeature.Abundance, count);
                        }
                    }
                }
            }

            // Work on total feature count here.
            Assert.Greater(features.Count, 0);

            return features;
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW")]
        public void ReadTime(string path)
        {
            using (var provider = RawLoaderFactory.CreateFileReader(path))
            {
                provider.AddDataFile(path, 0);
                var scanData = provider.GetScanData(0);

                var starTime = System.DateTime.Now;
                foreach (var scan in scanData.Keys)
                {
                    var summary = new ScanSummary();
                    provider.GetRawSpectra(scan, 0, 1, out summary);
                }
                var endTime = System.DateTime.Now;

                Console.WriteLine(endTime.Subtract(starTime).TotalSeconds);
            }
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW",
            ExpectedException = typeof(ScanOutOfRangeException))]
        public void AskForBigScan(string path)
        {
            using (var provider = RawLoaderFactory.CreateFileReader(path))
            {
                provider.AddDataFile(path, 0);
                var summary = new ScanSummary();
                
                provider.GetRawSpectra(10000000, 0, 1, out summary);         
            }
        }

        [Test]
        [TestCase(  @"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
                    @"M:\data\proteomics\TestData\QC-Shew\226151-testDatabase.db3")]
        public void TestDatabaseInsertion(string path, string databasePath)
        {
            var features = TestUmcFeatures(path);

            if (File.Exists(databasePath))            
                File.Delete(databasePath);   
            
            NHibernateUtil.ConnectToDatabase(databasePath, true);
            var cache             = new MSFeatureDAOHibernate();
            var msFeatures = new List<MSFeatureLight>();
            foreach(var feature in features)            
                msFeatures.AddRange(feature.MSFeatures);            
            cache.AddAll(msFeatures);

            var umcCache = new UmcDAOHibernate();
            umcCache.AddAll(features.ToList());
        }
    }
}
