using System;
using System.Collections.Generic;
using MultiAlignEngine.Features;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features
{
    public interface IUmcClusterDAO : IGenericDAO<UMCClusterLight>
    {
        ICollection<UMCClusterLight> FindByMass(double mass);
        void ClearAllClusters();
    }
}
