/// <file>IUmcClusterDAO.cs</copyright>
/// <copyright>Pacific Northwest National Laboratory</copyright>
/// <author email="Kevin.Crowell@pnl.gov">Kevin Crowell</author>

using System;
using System.Collections.Generic;
using System.Text;
using MultiAlignEngine.Features;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain.DAO
{
    public interface IUmcClusterDAO : IGenericDAO<clsCluster>
    {
        ICollection<clsCluster> FindByMass(double mass);
    }
}
