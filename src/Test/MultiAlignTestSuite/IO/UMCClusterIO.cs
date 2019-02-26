#region

using System.Collections.Generic;
using System.IO;
using FeatureAlignment.Data.Features;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO
{
    [TestFixture]
    internal class UMCClusterIO
    {
        protected string m_basePath;

        [SetUp]
        public void TestSetup()
        {
            m_basePath = @"M:\";
        }

        [Test]
        [TestCase(@"clusterTest.db3", Ignore = "Local test file")]
        public void TestAddClusters(string name)
        {
            var databasePath = Path.Combine(m_basePath, name);

            // This is a factory based method that creates a set of data access providers used throughout MultiAlign
            var providers = DataAccessFactory.CreateDataAccessProviders(databasePath, true);


            var clusters = new List<UMCClusterLight>();
            for (var i = 0; i < 10; i++)
            {
                var cluster = new UMCClusterLight();
                cluster.Id = i;
                cluster.GroupId = 0;
            }
            providers.ClusterCache.AddAll(clusters);
        }
    }
}