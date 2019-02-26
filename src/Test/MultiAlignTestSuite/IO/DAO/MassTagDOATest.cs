#region

using System.Collections.Generic;
using FeatureAlignment.Data.MassTags;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Hibernate;
using MultiAlignCore.IO.MassTags;
using NUnit.Framework;

#endregion

namespace MultiAlignTestSuite.IO.DAO
{
    [TestFixture]
    public sealed class MassTagDoaTest
    {
        [Test]
        public void SaveMassTags()
        {
            var lights = new List<MassTagLight>();
            for (var i = 0; i < 100; i++)
            {
                lights.Add(new MassTagLight());
                lights[i].Id = i;
            }

            IMassTagDAO cache = new MassTagDAOHibernate();
            cache.AddAll(lights);
        }
    }
}