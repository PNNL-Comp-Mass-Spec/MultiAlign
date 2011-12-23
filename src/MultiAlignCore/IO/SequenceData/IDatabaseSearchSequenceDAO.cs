using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignCore.Data.SequenceData;
using MultiAlignCore.IO.Features;

namespace MultiAlignCore.IO.SequenceData
{    
    public interface IDatabaseSearchSequenceDAO: IGenericDAO<DatabaseSearchSequence>
    {
        List<DatabaseSearchSequence> FindByDatasetId(int datasetId);
    }
}
