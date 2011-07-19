using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;

namespace MultiAlignCore.IO.Features
{
    public interface IUmcClusterDAO : IGenericDAO<clsCluster>
    {
        ICollection<clsCluster> FindByMass(double mass);
    }
}
