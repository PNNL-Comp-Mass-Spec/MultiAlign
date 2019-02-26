using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureAlignment.Data.Features;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassSpecData;
using InformedProteomics.FeatureFinding.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.IO.RawData;
using MultiAlignCore.IO.TextFiles;
using NUnit.Framework;

namespace MultiAlignTestSuite.IO
{
    [TestFixture]
    public class PromexFileReading
    {
        // File paths for test data
        const string pbf1 = @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_1_8JUL15_Samwise_15-05-04.pbf";
        const string pbf2 = @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_2_8JUL15_Samwise_15-05-04.pbf";
        const string pbf3 = @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_3_8JUL15_Samwise_15-05-04.pbf";

        const string ms1ft1 = @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_1_8JUL15_Samwise_15-05-04.ms1ft";
        const string ms1ft2 = @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_2_8JUL15_Samwise_15-05-04.ms1ft";
        const string ms1ft3 = @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_3_8JUL15_Samwise_15-05-04.ms1ft";

        [Test]
        public void TestPromexFileReading()
        {
            const int datasetId = 0;
            var reader = new InformedProteomicsReader(datasetId, pbf1);
            var promexFileReader = new PromexFileReader(reader, datasetId);
            var features =
                promexFileReader.ReadFile(ms1ft1);
            Console.WriteLine(features.Count());
        }

        [Test]
        public void TestPromexClustering()
        {
            var provider = new ScanSummaryProviderCache();
            var reader1 = provider.GetScanSummaryProvider(pbf1, 0) as InformedProteomicsReader;
            var reader2 = provider.GetScanSummaryProvider(pbf2, 1) as InformedProteomicsReader;
            var reader3 = provider.GetScanSummaryProvider(pbf3, 2) as InformedProteomicsReader;

            var promexFileReader1 = new PromexFileReader(reader1, 0);
            var features1 =
                promexFileReader1.ReadFile(ms1ft1);

            var promexFileReader2 = new PromexFileReader(reader2, 1);
            var features2 =
                promexFileReader2.ReadFile(ms1ft2);

            var promexFileReader3 = new PromexFileReader(reader3, 2);
            var features3 =
                promexFileReader3.ReadFile(ms1ft3);

            var features = new List<UMCLight>();
            features.AddRange(features1);
            features.AddRange(features2);
            features.AddRange(features3);

            var clusterer = new PromexClusterer
            {
                Readers =  provider,
            };
            var clusters = clusterer.Cluster(features);

            Console.WriteLine(clusters.Count(c => c.Features.Count > 1));
        }

        [Test]
        public void CompareFileReading()
        {
            // Read using MultiAlign to Promex adapters
            var provider = new ScanSummaryProviderCache();
            var reader1 = provider.GetScanSummaryProvider(pbf1, 0) as InformedProteomicsReader;
            var promexFileReader = new PromexFileReader(reader1, 0);
            var features = promexFileReader.ReadFile(ms1ft1).ToList();

            var lcmsRun = PbfLcMsRun.GetLcMsRun(pbf1);
            var promexFeatures = LcMsFeatureAlignment.LoadProMexResult(0, ms1ft1, lcmsRun).ToList();

            Assert.AreEqual(features.Count, promexFeatures.Count);

            for (int i = 0; i < features.Count; i++)
            {
                Assert.AreEqual(features[i].MassMonoisotopic, promexFeatures[i].Mass);
                ////Assert.AreEqual(features[i].Mz, promexFeatures[i].RepresentativeMz);
                Assert.AreEqual(features[i].Net, promexFeatures[i].Net);
                Assert.AreEqual(features[i].ScanStart, promexFeatures[i].MinScanNum);
                Assert.AreEqual(features[i].ScanEnd, promexFeatures[i].MaxScanNum);
                Assert.AreEqual(features[i].Abundance, promexFeatures[i].Abundance);
            }
        }

        [Test]
        public void CompareClustering()
        {
            // Cluster using MultiAlign to Promex adapters
            var provider = new ScanSummaryProviderCache();
            var reader1 = provider.GetScanSummaryProvider(pbf1, 0) as InformedProteomicsReader;
            var reader2 = provider.GetScanSummaryProvider(pbf2, 1) as InformedProteomicsReader;
            var promexFileReader1 = new PromexFileReader(reader1, 0);
            var features1 = promexFileReader1.ReadFile(ms1ft1);

            var promexFileReader2 = new PromexFileReader(reader2, 1);
            var features2 =
                promexFileReader2.ReadFile(ms1ft2);

            var features = new List<UMCLight>();
            features.AddRange(features1);
            features.AddRange(features2);

            var clusterer = new PromexClusterer
            {
                Readers = provider,
            };
            var clusters = clusterer.Cluster(features);
            var clusterCount = clusters.Count(c => c.UmcList.Count > 1);

            // Cluster using only ProMex
            var lcmsRun1 = PbfLcMsRun.GetLcMsRun(pbf1);
            var lcmsRun2 = PbfLcMsRun.GetLcMsRun(pbf2);

            var aligner = new LcMsFeatureAlignment(new LcMsFeatureAlignComparer(new Tolerance(10, ToleranceUnit.Ppm)));
            var promexFeatures1 = LcMsFeatureAlignment.LoadProMexResult(0, ms1ft1, lcmsRun1);
            aligner.AddDataSet(0, promexFeatures1, lcmsRun1);

            var promexFeatures2 = LcMsFeatureAlignment.LoadProMexResult(1, ms1ft2, lcmsRun2);
            aligner.AddDataSet(1, promexFeatures2, lcmsRun2);

            aligner.AlignFeatures();
            var promexClusters = aligner.GetAlignedFeatures();
            var promexClusterCount = promexClusters.Count(c => c.Count(f => f != null) > 1);

            Assert.AreEqual(clusters.Count, promexClusters.Count);
            Assert.AreEqual(clusterCount, promexClusterCount);
        }
    }
}
