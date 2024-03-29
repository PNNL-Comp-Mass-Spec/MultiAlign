﻿#region

using System.Collections.Generic;
using FeatureAlignment.Data.Features;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO
{
    [TestFixture]
    public class UMCFeatureDOATest
    {
        [OneTimeSetUp]
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