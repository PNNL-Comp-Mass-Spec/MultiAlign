#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeatureAlignment.Data;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.RawData;
using MultiAlignCore.IO.TextFiles;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.Algorithms
{
    [TestFixture]
    public class UmcTreeFeatureFinding: TestBase
    {
        [Test]
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv", 77034)]
        public IEnumerable<UMCLight> TestUmcFeatures(string relativePath, int expectedFeatureCount)
        {
            // Get the absolute path
            var path = GetPath(relativePath);

            var reader = new MsFeatureLightFileReader { Delimiter = ',' };
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

            IScanSummaryProvider provider = null;
            var rawFilePath = path.Replace("_isos.csv", ".raw");
            UpdateStatus("Using raw data to create better features.");

            var providerCache = new ScanSummaryProviderCache();
            provider = providerCache.GetScanSummaryProvider(rawFilePath, 1);

            var features = finder.FindFeatures(newMsFeatures.ToList(), options, provider);

            // Work on total feature count here.
            Assert.Greater(features.Count, 0);

            Assert.AreEqual(expectedFeatureCount, features.Count);

            return features;
        }

        [Test]
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW")]
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05_isos.csv",
            @"Data\QC_SHEW\QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05.RAW")]
        public void TestUmcFeatures(string relativePath, string relativeRawPath)
        {
            // Get absolute paths
            var path = GetPath(relativePath);
            var rawPath = GetPath(relativeRawPath);

            var reader = new MsFeatureLightFileReader { Delimiter = ',' };
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


            var provider = RawLoaderFactory.CreateFileReader(rawPath, 0);

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
        [TestCase(@"Data\Lewy\Lewy2_18Cs_2Nov13_Samwise_13-07-28_isos.csv",
            @"Data\Lewy\Lewy2_18Cs_2Nov13_Samwise_13-07-28.raw",
            500)]
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW",
            500)]
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05_isos.csv",
            @"Data\QC_SHEW\QC_Shew_11_02_pt5-d_6Jun11_Sphinx_11-04-05.RAW",
            500)]
        public void TestUmcFeaturesMultipleCharges(string relativePath, string relativeRawPath, int maxScanDiff)
        {
            // Get absolute paths
            var path = GetPath(relativePath);
            var rawPath = GetPath(relativeRawPath);

            var reader = new MsFeatureLightFileReader { Delimiter = ',' };
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

            var provider = RawLoaderFactory.CreateFileReader(rawPath, 0);

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
            Ignore = "Local test file")]
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
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW"
            )]
        public void ReadTime(string relativePath)
        {
            // Get the absolute path
            var path = GetPath(relativePath);

            using (var provider = RawLoaderFactory.CreateFileReader(path, 0))
            {

                var starTime = DateTime.Now;
                var scanData = provider.GetScanSummaries();
                var endTime = DateTime.Now;

                Console.WriteLine(endTime.Subtract(starTime).TotalSeconds);
            }
        }

        [Test]
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27.RAW")]
        public void AskForBigScan(string relativePath)
        {
            // Get the absolute path
            var path = GetPath(relativePath);

            Assert.Throws<ScanOutOfRangeException>(() => GetOutOfRangeScan(path));
        }

        private void GetOutOfRangeScan(string path)
        {
            using (var provider = RawLoaderFactory.CreateFileReader(path, 0))
            {
                var summary = provider.GetScanSummary(10000000);
            }
        }

        [Test]
        [TestCase(@"Data\QC_SHEW\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_isos.csv",
            @"testResults\QC_Shew_11_02_pt5-b_6Jun11_Sphinx_11-03-27_TestDatabaseInsertion.db3", 77034)]
        public void TestDatabaseInsertion(string relativePath, string databasePathRelative, int expectedFeatureCount)
        {
            // Get the absolute path
            var path = GetPath(relativePath);
            var databasePath = GetPath(databasePathRelative);

            var features = TestUmcFeatures(path, expectedFeatureCount);

            if (File.Exists(databasePath))
                File.Delete(databasePath);

            try
            {
                NHibernateUtil.ConnectToDatabase(databasePath, true);
                var cache = new MSFeatureDAOHibernate();
                var msFeatures = new List<MSFeatureLight>();
                foreach (var feature in features)
                    msFeatures.AddRange(feature.MsFeatures);
                cache.AddAll(msFeatures);

                var umcCache = new UmcDAOHibernate();
                umcCache.AddAll(features.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caching features: " + ex.Message);
                Console.WriteLine(ex.StackTrace);

                throw;
            }

        }
    }
}