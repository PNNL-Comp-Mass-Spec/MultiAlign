using System.Collections.Generic;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Features
{
    public interface IMsnFeatureToMSFeatureDAO : IGenericDAO<MSFeatureToMSnFeatureMap>
    {
        List<MSFeatureToMSnFeatureMap> FindByDatasetId(int datasetId);
        List<MSFeatureToMSnFeatureMap> FindByUMCFeatureId(int datasetId, int featureId);
    }
}
