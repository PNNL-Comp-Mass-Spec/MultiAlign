using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignTestSuite.IO
{
    [TestFixture]
    class UMCClusterIO
    {
        protected string m_basePath;
        
        [SetUp]
        public void TestSetup()
        {
            m_basePath = @"M:\";
        }

        [Test]
        [TestCase(@"clusterTest.db3")]
        public void TestAddClusters(string name)
        {
            string databasePath = Path.Combine(m_basePath, name);

            // This is a factory based method that creates a set of data access providers used throughout MultiAlign
            FeatureDataAccessProviders providers = DataAccessFactory.CreateDataAccessProviders(databasePath, true);


            List<UMCClusterLight> clusters = new List<UMCClusterLight>();
            for (int i = 0; i < 10; i++)
            {
                UMCClusterLight cluster = new UMCClusterLight();
                cluster.ID      = i;
                cluster.GroupID = 0;
            }
            providers.ClusterCache.AddAll(clusters);
        }
    }
}
