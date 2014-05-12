#region

using System.Collections.Generic;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;
using NUnit.Framework;
using PNNLOmics.Data.Features;

#endregion

namespace MultiAlignTestSuite
{
    [TestFixture]
    public class UMCFeatureDOATest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            NHibernateUtil.CreateDatabase("test.db3");
        }

        [Test]
        public void SaveUMCFeatures()
        {
            var lights = new List<UMCLight>();
            for (var i = 0; i < 100; i++)
            {
                lights.Add(new UMCLight());
                lights[i].Id = i;
            }

            IUmcDAO cache = new UmcDAOHibernate();
            cache.AddAll(lights);
        }
    }
}