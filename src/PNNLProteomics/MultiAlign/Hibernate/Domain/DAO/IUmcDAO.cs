using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
    public interface IUmcDAO : IGenericDAO<clsUMC>
    {
        List<clsUMC> FindByMass(double mass);
        List<clsUMC> FindByMassRange(double mass1, double mass2);
    }
}
