using System.Collections.Generic;
using PNNLOmics.Data.MassTags;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;


namespace MultiAlignTestSuite
{
    public class MassTagDOATest
    {

        public void SaveMassTags()
        {
            List<MassTagLight> lights = new List<MassTagLight>();
            for (int i = 0; i < 100; i++)
            {
                lights.Add(new MassTagLight());
                lights[i].ID = i;
            }

            IMassTagDAO cache = new MassTagDAO();
            cache.AddAll(lights);
            
        }
    }
}
