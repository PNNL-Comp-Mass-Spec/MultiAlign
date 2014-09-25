#region

using System;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO.DAO
{
    /// <summary>
    ///     Main application.
    /// </summary>
    public sealed class DataProviderTest
    {

        /// <summary>
        ///     Sets up the NHibernate caches for storing and retrieving data.
        /// </summary>
        /// <returns></returns>
        private static void SetupDataProviders(string path, bool createNew)
        {
            DataAccessFactory.CreateDataAccessProviders(path, createNew);
        }

        /// <summary>
        ///     Creates data providers to the database of the analysis name and path provided.
        /// </summary>
        /// <returns></returns>
        private static void SetupDataProviders(string path)
        {
            SetupDataProviders(path, true);
        }

        [Test]
        [TestCase(@"m:\data\proteomics\matest-gui\guitest.db3")]
        public void TestLoad(string path)
        {            
            SetupDataProviders(path);
            var dao = new MSFeatureDAOHibernate();
            var start = DateTime.Now;
            dao.FindByDatasetId(0);
            var end = DateTime.Now;            
            var span = end.Subtract(start);
            Console.WriteLine("{0} total ms", span.TotalMilliseconds);

            var msFeatureTest = new MSFeatureDOATest();
            start = DateTime.Now;
            msFeatureTest.LoadMSFeaturesFromCache(path);
            end = DateTime.Now;
            span = end.Subtract(start);
            Console.WriteLine("{0} total ms", span.TotalMilliseconds);
        }
    }
}