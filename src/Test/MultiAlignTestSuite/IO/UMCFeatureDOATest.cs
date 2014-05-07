using System.Collections.Generic;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;
using MultiAlignCore.IO.Features.Hibernate;

using NUnit.Framework;

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
            List<UMCLight> lights = new List<UMCLight>();
            for (int i = 0; i < 100; i++)
            {
                lights.Add(new UMCLight());
                lights[i].ID = i;                
            }

            IUmcDAO cache = new UmcDAOHibernate();
            cache.AddAll(lights);            
        }
    }
}
