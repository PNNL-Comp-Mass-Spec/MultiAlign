using System.Collections.Generic;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features
{
    public interface IMassTagMatchDAO : IGenericDAO<ClusterToMassTagMap>
    {
        void ClearAllMatches();
        List<ClusterToMassTagMap> FindByClusterId(int id);
    }
}
