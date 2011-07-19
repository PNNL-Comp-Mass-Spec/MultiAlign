using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
    public interface IUmcClusterDAO : IGenericDAO<clsCluster>
    {
        ICollection<clsCluster> FindByMass(double mass);
    }
}
