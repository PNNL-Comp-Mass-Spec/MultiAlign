#region

using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.IO.Features.Hibernate;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.Algorithms.Alignment.LCMSWarp
{
    [TestFixture]
    public class LcmsWarpAlignmentTests
    {
        [Test]
        [TestCase(@"M:\data\proteomics\TestData\AlignmentDatabases\test011.db3")]
        public void TestAlignment(string databasePath)
        {
            NHibernateUtil.ConnectToDatabase(databasePath, false);
            var featureCache = new UmcDAOHibernate();

            var features0 = featureCache.FindByDatasetId(0);
            var features1 = featureCache.FindByDatasetId(1);

            var aligner = new LcmsWarpFeatureAligner();

            foreach (var feature in features1)
            {
                feature.MassMonoisotopicAligned = feature.MassMonoisotopic;
                feature.NetAligned = feature.Net;
            }

            aligner.Options = new AlignmentOptions();
            var data = aligner.Align(features0, features1);
        }
    }
}