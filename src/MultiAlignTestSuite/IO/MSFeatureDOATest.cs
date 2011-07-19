using System.Collections.Generic;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;
using MultiAlignCore.IO.Features.Hibernate;

namespace MultiAlignTestSuite
{
    public class MSFeatureDOATest
    {

        public void SaveMSFeatures()
        {
            List<MSFeatureLight> lights = new List<MSFeatureLight>();
            for (int i = 0; i < 100; i++)
            {
                lights.Add(new MSFeatureLight());
                lights[i].ID = i;
            }

            IMSFeatureDAO cache = new MSFeatureDAOHibernate();
            cache.AddAll(lights);            
        }
    }
}
