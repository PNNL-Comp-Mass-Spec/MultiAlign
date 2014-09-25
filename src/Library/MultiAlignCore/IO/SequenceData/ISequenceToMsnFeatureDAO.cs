#region

using System.Collections.Generic;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features;

#endregion

namespace MultiAlignCore.IO.SequenceData
{
    public interface ISequenceToMsnFeatureDAO : IGenericDAO<SequenceToMsnFeature>
    {
       List<SequenceToMsnFeature> FindByDatasetId(int datasetId, int featureId);
       List<SequenceToMsnFeature> FindByDatasetId(int datasetId);
    }
}