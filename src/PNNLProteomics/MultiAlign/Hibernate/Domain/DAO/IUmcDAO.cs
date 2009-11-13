/// <file>IUmcDAO.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
    public interface IUmcDAO : IGenericDAO<Umc>
    {
        List<Umc> FindByMass(double mass);
        List<Umc> FindByMassRange(double mass1, double mass2);
    }
}
