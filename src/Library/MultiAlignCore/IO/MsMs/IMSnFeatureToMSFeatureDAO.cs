#region

using System.Collections.Generic;
using MultiAlignCore.Data;

#endregion

namespace MultiAlignCore.IO.MsMs
{
    public interface IMsnFeatureToMSFeatureDAO : IGenericDAO<MSFeatureToMSnFeatureMap>
    {
        List<MSFeatureToMSnFeatureMap> FindByDatasetId(int datasetId);
        List<MSFeatureToMSnFeatureMap> FindByUMCFeatureId(int datasetId, int featureId);
    }
}