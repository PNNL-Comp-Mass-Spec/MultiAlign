using System.Collections.Generic;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignTestSuite
{
    public class MassTagDOATest
    {

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
