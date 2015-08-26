#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using MultiAlignCore.IO.RawData;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.Algorithms
{
    [TestFixture]
    public class UmcTreeFeatureFinding
    {
        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv")]
        public IEnumerable<UMCLight> TestUmcFeatures(string path)
        {
            var reader = new MsFeatureLightFileReader {Delimeter = ","};
            var newMsFeatures = reader.ReadFile(path);

            var finder = new UmcTreeFeatureFinder
            {
                MaximumNet = .005,
                MaximumScan = 50
            };
            var tolerances = new FeatureTolerances
            {
                Mass = 8,
                Net = .005
            };
            var options = new LcmsFeatureFindingOptions(tolerances);
            var features = finder.FindFeatures(newMsFeatures.ToList(), options, null);

            // Work on total feature count here.
            Assert.Greater(features.Count, 0);

            return features;
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW",
            Ignore = false)]
        [TestCase(
            @"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW",
            Ignore = true)]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05_isos.csv",
            @"M:\data\proteomics\TestData\QC-Shew\226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05.RAW",
            Ignore = true)]
        public void TestUmcFeatures(string path, string rawPath)
        {
            var reader = new MsFeatureLightFileReader {Delimeter = ","};
            var newMsFeatures = reader.ReadFile(path);
            var finder = new UmcTreeFeatureFinder();
            var featureTolerances = new FeatureTolerances
            {
                Mass = 12,
                Net = .04
            };
            var options = new LcmsFeatureFindingOptions(featureTolerances)
            {
                MaximumNetRange = .003,
                MaximumScanRange = 50
            };


            var provider = RawLoaderFactory.CreateFileReader(rawPath);
            provider.AddDataFile(rawPath, 0);

            var start = DateTime.Now;
            IEnumerable<UMCLight> features = finder.FindFeatures(newMsFeatures.ToList(), options, provider);
            var end = DateTime.Now;
            Console.WriteLine(@"Test Took: " + end.Subtract(start).TotalSeconds);


            if (features == null)
                throw new NullReferenceException("The feature list came back empty.  This is a problem.");


            var dirPath = Path.GetDirectoryName(path);
            if (dirPath != null)
                using (
                    var writer =
                        File.CreateText(Path.Combine(dirPath, Path.GetFileName(path).Replace("_isos.csv", "_xics.csv")))
                    )
                {
                    foreach (var feature in features)
                    {
                        writer.WriteLine();
                        writer.WriteLine("Feature {0}", feature.Id);
                        var chargeMap = feature.CreateChargeMap();
                        foreach (var charge in chargeMap.Keys)
                        {
                            writer.WriteLine();
                            foreach (var msFeature in chargeMap[charge])
                            {
                                var count = msFeature.MSnSpectra.Count;
                                writer.WriteLine("{0},{1},{2},{3},{4}", charge, msFeature.Mz, msFeature.Scan,
                                    msFeature.Abundance, count);
                            }
                        }
                    }
                }

            // Work on total feature count here.
            Assert.Greater(features.Count(), 0);
        }


        [Test]
        [TestCase(@"M:\data\proteomics\Applications\lewy-small\Lewy2_18Cs_2Nov13_Samwise_13-07-28_isos.csv",
            @"M:\data\proteomics\Applications\lewy-small\Lewy2_18Cs_2Nov13_Samwise_13-07-28.raw",
            500,
            Ignore = false)]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW",
            500,
            Ignore = true)]
        [TestCase(
            @"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW",
            500,
            Ignore = true)]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05_isos.csv",
            @"M:\data\proteomics\TestData\QC-Shew\226159_QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05.RAW",
            500,
            Ignore = true)]
        public void TestUmcFeaturesMultipleCharges(string path, string rawPath, int maxScanDiff)
        {
            var reader = new MsFeatureLightFileReader {Delimeter = ","};
            var newMsFeatures = reader.ReadFile(path);
            var finder = new UmcTreeFeatureFinder();
            var featureTolerances = new FeatureTolerances
            {
                Mass = 12,
                Net = .05
            };
            var options = new LcmsFeatureFindingOptions(featureTolerances)
            {
                MaximumNetRange = .002,
                MaximumScanRange = 50
            };

            var provider = RawLoaderFactory.CreateFileReader(rawPath);
            provider.AddDataFile(rawPath, 0);

            var start = DateTime.Now;
            IEnumerable<UMCLight> features = finder.FindFeatures(newMsFeatures.ToList(), options, provider);
            var end = DateTime.Now;
            Console.WriteLine(@"Test Took: " + end.Subtract(start).TotalSeconds);


            if (features == null)
                throw new NullReferenceException("The feature list came back empty.  This is a problem.");


            var dirPath = Path.GetDirectoryName(path);
            if (dirPath != null)
                using (
                    var writer =
                        File.CreateText(Path.Combine(dirPath, Path.GetFileName(path).Replace("_isos.csv", "_xics.csv")))
                    )
                {
                    foreach (var feature in features)
                    {
                        writer.WriteLine();
                        writer.WriteLine("Feature {0}", feature.Id);
                        var chargeMap = feature.CreateChargeMap();

                        if (chargeMap.Keys.Count < 2)
                            continue;

                        foreach (var charge in chargeMap.Keys)
                        {
                            writer.WriteLine();
                            foreach (var msFeature in chargeMap[charge])
                            {
                                var count = msFeature.MSnSpectra.Count;
                                writer.WriteLine("{0},{1},{2},{3},{4}", charge, msFeature.Mz, msFeature.Scan,
                                    msFeature.Abundance, count);
                            }
                        }


                        var charges = chargeMap.Keys.ToList();

                        for (var i = 0; i < charges.Count; i++)
                        {
                            for (var j = i; j < charges.Count; j++)
                            {
                                var x = chargeMap[charges[i]];
                                var y = chargeMap[charges[j]];

                                var diff = x.MinScan() - y.MinScan();
                                if (diff > maxScanDiff)
                                {
                                    throw new Exception(
                                        "There is a problem with the feature finder across charge states");
                                }
                            }
                        }
                    }
                }


            // Work on total feature count here.
            Assert.Greater(features.Count(), 0);
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\smallFeature.csv",
            Ignore = false)]
        public void TestChargeStateSplit(string path)
        {
            var data = File.ReadAllLines(path);
            var map = new Dictionary<int, List<MSFeatureLight>>();

            for (var i = 1; i < data.Length; i++)
            {
                var feature = new MSFeatureLight();
                var msFeatureData = data[i].Split(',');

                feature.ChargeState = Convert.ToInt32(msFeatureData[0]);
                feature.MassMonoisotopic = Convert.ToDouble(msFeatureData[1]);
                feature.Scan = Convert.ToInt32(msFeatureData[2]);
                feature.Abundance = Convert.ToInt64(msFeatureData[3]);

                if (!map.ContainsKey(feature.ChargeState))
                {
                    map.Add(feature.ChargeState, new List<MSFeatureLight>());
                }
                map[feature.ChargeState].Add(feature);
            }


            var features = new List<UMCLight>();

            foreach (var charge in map.Keys)
            {
                var feature = new UMCLight();
                foreach (var msFeature in map[charge])
                {
                    feature.AddChildFeature(msFeature);
                }
                feature.CalculateStatistics(ClusterCentroidRepresentation.Median);
                features.Add(feature);
            }

            var finder = new MsFeatureTreeClusterer<MSFeatureLight, UMCLight>();
            var comparison = finder.CompareMonoisotopic(features[0], features[1]);

            Assert.AreNotEqual(comparison, 0);
        }


        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW"
            )]
        public void ReadTime(string path)
        {
            using (var provider = RawLoaderFactory.CreateFileReader(path))
            {
                provider.AddDataFile(path, 0);
                var scanData = provider.GetScanData(0);

                var starTime = DateTime.Now;
                foreach (var scan in scanData.Keys)
                {
                    ScanSummary summary;
                    provider.GetRawSpectra(scan, 0, 1, out summary);
                }
                var endTime = DateTime.Now;

                Console.WriteLine(endTime.Subtract(starTime).TotalSeconds);
            }
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\smallTest\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW",
            ExpectedException = typeof (ScanOutOfRangeException))]
        public void AskForBigScan(string path)
        {
            using (var provider = RawLoaderFactory.CreateFileReader(path))
            {
                provider.AddDataFile(path, 0);
                ScanSummary summary;
                provider.GetRawSpectra(10000000, 0, 1, out summary);
            }
        }

        [Test]
        [TestCase(@"M:\data\proteomics\TestData\QC-Shew\226151_QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"M:\data\proteomics\TestData\QC-Shew\226151-testDatabase.db3")]
        public void TestDatabaseInsertion(string path, string databasePath)
        {
            var features = TestUmcFeatures(path);

            if (File.Exists(databasePath))
                File.Delete(databasePath);

            NHibernateUtil.ConnectToDatabase(databasePath, true);
            var cache = new MSFeatureDAOHibernate();
            var msFeatures = new List<MSFeatureLight>();
            foreach (var feature in features)
                msFeatures.AddRange(feature.MsFeatures);
            cache.AddAll(msFeatures);

            var umcCache = new UmcDAOHibernate();
            umcCache.AddAll(features.ToList());
        }
    }
}