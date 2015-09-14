using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;
using MultiAlignCore.IO.TextFiles;
using NUnit.Framework;

namespace MultiAlignTestSuite.IO
{
    [TestFixture]
    public class PromexFileReading
    {
        [Test]
        public void TestPromexFileReading()
        {
            const int datasetId = 0;
            var reader = new InformedProteomicsReader();
            reader.AddDataFile(@"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_2_8JUL15_Samwise_15-05-04.pbf", datasetId);
            var promexFileReader = new PromexFileReader(reader, datasetId);
            var features =
                promexFileReader.ReadFile(
                    @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_2_8JUL15_Samwise_15-05-04.ms1ft");
            Console.WriteLine(features.Count());
        }

        [Test]
        public void TestPromexClustering()
        {
            var reader = new InformedProteomicsReader();
            reader.AddDataFile(@"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_1_8JUL15_Samwise_15-05-04.pbf", 0);
            reader.AddDataFile(@"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_2_8JUL15_Samwise_15-05-04.pbf", 1);

            var promexFileReader1 = new PromexFileReader(reader, 0);
            var features1 =
                promexFileReader1.ReadFile(
                    @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_1_8JUL15_Samwise_15-05-04.ms1ft");

            var promexFileReader2 = new PromexFileReader(reader, 0);
            var features2 =
                promexFileReader2.ReadFile(
                    @"C:\Users\wilk011\Documents\DataFiles\TopDownTest\CPTAC_Peptidomics_SKOV3_2_8JUL15_Samwise_15-05-04.ms1ft");

            var features = new List<UMCLight>();
            features.AddRange(features1);
            features.AddRange(features2);

            var clusterer = new PromexClusterer(reader);
            var clusters = clusterer.Cluster(features);

            var multiClusters = clusters.Where(c => c.Features.Count > 1).ToList();

            Console.WriteLine(clusters.Count());
        }
    }
}
