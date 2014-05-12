#region

using System.Collections.Generic;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features;

#endregion

namespace MultiAlignCore.IO.SequenceData
{
    public interface IDatabaseSearchSequenceDAO : IGenericDAO<DatabaseSearchSequence>
    {
        List<DatabaseSearchSequence> FindByDatasetId(int datasetId, int featureId);
        List<DatabaseSearchSequence> FindByDatasetId(int datasetId);
    }
}