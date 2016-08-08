#region

using MultiAlignCore.Data;
using MultiAlignCore.IO.Hibernate;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO.DAO
{
    /// <summary>
    ///     Testing for loading a MSn database using hibernate.
    /// </summary>
    [TestFixture]
    public class NHibernateFeatureTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            NHibernateUtil.CreateDatabase("test.db3");
        }

        [Test]
        public void CreateMsnTest()
        {
            var msnFeatures = new MSnFeatureToMSFeatureDAOHibernate();

            var map = new MSFeatureToMSnFeatureMap();
            map.LCMSFeatureID = 0;
            map.MSDatasetID = 0;
            map.MSFeatureID = 0;
            map.MSMSFeatureID = 0;
            map.RawDatasetID = 0;

            msnFeatures.Add(map);
        }
    }
}